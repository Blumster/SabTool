using System;
using System.IO;

namespace SabTool.Client.Pebble
{
    public class PblMemFile : PblFile
    {
        public byte[] Payload { get; set; }
        public int Length { get; set; }
        public int MaxLength { get; set; }
        public int Offset { get; set; }
        public byte Flags1C { get; set; }

        public PblMemFile() // 0xDBA7C0
        {
            F4 = 2;
            F8 &= 0xFE;
            Flags1C &= 0xF8;
        }

        ~PblMemFile() // 0xDBB0D0
        {
            if (IsOpen())
            {
                Close();
            }
        }

        public override long GetFileSize() // 0xDBA7F0
        {
            return Length;
        }

        public override bool Func16()
        {
            return false;
        }

        public override bool IsOpen() // 0xDBA800
        {
            return (Flags1C & 1) == 1;
        }

        public override void Read(byte[] destination, int bytesToRead) // 0xDBA810
        {
            if (destination == null)
                return;

            Array.Copy(Payload, Offset, destination, 0, bytesToRead);

            Offset += bytesToRead;
        }

        public override bool Write(byte[] source, int bytesToWrite) // 0xDBA860
        {
            if (Offset + bytesToWrite > MaxLength)
                throw new Exception("Buffer isn't big enough!");

            Array.Copy(source, 0, Payload, Offset, bytesToWrite);

            Offset += bytesToWrite;

            if (Length < Offset)
                Length = Offset;

            return true;
        }

        public override void Seek(long destOffset, SeekOrigin type) // 0xDBA8C0
        {
            switch (type)
            {
                case SeekOrigin.Begin:
                    Offset = (int)destOffset;
                    break;

                case SeekOrigin.Current:
                    Offset += (int)destOffset;
                    break;

                case SeekOrigin.End:
                    Offset = Length + (int)destOffset;
                    break;
            }
        }

        public override long GetOffset() // 0xDBA900
        {
            return Offset;
        }

        public override void Close() // 0xDBA7A0
        {
            Flags1C &= 0xFE;

            if ((Flags1C & 4) == 4)
            {
                Payload = null;

                Flags1C &= 0xFB;
            }
        }

        public virtual void Read2(byte[] destination, int bytesToRead) // 0xDBA920
        {
            Read(destination, bytesToRead);
        }

        public virtual void Seek2(int destOffsetLow, int destOffsetHigh, SeekOrigin type) // 0xDBA930
        {
            Seek(destOffsetLow + (((long)destOffsetHigh) << 32), type);
        }

        public bool SetPayloadArray(byte[] payloadArray, int arraySize) // 0xDBA6F0
        {
            if (arraySize < 0)
                arraySize = payloadArray.Length;

            Flags1C |= 3;
            Payload = payloadArray;
            Length = arraySize;
            MaxLength = arraySize;
            Offset = 0;

            return true;
        }

        public bool ReadFromLooseFile(PblLooseFile looseFile) // 0xDBA630
        {
            if (looseFile.IsOpen())
            {
                var fileSize = (int)looseFile.GetFileSize();

                Flags1C |= 7;

                Payload = new byte[fileSize];
                Length = fileSize;
                MaxLength = fileSize;
                Offset = 0;

                // Skipping fragmented reading as in the client
                looseFile.Read(Payload, fileSize);
                return true;
            }

            return false;
        }

        public bool ReadFromDiscFile(PblDiscFile discFile) // 0xDBA560
        {
            if (discFile.IsOpen())
            {
                var fileSize = (int)discFile.GetFileSize();

                Flags1C |= 7;

                Payload = new byte[fileSize];
                Length = fileSize;
                MaxLength = fileSize;
                Offset = 0;

                // Skipping fragmented reading as in the client
                discFile.Read(Payload, fileSize);
                return true;
            }

            return false;
        }

        public bool SubDBA730(byte[] payloadArray, int arraySize, int maxArraySize) // 0xDBA730
        {
            if (arraySize < 0)
                arraySize = payloadArray.Length;

            if (maxArraySize < 0)
                maxArraySize = arraySize;

            Length = arraySize;
            Payload = payloadArray;
            Flags1C = (byte)((Flags1C & 0xFD) | 1);
            MaxLength = maxArraySize;
            Offset = 0;

            return true;
        }
    }
}
