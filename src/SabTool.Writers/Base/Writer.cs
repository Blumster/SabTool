using System;
using System.IO;

namespace SabTool.Writers.Base
{
    public abstract class Writer
    {
        public Stream Stream { get; }

        public Writer(Stream stream)
        {
            Stream = stream;
        }

        public abstract bool Write();
    }
}
