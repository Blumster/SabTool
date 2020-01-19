using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SabTool.Client.Pebble
{
    public class PblDiscFile : PblFile
    {
        public bool IsFileOpen { get; set; }
        public bool B13 { get; set; }
        public bool IsDLC { get; set; }
        public bool B15 { get; set; }
        public bool IsEOF { get; set; }
        public int F20 { get; set; }
        public long F24 { get; set; }
        public long F32 { get; set; }
        public long Offset { get; set; }
        public long FileSize { get; set; }
        public int F68 { get; set; }
        public byte[] F144UnkAlignedBuffer { get; set; }
        public int F144UnkALignedBufferInd { get; set; }
        public byte[] F148UnkBuffer { get; set; }

        public PblDiscFile()
            : base()
        {

        }

        public override long GetFileSize()
        {
            throw new NotImplementedException();
        }

        public override bool IsOpen() // 0xDBA9D0
        {
            return IsFileOpen;
        }

        public override void Read(byte[] destination, int bytesToRead)
        {
            var remainingBytes = FileSize - Offset;

            IsEOF = remainingBytes <= 0 && (remainingBytes < 0 || FileSize == Offset);

            if (bytesToRead)
        }

        public override bool Write(byte[] source, int bytesToWrite)
        {
            throw new NotImplementedException();
        }

        public override void Seek(long destOffset, SeekOrigin type)
        {
            throw new NotImplementedException();
        }

        public override long GetOffset()
        {
            throw new NotImplementedException();
        }

        public bool Open(string fileName, int a3)
        {
            return false;
        }

        public void SubDBA960(int a2)
        {
            if (F144UnkAlignedBuffer == null)
                F68 = a2;
        }

        public void SubDBA950() // 0xDBA950
        {
            F24 = -1L;
            F32 = -1L;
        }

        public void SubDBB110() // 0xDBB110
        {
            if (F144UnkAlignedBuffer == null)
            {
                if (F68 == -1)
                {
                    // todo: some shit
                }
                else
                {
                    F148UnkBuffer = new byte[F68 + 255];
                    F144UnkAlignedBuffer = F148UnkBuffer; // TODO: can't point inside the array, maybe use an index here?
                    F144UnkALignedBufferInd = 0;
                }
            }
        }

        public void SubDBB1A0() // 0xDBB1A0
        {
            if (F144UnkAlignedBuffer != null)
            {
                if (F68 == -1)
                {
                    // todo: some shit
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

            if (F68 != -1)
                v4 = F68;

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

                SubDBCFA0(F144UnkAlignedBuffer, v5);

                F24 = off;
                F32 = off + v5;

                Offset = originalOff;
            }
        }
    }
}
