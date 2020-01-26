using System;
using System.IO;
using System.Text;

namespace SabTool.Client.Streaming
{
    using Pebble;

    public class WSStreamBlockNode
    {
        public PblTreeNode<WSStreamBlock> TreeNode { get; set; }
        public WSStreamBlock StreamBlock { get; set; }

        public WSStreamBlockNode()
        {
            TreeNode = new PblTreeNode<WSStreamBlock>(null);
            TreeNode.Dword0 = (int)(TreeNode.Dword0 & 0xFFFFFF0F) | 0xC;

            StreamBlock = new WSStreamBlock(true);
        }

        public WSStreamBlockNode(int crc)
            : this()
        {
            TreeNode.Crc = crc;
        }
    }

    public class WSStreamBlock
    {
        private object _lock = new object();
        public static uint ActiveStreamBlockCount { get; private set; }

        public string FileName { get; set; }
        public float FieldA4 { get; set; }
        public float FieldA8 { get; set; }
        public float FieldAC { get; set; }
        public float FieldC0 { get; set; }
        public float FieldCC { get; set; }
        public float FieldD0 { get; set; }
        public float FieldD4 { get; set; }
        public float FieldDC { get; set; }
        public float FieldE0 { get; set; }
        public float FieldE4 { get; set; }
        public uint Flags { get; set; }       // 0x108
        public int Field198_CRC { get; set; } // 0x198

        public WSStreamBlock(bool unkBaseClassStuff)
        {
            lock (_lock)
                ++ActiveStreamBlockCount;

            Flags = 0xAAAAAAAA; // Allocation memsets it to this value
            Flags &= 0xFFCFFEE7;
            Field198_CRC = -1;

            // ...

            Flags = (Flags & 0xFFF10000) | 0x10000;
        }

        ~WSStreamBlock()
        {
            lock (_lock)
                --ActiveStreamBlockCount;
        }

        public void SetFileNameFromFormat(string format, params object[] args)
        {
            FileName = string.Format(format, args);
        }

        public void ReadSomeArrays(BinaryReader br)
        {

        }

        public void Sub659F20(BinaryReader br)
        {

        }
    }
}
