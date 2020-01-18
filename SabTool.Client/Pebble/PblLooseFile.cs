using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SabTool.Client.Pebble
{
    public class PblLooseFile : PblFile
    {
        public static bool StaticDiscFileHeaderRead { get; set; }
        public static bool StaticDiscFileStatus2 { get; set; }
        public static long StaticDiscFileSize { get; set; }
        public static long StaticDiscFileOffset { get; set; }
        public static int StaticDiscFileRefCount { get; set; }
        public static PblDiscFile StaticDiscFile { get; set; }
        public static byte[] StaticFileBuffer { get; } = new byte[128];

        public PblDiscFile DiscFile { get; set; }
        public long F16FileOffset { get; set; }
        public long F24Offset { get; set; }
        public int F32Size { get; set; }
        public bool B36 { get; set; }
        public bool B37 { get; set; }
        public bool IsDiscFileAttached { get; set; }

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
            if (IsDiscFileAttached)
                return DiscFile.GetFileSize();

            if (B36)
                return F32Size;

            return 0L;
        }

        public override bool IsEndOfFile() // 0x08
        {
            if (IsDiscFileAttached)
                return DiscFile.IsEndOfFile();

            if (B36)
                return F24Offset >= F32Size;

            return true;
        }

        public override bool IsOpen() // 0x0C
        {
            if (IsDiscFileAttached)
                return DiscFile.IsOpen();

            return B36;
        }

        public override void Read(byte[] destination, int bytesToRead) //0x14
        {
            if (IsDiscFileAttached)
            {
                DiscFile.Read(destination, bytesToRead);
                return;
            }

            if (B36 && F24Offset < F32Size)
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

        public override bool Write(byte[] source, int bytesToWrite) // 0x18
        {
            return false;
        }

        public override void Seek(long destOffset, SeekOrigin type) // 0x1C
        {
            if (IsDiscFileAttached)
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

        public override long GetOffset() // 0x24
        {
            if (IsDiscFileAttached)
                return DiscFile.GetOffset();

            return F24Offset;
        }

        public override void Close() // 0x28
        {
            if (IsDiscFileAttached)
            {
                if (DiscFile != null)
                {
                    DiscFile.Close();
                    DiscFile = null;
                }

                IsDiscFileAttached = false;
            }
            else if (B36)
            {
                --StaticDiscFileRefCount;
            }

            if (StaticDiscFileRefCount == 0 && StaticDiscFile != null && !StaticDiscFileStatus2)
                StaticDiscFile = null;
        }
        #endregion

        public bool Open(string fileName, int a3, bool a4)
        {
            Close();

            if (StaticDiscFileStatus2 && StaticDiscFile != null && a4)
            {
                if (!StaticDiscFileHeaderRead)
                {
                    StaticDiscFile.Seek(StaticDiscFileOffset, SeekOrigin.Begin);
                    StaticDiscFile.Read(StaticFileBuffer, 128);

                    StaticDiscFileHeaderRead = true;
                }

                var fileNameItr = 0;
                var off = 8;
                bool match;

                while (true)
                {
                    var c = fileName[fileNameItr];

                    if (char.IsLetter(c))
                        c = char.ToLowerInvariant(c);

                    if (c == '\\')
                        c = '/';

                    var o = (char)StaticFileBuffer[off++];

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

                if (match || false)
                {
                    IsDiscFileAttached = false;
                    DiscFile = StaticDiscFile;
                    F24Offset = 0;
                }

                // todo: lot of shit
            }

            IsDiscFileAttached = true;
            DiscFile = new PblDiscFile
            {
                B13 = B37
            };

            return DiscFile.Open(fileName, a3);
        }

        public static bool GetStaticDiscFileInstance(string file) // 0xDC0FA0
        {
            if (StaticDiscFile != null)
                return true;

            StaticDiscFile = new PblDiscFile();
            StaticDiscFile.SubDBA960(-1);
            StaticDiscFile.B13 = false;

            if (StaticDiscFile.Open(file, 0))
            {
                StaticDiscFileSize = StaticDiscFile.GetFileSize();
                StaticDiscFileOffset = 0;
                StaticDiscFileHeaderRead = false;
                StaticDiscFileStatus2 = true;
                StaticDiscFileRefCount = 0;

                return true;
            }

            StaticDiscFile = null;
            StaticDiscFileStatus2 = false;
            StaticDiscFileRefCount = 0;

            return false;
        }
    }
}
