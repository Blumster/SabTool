using System;
using System.IO;
using System.Threading;

namespace SabTool.Client.Pebble
{
    public class PblDiscFile : PblFile
    {
        public static ManualResetEvent ResumeIOEvent = new ManualResetEvent(true);
        public static ManualResetEvent DiscfileIODoneEvent = new ManualResetEvent(true);

        public static int AvailableBufferInd = 32;
        public static byte[][] Buffers = new byte[32][];

        public bool IsFileOpen { get; set; }
        public bool B13 { get; set; }
        public bool IsDLC { get; set; }
        public bool B15 { get; set; }
        public bool IsEOF { get; set; }
        public int F20 { get; set; }
        public long F24 { get; set; }
        public long F32 { get; set; }
        public FileStream FileStream { get; set; }
        public int F44 { get; set; }
        public long Offset { get; set; }
        public long FileSize { get; set; }
        public int F64 { get; set; }
        public int CustomArraySize { get; set; }
        public int F72 { get; set; }
        public int F76 { get; set; }
        // Overlapped
        public int F100 { get; set; }
        public long F104 { get; set; }
        public int F112 { get; set; }
        public byte[] ReadBuffer { get; set; }
        public byte[] WriteBuffer { get; set; }
        public int F124 { get; set; }
        public int F128 { get; set; }
        public int WriteBufferLength { get; set; }
        public int F136 { get; set; }
        public int Method { get; set; }
        public byte[] F144UnkAlignedBuffer { get; set; }
        public int F144UnkALignedBufferInd { get; set; }
        public byte[] F148UnkBuffer { get; set; }
        public int F152 { get; set; }
        public int F156 { get; set; }
        public string FileName { get; set; }

        static PblDiscFile() // doesn't exist, it is handled in the constructor once
        {
            for (var i = 0; i < 32; ++i)
                Buffers[i] = new byte[4096];
        }

        public PblDiscFile() // 0xDBC8E0
            : base()
        {
            F8 &= 0xFE;
            B13 = true;
            F24 = -1L;
            F32 = -1L;
            CustomArraySize = -1;

            F4 = 1;
        }

        ~PblDiscFile() // 0xDBBEE0
        {
            if (IsFileOpen)
                Close();

            // some unk shit

            DeallocUnkBuffer();

            F76 = 0;
            F104 = 0L;
            F112 = 0;

            if (ReadBuffer != null)
                ReadBuffer = null;

            WriteBuffer = null;
        }

        #region Virtual table 0x107BDFC
        public override long GetFileSize() // 0xDBCF80
        {
            return FileStream.Length;
        }

        public override bool IsEndOfFile() // 0x7BC980
        {
            return IsEOF;
        }

        public override bool IsOpen() // 0xDBA9D0
        {
            return IsFileOpen;
        }

        public override bool Func16() // 0xDBE060
        {
            if (!B15)
                return false;

            if (F124 == 0 || F124 == 4)
                return false;

            // overlapped operation, won't finish if not needed

            if (F124 - 1 <= 2)
                SubDBD530();

            if (F124 != 4)
                SubDBDC30();

            return F124 != 4;
        }

        public override void Read(byte[] destination, int bytesToRead) // 0xDBB2D0
        {
            var remainingBytes = FileSize - Offset;
            long size = bytesToRead;

            IsEOF = remainingBytes <= 0 && (remainingBytes < 0 || FileSize == Offset);

            if (bytesToRead > remainingBytes)
                size = remainingBytes;

            var arraySize = 4096;
            if (CustomArraySize != -1)
                arraySize = CustomArraySize;

            if (size < 0 || (size < int.MaxValue && size < arraySize) || (Offset & 0x7FF) != 0)
            {
                if (F32 < size + Offset || Offset < F24)
                {
                    var size_h = (int)(size >> 32);
                    var v11 = (int)size;
                    var v17 = v11;
                    var v16 = 0L;

                    if (size != 0)
                    {
                        long PAIR(int high, int low)
                        {
                            return (high << 32) | low;
                        }

                        while (true)
                        {
                            SubDBB210();

                            var v13 = (int)((F32 - Offset) >> 32);
                            var v12 = (int)F32 - (int)Offset;

                            if (PAIR(v13, v12) >= PAIR(size_h, v11))
                                v13 = size_h;
                            else
                                v11 = v12;

                            Array.Copy(F144UnkAlignedBuffer, Offset - F24, destination, v16, v11);

                            v16 += PAIR(v13, v11);

                            var v14 = (int)(PAIR(size_h, v17) - PAIR(v13, v11));

                            size_h = (int)((PAIR(size_h, v17) - PAIR(v13, v11)) >> 32);

                            Offset += v11;

                            if (PAIR(size_h, v14) == 0)
                                break;

                            v11 = v14;
                        }
                    }
                }
                else
                {
                    Array.Copy(F144UnkAlignedBuffer, Offset - F24, destination, 0, size);
                    Offset += size;
                }
            }
            else
            {
                SubDBCFA0(null, (int)Offset, (int)size);
                Offset += size;
            }

            IsEOF = Offset >= FileSize;
        }

