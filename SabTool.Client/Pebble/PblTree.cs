using System;
using System.Collections.Generic;
using System.Text;

namespace SabTool.Client.Pebble
{
    public class PblTreeNode
    {
        public uint Dword0 { get; set; }
        public PblTreeNode NextNode { get; set; }
        public PblTreeNode PrevNode { get; set; }
        public PblTreeNode UnkNode { get; set; }
        public int Crc { get; set; }

        public PblTreeNode(PblTreeNode unkNode)
        {
            Dword0 &= 0xFFFFFFFC;
            NextNode = null;
            PrevNode = null;
            UnkNode = unkNode;
        }
    }

    public class PblTree<T>
    {
    }
}
