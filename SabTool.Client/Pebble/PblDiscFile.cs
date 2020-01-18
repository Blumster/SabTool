using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SabTool.Client.Pebble
{
    public class PblDiscFile : PblFile
    {
        public bool B13 { get; set; }
        public int F68 { get; set; }
        public byte[] F144 { get; set; }

        public PblDiscFile()
            : base()
        {

        }

        public bool Open(string fileName, int a3)
        {
            return false;
        }

        public void SubDBA960(int a2)
        {
            if (F144 == null)
                F68 = a2;
        }

        public override long GetFileSize()
        {
            throw new NotImplementedException();
        }

        public override bool IsOpen()
        {
            throw new NotImplementedException();
        }

        public override void Read(byte[] destination, int bytesToRead)
        {
            throw new NotImplementedException();
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
    }
}
