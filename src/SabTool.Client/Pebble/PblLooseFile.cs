using System;
using System.IO;
using System.Text;

namespace SabTool.Client.Pebble
{
    public class PblLooseFile : PblFile
    {
        public static PblLooseFileBuffer StaticDiscFileData => new PblLooseFileBuffer();

        public PblDiscFile DiscFile { get; set; }
        public long F16FileOffset { get; set; }
        public long F24Offset { get; set; }
        public int F32Size { get; set; }
        public bool IsFromLooseFile { get; set; }
        public bool B37 { get; set; }
        public bool IsCustomDiscFile { get; set; }
        public bool B39 { get; set; }

        public PblLooseFile()
            : base()
        {
            B37 = true;
        }

        ~PblLooseFile()
        {
            if (DiscFile != null)
                DiscFile.Close();
        }

        #region Virtual Table 0x107BF08
        public override long GetFileSize() // 0x04
        {
            lock (StaticDiscFileData.Lock)
            {
                if (IsCustomDiscFile)
                    return DiscFile.GetFileSize();

                if (IsFromLooseFile)
                    return F32Size;

                return 0L;
            }
        }

        public override bool IsEndOfFile() // 0x08
        {
            lock (StaticDiscFileData.Lock)
            {
                if (IsCustomDiscFile)
                    return DiscFile.IsEndOfFile();

                if (IsFromLooseFile)
                    return F24Offset >= F32Size;

                return true;
            }
        }

        public override bool IsOpen() // 0x0C
        {
            lock (StaticDiscFileData.Lock)
            {
                if (IsCustomDiscFile)
                    return DiscFile.IsOpen();

                return IsFromLooseFile;
            }
        }

        public override void Read(byte[] destination, int bytesToRead) //0x14
        {
            lock (StaticDiscFileData.Lock)
            {
                if (IsCustomDiscFile)
                {
                    DiscFile.Read(destination, bytesToRead);
                    return;
                }

                if (IsFromLooseFile && F24Offset < F32Size)
                {
                    var fileOffset = DiscFile.GetOffset();
                    var calcOffset = F24Offset + F16FileOffset;
                    if (fileOffset != calcOffset)
                        DiscFile.Seek(calcOffset, SeekOrigin.Begin);

                    long toRead = bytesToRead;
                    if (toRead > F32Size - F24Offset)
                        toRead = F32Size - F24Offset;

                    DiscFile.Read(destination, (int)toRead);

                    F24Offset += toRead;
                }
            }
        }

        public override bool Write(byte[] source, int bytesToWrite) // 0x18
        {
            return false;
        }

        public override void Seek(long destOffset, SeekOrigin type) // 0x1C
        {
            lock (StaticDiscFileData.Lock)
            {
                if (IsCustomDiscFile)
                {
                    DiscFile.Seek(destOffset, type);
                    return;
                }

                switch (type)
                {
                    case SeekOrigin.Begin:
                        F24Offset = destOffset;
                        break;

                    case SeekOrigin.Current:
                        F24Offset += destOffset;
                        break;

                    case SeekOrigin.End:
                        F24Offset = F32Size;
                        F24Offset += destOffset;
                        break;
                }
            }
        }

        // Read2 not overridden

        public override long GetOffset() // 0x24
        {
            lock (StaticDiscFileData.Lock)
            {
                if (IsCustomDiscFile)
                    return DiscFile.GetOffset();

                return F24Offset;
            }
        }

        public override void Close() // 0x28
        {
            lock (StaticDiscFileData.Lock)
            {
                if (IsCustomDiscFile)
                {
                    if (DiscFile != null)
                    {
                        DiscFile.Close();
                        DiscFile = null;
                    }

                    IsCustomDiscFile = false;
                }
                else if (IsFromLooseFile)
                {
                    --StaticDiscFileData.RefCount;
                }

                if (StaticDiscFileData.RefCount == 0 && StaticDiscFileData.DiscFile != null && !StaticDiscFileData.Status2)
                    StaticDiscFileData.DiscFile = null;
            }
        }
        #endregion

