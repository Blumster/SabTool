using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SabTool.Parsers.Base
{
    public abstract class Parser : IDisposable
    {
        private readonly Stack<long> _seekAdresses = new Stack<long>();

        public abstract string Name { get; }
        public Stream Stream { get; }
        public BinaryReader Reader { get; }

        public Parser(Stream stream)
        {
            Stream = stream;

            Reader = new BinaryReader(stream, Encoding.UTF8, true);
        }

        public void Seek(long offset, SeekOrigin origin = SeekOrigin.Begin)
        {
            _seekAdresses.Push(Stream.Position);

            Stream.Seek(offset, origin);
        }

        public void SeekBack()
        {
            if (_seekAdresses.Count == 0)
                throw new InvalidOperationException("You need to seek first to be able to jump back to the previous offset!");

            Stream.Position = _seekAdresses.Pop();
        }

        public void ClearSeekStack()
        {
            _seekAdresses.Clear();
        }

        public abstract bool Parse();

        public void Dispose()
        {
            // Leaving the stream open so it can be reused, because in one file there are thousands of smaller files written

            Reader.Dispose();
        }
    }
}
