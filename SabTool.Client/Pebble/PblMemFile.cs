using System;
using System.IO;

namespace SabTool.Client.Pebble
{
    public class PblMemFile : PblFile
    {
        public byte[] FilePayload { get; set; }
        public int FileLength { get; set; }
        public int FileMaxLength { get; set; }
        public int FileOffset { get; set; }
        public int FileF1C { get; set; }

        public PblMemFile()
        {
            F4 = 2;
            F8 &= 0xFE;
            FileF1C &= 0xF8;
        }

        public override long GetFileSize()
        {
            return FileLength;
        }

        public override bool Func16()
        {
            return false;
        }

        public override bool IsOpen()
        {
            return (FileF1C & 1) == 1;
        }

        public override void Read(byte[] destination, int bytesToRead)
        {
            if (destination == null)
                return;

            Array.Copy(FilePayload, FileOffset, destination, 0, bytesToRead);

            FileOffset += bytesToRead;
        }

        public override bool Write(byte[] source, int bytesToWrite)
        {
            if (FileOffset + bytesToWrite > FileMaxLength)
                throw new Exception("Buffer isn't big enough!");

            Array.Copy(source, 0, FilePayload, FileOffset, bytesToWrite);

            FileOffset += bytesToWrite;

            if (FileLength < FileOffset)
                FileLength = FileOffset;

            return true;
        }

        public override void Seek(long destOffset, SeekOrigin type)
        {
            switch (type)
            {
                case SeekOrigin.Begin:
                    FileOffset = (int)destOffset;
                    break;

                case SeekOrigin.Current:
                    FileOffset += (int)destOffset;
                    break;

                case SeekOrigin.End:
                    FileOffset = FileLength + (int)destOffset;
                    break;
            }
        }

        public override long GetOffset()
        {
            return FileOffset;
        }

        public override void Close()
        {
            FileF1C &= 0xFE;

            if ((FileF1C & 4) == 4)
            {
                FilePayload = null;

                FileF1C &= 0xFB;
            }
        }

        public virtual void Read2(byte[] destination, int bytesToRead)
        {
            Read(destination, bytesToRead);
        }

        public virtual void Seek2(int destOffsetLow, int destOffsetHigh, SeekOrigin type)
        {
            Seek(destOffsetLow + (((long)destOffsetHigh) << 32), type);
        }

        public void SetPayloadArray(byte[] payloadArray, int arraySize)
        {
            if (arraySize < 0)
                arraySize = payloadArray.Length;

            FileF1C |= 3;
            FilePayload = payloadArray;
            FileLength = arraySize;
            FileMaxLength = arraySize;
            FileOffset = 0;
        }

        public void ReadFromLooseFile(PblLooseFile looseFile)
        {
            // TODO
        }

        public bool Sub_DBA730(byte[] payloadArray, int arraySize, int maxArraySize)
        {
            if (arraySize < 0)
                arraySize = payloadArray.Length;

            if (maxArraySize < 0)
                maxArraySize = arraySize;

            FileLength = arraySize;
            FilePayload = payloadArray;
            FileF1C = (FileF1C & 0xFD) | 1;
            FileMaxLength = maxArraySize;
            FileOffset = 0;

            return true;
        }
    }
}