        public bool Open(string fileName, int a3, bool a4)
        {
            lock (StaticDiscFileData.Lock)
            {
                Close();

                if (StaticDiscFileData.Status2 && StaticDiscFileData.DiscFile != null && !a4)
                {
                    if (!StaticDiscFileData.HeaderRead)
                    {
                        StaticDiscFileData.DiscFile.Seek(StaticDiscFileData.FileOffset, SeekOrigin.Begin);
                        StaticDiscFileData.DiscFile.Read(StaticDiscFileData.TempBuffer, 128);

                        StaticDiscFileData.Setup();

                        StaticDiscFileData.HeaderRead = true;
                    }

                    var fileNameItr = 0;
                    var off = 0;
                    bool match;

                    while (true)
                    {
                        var c = fileName[fileNameItr];

                        if (char.IsLetter(c))
                            c = char.ToLowerInvariant(c);

                        if (c == '\\')
                            c = '/';

                        var o = StaticDiscFileData.FileName[off++];

                        if (char.IsLetter(o))
                            o = char.ToLowerInvariant(o);

                        if (o == '\\')
                            o = '/';

                        match = c == o;
                        if (!match)
                            break;

                        if (c == 0)
                        {
                            match = o == 0;
                            break;
                        }

                        ++fileNameItr;
                    }

                    var firstInd = fileName.IndexOf('\\');
                    if (match || (firstInd != -1 && fileName.Substring(firstInd + 1) == StaticDiscFileData.FileName))
                    {
                        IsCustomDiscFile = false;
                        DiscFile = StaticDiscFileData.DiscFile;
                        F24Offset = 0L;

                        IsFromLooseFile = StaticDiscFileData.FileSize != -1L;
                        if (!IsFromLooseFile)
                        {
                            StaticDiscFileData.FileOffset += 128L;
                        }
                        else
                        {
                            F32Size = (int)StaticDiscFileData.FileSize;
                            F16FileOffset = StaticDiscFileData.FileOffset + 128;

                            StaticDiscFileData.FileOffset += StaticDiscFileData.FileSize + 128;

                            if ((StaticDiscFileData.FileSize & 0xF) != 0)
                                StaticDiscFileData.FileOffset += 16 - (StaticDiscFileData.FileSize & 0xF);
                        }

                        StaticDiscFileData.HeaderRead = false;

                        if (IsFromLooseFile)
                        {
                            ++StaticDiscFileData.RefCount;
                        }
                        else
                        {
                            IsCustomDiscFile = true;
                            DiscFile = new PblDiscFile
                            {
                                B13 = B37
                            };

                            if (DiscFile.Open(fileName, a3))
                            {
                                StaticDiscFileData.Status2 = false;

                                return true;
                            }

                            return false;
                        }
                    }
                }

                IsCustomDiscFile = true;
                DiscFile = new PblDiscFile
                {
                    B13 = B37
                };

                return DiscFile.Open(fileName, a3);
            }
        }

        public static bool GetStaticDiscFileInstance(string file) // 0xDC0FA0
        {
            if (StaticDiscFileData.DiscFile != null)
                return true;

            StaticDiscFileData.DiscFile = new PblDiscFile();
            StaticDiscFileData.DiscFile.SubDBA960(-1);
            StaticDiscFileData.DiscFile.B13 = false;

            if (StaticDiscFileData.DiscFile.Open(file, 0))
            {
                StaticDiscFileData.FileSize = StaticDiscFileData.DiscFile.GetFileSize();
                StaticDiscFileData.FileOffset = 0;
                StaticDiscFileData.HeaderRead = false;
                StaticDiscFileData.Status2 = true;
                StaticDiscFileData.RefCount = 0;

                return true;
            }

            StaticDiscFileData.DiscFile = null;
            StaticDiscFileData.Status2 = false;
            StaticDiscFileData.RefCount = 0;

            return false;
        }
    }

    public class PblLooseFileBuffer
    {
        public byte[] TempBuffer => new byte[128];

        public int Unk0 { get; set; }
        public int Size { get; set; }
        public string FileName { get; set; }
        public long FileOffset { get; set; }
        public long FileSize { get; set; }
        public PblDiscFile DiscFile { get; set; }
        public bool HeaderRead { get; set; }
        public bool Status2 { get; set; }
        public int RefCount { get; set; }
        public object Lock => new object();

        public void Setup()
        {
            Unk0 = BitConverter.ToInt32(TempBuffer, 0);
            Size = BitConverter.ToInt32(TempBuffer, 4);
            FileName = Encoding.UTF8.GetString(TempBuffer, 8, Math.Min(Array.IndexOf(TempBuffer, 0, 8) - 8, 120));
        }
    }
}