        public override bool Write(byte[] source, int bytesToWrite) // 0xDBD110
        {
            if (source != null && bytesToWrite > 0 && IsFileOpen)
            {
                try
                {
                    FileStream.Write(source, 0, bytesToWrite);
                }
                catch
                {
                    return false;
                }

                Offset += bytesToWrite;

                F24 = -1L;
                F32 = -1L;

                if (Offset > FileSize)
                    FileSize = Offset;
            }

            return false;
        }

        public override void Seek(long destOffset, SeekOrigin type) // 0xDBD190, is this function ever used? It's bugged...
        {
            ResumeIOEvent.WaitOne();
            DiscfileIODoneEvent.Reset();

            if (IsFileOpen)
            {
                if (type == SeekOrigin.Begin)
                    Offset = destOffset;
                else if (type == SeekOrigin.End)
                    Offset = GetFileSize() + destOffset;
                else if (type == SeekOrigin.Current)
                    Offset += destOffset;

                IsEOF = Offset == FileSize;

                FileStream.Seek(destOffset, SeekOrigin.Begin); // Bug in the client? Updates Offset by the type, but at the end always moves with Begin
            }

            DiscfileIODoneEvent.Set();
        }

        public override void Read2(byte[] dest, int bytesToRead) // 0xDBD340
        {
            ResumeIOEvent.WaitOne();
            DiscfileIODoneEvent.Reset();

            if (FileStream.Read(dest, 0, bytesToRead) == 0)
            {
                throw new Exception("Unable to read!");
            }

            Offset += bytesToRead;

            DiscfileIODoneEvent.Set();
        }

        public override long GetOffset() // 0xDBD320
        {
            return Offset;
        }

        public override void Close() // 0xDBCD30
        {
            if (IsOpen())
            {
                DeallocUnkBuffer();

                FileStream.Close();
                FileStream = null;

                IsFileOpen = false;
                FileName = string.Empty;

                SubDBA950();
            }
        }

        public virtual bool CreateFile(string fileName) // 0xDBD480
        {
            if (Method != 0)
                return false;

            Method = 0;

            try
            {
                var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                fs.Close();

                return true;
            }
            catch
            {

            }

            return false;
        }

        public virtual void SubDBE0E0(byte[] buffer, int a3) // 0xDBE0E0
        {
            if (Method != 0)
                return;

            var v4 = a3;
            if (a3 > FileSize - Offset)
                v4 = (int)(FileSize - Offset);

            if (buffer == null || v4 <= 0 || !IsFileOpen || ReadBuffer != null || (F124 != 0 && F124 != 4))
                return;

            WriteBuffer = buffer;

            var v6 = (int)(Offset >> 32) == 0;
            var v7 = Offset < 0;

            if (v7 || v6)
            {
                if (v7)
                    Offset = 0L;
                else
                    v6 = (Offset & 0xFFFFFFFF) == 0;
            }

            F128 = 0;
            WriteBufferLength = 0;
            F136 = 0;

            F104 = Offset % F76;
            if (F104 != 0)
            {
                var v11 = F76 - (int)F104;

                F128 = v11;

                if (v11 > v4)
                    F128 = v4;
            }

            WriteBufferLength = F76 * ((v4 - F128) / F76);
            F136 = v4 - WriteBufferLength - F128;

            if (F128 == 0)
            {
                if (F136 != 0)
                    ReadBuffer = new byte[F76];

                Method = 2;
                F124 = 1;
                SubDBDC30();
            }
        }

        public virtual void SubDBDD70(byte[] buffer, int a3) // 0xDBDD70
        {
            if (Method != 0 || buffer == null || a3 <= 0 || !IsFileOpen || (F124 != 0 && F124 != 4))
                return;

            WriteBuffer = buffer;

            if (Offset < 0)
                Offset = 0L;

            F128 = 0;
            WriteBufferLength = 0;
            F136 = 0;
            F104 = 0;
            F124 = 1;
            Method = 3;
            SubDBDC30();
        }

        public virtual bool DeleteFile(string fileName) // 0xDBD4D0
        {
            if (Method != 0)
                return false;

            Method = 0;

            try
            {
                File.Delete(fileName);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual void SubDBD600() // 0xDBD600
        {
            if (Method == 0 || F124 == 0 || F124 == 4)
                return;

            while (Func16());

            if (ReadBuffer != null)
                ReadBuffer = null;

            WriteBuffer = null;
            F124 = 0;
            F128 = 0;
            WriteBufferLength = 0;
            F136 = 0;
            Method = 0;
        }

        public virtual void Seek2(long dest, SeekOrigin type) // 0xDBD510
        {
            Seek(dest, type);
        }
        #endregion
        public void SubDBA950() // 0xDBA950
        {
            F24 = -1L;
            F32 = -1L;
        }

        public void SubDBA960(int a2) // 0xDBA960
        {
            if (F144UnkAlignedBuffer == null)
                CustomArraySize = a2;
        }

        public void AllocUnkBuffer() // 0xDBB110
        {
            if (F144UnkAlignedBuffer == null)
            {
                if (CustomArraySize == -1)
                {
                    if (AvailableBufferInd == 0)
                        throw new Exception("No more static buffers are available!");

                    AvailableBufferInd -= 1;

                    F144UnkAlignedBuffer = Buffers[AvailableBufferInd];
                }
                else
                {
                    F148UnkBuffer = new byte[CustomArraySize + 255];
                    F144UnkAlignedBuffer = F148UnkBuffer; // TODO: can't point inside the array, maybe use an index here?
                    F144UnkALignedBufferInd = 0;
                }
            }
        }

        public void DeallocUnkBuffer() // 0xDBB1A0
        {
            if (F144UnkAlignedBuffer != null)
            {
                if (CustomArraySize == -1)
                {
                    if (AvailableBufferInd < 32)
                        AvailableBufferInd++;
                }

                F24 = -1L;
                F32 = -1L;
                F144UnkAlignedBuffer = null;
                F148UnkBuffer = null;
            }
        }

        public void SubDBB210() // 0xDBB210
        {
            var originalOff = Offset;

            var off = Offset & 0xFFFFF800;
            var v4 = 4096;
            long v5;

            if (CustomArraySize != -1)
                v4 = CustomArraySize;

            if (FileSize - Offset >= v4)
            {
                v5 = v4;
            }
            else
            {
                v5 = FileSize - Offset;
            }

            if (off != F24 || F32 != off + v5)
            {
                Offset = off;

                SubDBCFA0(F144UnkAlignedBuffer, 0, (int)v5);

                F24 = off;
                F32 = off + v5;

                Offset = originalOff;
            }
        }

        public bool CreateFile2(string fileName) // 0xDBCA50, unfinished
        {
            if (IsFileOpen)
                Close();

            if (string.IsNullOrEmpty(fileName))
                return false;

            FileName = fileName;

            var colonInd = FileName.IndexOf(':');
            var fixedFolderName = colonInd != -1 ? fileName.Substring(FileName.IndexOf(':') + 2) : fileName;

            if (fixedFolderName.IndexOf('\\') != -1 || fixedFolderName.IndexOf('/') != -1)
                Directory.CreateDirectory(Path.GetDirectoryName(fixedFolderName.Replace('/', '\\')));

            try
            {
                FileStream = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                IsFileOpen = true;
            }
            catch
            {
                IsFileOpen = false;
            }

            if (IsFileOpen)
            {
                Offset = 0;
                FileSize = 0;
            }

            return IsFileOpen;
        }

        public void SubDBCFA0(byte[] dest, int offset, int size) // 0xDBCFA0
        {
            ResumeIOEvent.WaitOne();
            DiscfileIODoneEvent.Reset();

            if (dest != null && size > 0 && IsFileOpen)
            {
                var seekPos = ((Offset + 2047) & 0xFFFFF800) - 2048;
                var v10 = (int)(Offset - seekPos);
                var bytes = new byte[2048];

                if (Offset - seekPos == 2048)
                {
                    seekPos = Offset;
                    v10 = 0;
                }

                if (Offset == 0)
                {
                    seekPos = 0;
                    v10 = 0;
                }

                FileStream.Seek(seekPos, SeekOrigin.Begin);

                var v11 = size;

                if (v10 != 0)
                {
                    FileStream.Read(bytes, 0, 2048);

                    dest[offset] = bytes[v10];
                    v11 = v10 + size - 2048;
                    offset += (int)-v10 + 2048;
                }

                var v12 = (int)(((v11 + 2047) & 0xFFFFF800) - 2048);
                var v13 = v11 - v12;
                if (v11 - v12 == 2048)
                {
                    v12 = v11;
                    v13 = 0;
                }

                if (v12 != 0)
                {
                    FileStream.Read(dest, offset, v12);
                    offset += v12;
                }

                if (v13 != 0)
                {
                    FileStream.Read(bytes, 0, 2048);

                    Array.Copy(bytes, 0, dest, offset, v13);
                }
            }

            DiscfileIODoneEvent.Set();
        }

        public void SubDBD270(long destOffset, SeekOrigin type) // 0xDBD270
        {
            if (IsFileOpen)
            {
                if (type == SeekOrigin.Begin)
                    Offset = destOffset;
                else if (type == SeekOrigin.End)
                    Offset = GetFileSize() + destOffset;
                else if (type == SeekOrigin.Current)
                    Offset += destOffset;

                if (Offset >= FileSize)
                {
                    FileSize = Offset;
                }

                FileStream.Seek(destOffset, SeekOrigin.Begin); // Bug in the client? Updates Offset by the type, but at the end always moves with Begin
            }
        }

        public bool CreateFileW2(string fileName, int a3) // 0xDBD3B0
        {
            if (IsFileOpen)
                Close();

            B15 = true;

            if (!string.IsNullOrEmpty(fileName))
            {
                var mode = FileMode.OpenOrCreate;
                var access = FileAccess.ReadWrite;
                var share = FileShare.ReadWrite;

                if (a3 == 1)
                {
                    access = FileAccess.Read;
                    share = FileShare.Read;
                    mode = FileMode.Open;
                }
                else if (a3 == 2)
                {
                    access = FileAccess.Write;
                    share = FileShare.Write;
                    mode = FileMode.Create;
                }

                try
                {
                    FileStream = new FileStream(FileName, mode, access, share, 4096, true);
                    IsFileOpen = true;
                }
                catch
                {
                    IsFileOpen = false;
                }

                if (IsFileOpen)
                {
                    FileSize = FileStream.Length;
                }

                F76 = 2048;
                SubDBD600();

                return IsFileOpen;
            }

            return false;
        }

        public void SubDBD530() // 0xDBD530
        {
            switch (F124)
            {
                case 1:
                    if (Method == 2)
                        Array.Copy(ReadBuffer, F104, WriteBuffer, 0, F128);
                    F124 = 2;
                    break;

                case 2:
                    F124 = 3;
                    break;

                case 3:
                    if (Method == 2)
                        Array.Copy(ReadBuffer, 0, WriteBuffer, F128, F136);

                    F124 = 4;
                    Offset += F128 + WriteBufferLength + F136;
                    Method = 0;

                    ReadBuffer = null;
                    WriteBuffer = null;
                    break;
            }
        }

        public bool Open(string fileName, int a3) // 0xDBD6C0
        {
            ResumeIOEvent.WaitOne();
            DiscfileIODoneEvent.Reset();

            if (IsFileOpen)
                Close();

            SubDBA950();

            if (!string.IsNullOrEmpty(fileName))
            {
                if (fileName.StartsWith("dlc", StringComparison.InvariantCultureIgnoreCase))
                    IsDLC = true;

                AllocUnkBuffer();

                try
                {
                    FileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                    IsFileOpen = true;
                }
                catch
                {
                    IsFileOpen = false;
                }

                if (IsFileOpen)
                {
                    Offset = 0;

                    var calcFileSize = GetFileSize();

                    Offset = calcFileSize;

                    if (calcFileSize > FileSize)
                        FileSize = calcFileSize;

                    FileStream.Seek(0, SeekOrigin.Begin);

                    FileSize = GetOffset();

                    if (IsFileOpen)
                    {
                        Offset = 0;

                        if (FileSize < 0)
                            FileSize = 0;

                        FileStream.Seek(0, SeekOrigin.Begin);
                    }
                }

                IsEOF = FileSize == 0;

                DiscfileIODoneEvent.Set();

                return IsFileOpen;
            }

            DiscfileIODoneEvent.Set();

            return false;
        }

        public bool WOpen(string fileName, int a3) // 0xDBD8D0
        {
            return Open(fileName, a3);
        }

        public bool Reopen(string fileName, int a3) // 0xDBDAE0
        {
            ResumeIOEvent.WaitOne();
            DiscfileIODoneEvent.Reset();

            if (IsFileOpen)
                Close();

            if (!string.IsNullOrEmpty(fileName))
            {
                if (fileName.StartsWith("dlc", StringComparison.InvariantCultureIgnoreCase))
                    IsDLC = true;

                try
                {
                    FileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                    IsFileOpen = true;
                }
                catch
                {
                    IsFileOpen = false;
                }

                if (IsFileOpen)
                    FileSize = FileStream.Length;

                IsEOF = FileSize == 0;
            }

            DiscfileIODoneEvent.Set();

            return IsFileOpen;
        }

        public void SubDBDC30() // 0xDBDC30, missing overlapped
        {
            var byteCount = 0;
            var v3 = false;
            byte[] buffer = null;
            var bufferOff = 0;

            if (F124 == 1)
            {
                if (F128 != 0)
                {
                    byteCount = F76;
                    // missing overlapped

                    if (Method == 3)
                        buffer = WriteBuffer;
                    else
                        buffer = ReadBuffer;

                    bufferOff = 0;

                    v3 = true;
                }
                else
                    F124 = 2;
            }

            if (F124 == 2)
            {
                if (WriteBufferLength > 0)
                {
                    bufferOff = F128;
                    // missing overlapped
                    byteCount = WriteBufferLength;
                    buffer = WriteBuffer;
                    v3 = true;
                }
                else
                    F124 = 3;
            }

            if (F124 != 3)
            {
                if (!v3)
                    return;

                if (Method == 3)
                    FileStream.Write(buffer, bufferOff, byteCount); // missing overlapped
                else
                    FileStream.Read(buffer, bufferOff, byteCount); // missing overlapped

                return;
            }

            if (F136 == 0)
            {
                F124 = 4;
                Offset += F128 + WriteBufferLength;

                if (ReadBuffer != null)
                    ReadBuffer = null;

                ReadBuffer = null;
                WriteBuffer = null;
                bufferOff = 0;
                Method = 0;

                if (!v3)
                    return;

                if (Method == 3)
                    FileStream.Write(buffer, bufferOff, byteCount); // missing overlapped
                else
                    FileStream.Read(buffer, bufferOff, byteCount); // missing overlapped

                return;
            }

            // missing overlapped
            if (Method == 3)
            {
                byteCount = F136;
                buffer = WriteBuffer;
                bufferOff = F128 + WriteBufferLength;
            }
            else
            {
                byteCount = F76;
                buffer = ReadBuffer;
                bufferOff = 0;
            }

            if (Method == 3)
                FileStream.Write(buffer, bufferOff, byteCount); // missing overlapped
            else
                FileStream.Read(buffer, bufferOff, byteCount); // missing overlapped
        }

        public bool SubDBE060() // 0xDBE060, missing overlapped
        {
            if (!B15 || F124 == 0 || F124 == 4)
                return false;

            // missing overlapped

            if (F124 - 1 <= 2)
                SubDBD530();

            if (F124 != 4)
            {
                SubDBDC30();
                return true;
            }

            return false;
        }
    }
}
