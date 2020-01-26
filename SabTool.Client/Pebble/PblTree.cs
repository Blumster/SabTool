using System;
using System.Collections.Generic;
using System.Text;

namespace SabTool.Client.Pebble
{
    public class PblTreeNode<T>
    {
        public int Dword0 { get; set; }
        public PblTreeNode<T> NextNode { get; set; }
        public PblTreeNode<T> PrevNode { get; set; }
        public PblTreeNode<T> UnkNode { get; set; }
        public int Crc { get; set; }
        public T Data { get; set; }

        public PblTreeNode(PblTreeNode<T> unkNode)
        {
            Dword0 &= -4;
            NextNode = null;
            PrevNode = null;
            UnkNode = unkNode;
        }

        public PblTreeNode<T> GetHighestNode()
        {
            var result = this;

            for (var i = NextNode; i != null; i = i.NextNode)
                result = i;

            return result;
        }

        public static PblTreeNode<T> SubDC79C0(PblTreeNode<T> thisNode)
        {
            var result = thisNode.PrevNode;

            if (result != null)
            {
                for (var i = result.NextNode; i != null; i = i.NextNode)
                    result = i;
            }
            else
            {
                for (result = thisNode.UnkNode; result != null; result = result.UnkNode)
                {
                    if (thisNode != result.PrevNode)
                        break;

                    thisNode = result;
                }
            }

            return result;
        }

        public void SubDC7C50(PblTreeNode<T> node, ref PblTreeNode<T> refNode)
        {
            var thisNode = this;

            while (node != refNode)
            {
                if (node != null && (node.Dword0 & 1) == 0)
                    break;

                var v4 = thisNode.NextNode;
                if (node == v4)
                {
                    v4 = thisNode.PrevNode;
                    if (v4 == null)
                        return;

                    if ((v4.Dword0 & 1) == 0)
                    {
                        v4.Dword0 |= 1;

                        var v5 = thisNode.PrevNode;

                        thisNode.Dword0 &= -2;
                        thisNode.PrevNode = v5.NextNode;

                        if (v5.NextNode != null)
                            v5.NextNode.UnkNode = thisNode;

                        v5.UnkNode = thisNode.UnkNode;

                        if (thisNode.UnkNode != null)
                        {
                            if (thisNode == thisNode.UnkNode.NextNode)
                                thisNode.UnkNode.NextNode = v5;
                            else
                                thisNode.UnkNode.PrevNode = v5;
                        }
                        else
                        {
                            refNode = v5;
                        }

                        v5.NextNode = thisNode;

                        thisNode.UnkNode = v5;

                        v4 = thisNode.PrevNode;
                        if (v4 == null)
                            return;
                    }

                    var v8 = v4.NextNode;
                    if (v8 == null || (v8.Dword0 & 1) != 0)
                    {
                        if (v4.PrevNode == null || (v4.PrevNode.Dword0 & 1) != 0)
                        {
                            v4.Dword0 &= -2;
                            node = thisNode;
                            thisNode = thisNode.UnkNode;
                            continue;
                        }
                    }

                    var v10 = v4.PrevNode;
                    if (v10 == null || (v10.Dword0 & 1) != 0)
                    {
                        v8.Dword0 |= 1;

                        var v11 = v4.NextNode;

                        v4.Dword0 &= -2;
                        v4.NextNode = v11.PrevNode;

                        if (v11.PrevNode != null)
                            v11.PrevNode.UnkNode = v4;

                        v11.UnkNode = v4.UnkNode;

                        if (v4.UnkNode != null)
                        {
                            if (v4 == v4.UnkNode.PrevNode)
                                v4.UnkNode.PrevNode = v11;
                            else
                                v4.UnkNode.NextNode = v11;
                        }
                        else
                        {
                            refNode = v11;
                        }

                        v11.PrevNode = v4;

                        v4.UnkNode = v11;
                        v4 = thisNode.PrevNode;

                        if (v4 == null)
                            return;
                    }

                    v4.Dword0 ^= (thisNode.Dword0 ^ v4.Dword0) & 1;

                    if (v4.PrevNode != null)
                        v4.PrevNode.Dword0 |= 1;

                    var v15 = thisNode.PrevNode;

                    thisNode.Dword0 |= 1;
                    thisNode.PrevNode = v15.NextNode;

                    if (v15.NextNode != null)
                        v15.NextNode.UnkNode = thisNode;

                    v15.UnkNode = thisNode.UnkNode;

                    var v17 = thisNode.UnkNode;
                    if (v17 != null)
                    {
                        if (thisNode == v17.NextNode)
                        {
                            v17.NextNode = v15;
                            v15.NextNode = thisNode;
                            thisNode.UnkNode = v15;
                            node = refNode;
                        }
                        else
                        {
                            v17.PrevNode = v15;
                            v15.NextNode = thisNode;
                            thisNode.UnkNode = v15;
                            node = refNode;
                        }
                    }
                    else
                    {
                        refNode = v15;
                        v15.NextNode = thisNode;
                        thisNode.UnkNode = v15;
                        node = refNode;
                    }
                }
                else
                {
                    if (v4 == null)
                        return;

                    if ((v4.Dword0 & 1) == 0)
                    {
                        v4.Dword0 |= 1;

                        var v18 = thisNode.NextNode;

                        thisNode.Dword0 &= -2;
                        thisNode.NextNode = v18.PrevNode;

                        if (v18.PrevNode != null)
                            v18.PrevNode.UnkNode = thisNode;

                        v18.UnkNode = thisNode.UnkNode;

                        if (thisNode.UnkNode != null)
                        {
                            if (thisNode == thisNode.UnkNode.PrevNode)
                                thisNode.UnkNode.PrevNode = v18;
                            else
                                thisNode.UnkNode.NextNode = v18;
                        }
                        else
                        {
                            refNode = v18;
                        }

                        v18.PrevNode = thisNode;

                        thisNode.UnkNode = v18;

                        v4 = thisNode.NextNode;
                        if (v4 == null)
                            return;
                    }

                    var v21 = v4.PrevNode;
                    if (v21 == null || (v21.Dword0 & 1) != 0)
                    {
                        if (v4.NextNode == null || (v4.NextNode.Dword0 & 1) != 0)
                        {
                            v4.Dword0 &= -2;
                            node = thisNode;
                            thisNode = thisNode.UnkNode;
                            continue;
                        }
                    }

                    if (v4.NextNode == null || (v4.NextNode.Dword0 & 1) != 0)
                    {
                        v21.Dword0 |= 1;

                        var v24 = v4.PrevNode;

                        v4.Dword0 &= -2;
                        v4.PrevNode = v24.NextNode;

                        if (v24.NextNode != null)
                            v24.NextNode.UnkNode = v4;

                        v24.UnkNode = v4.UnkNode;

                        if (v4.UnkNode != null)
                        {
                            if (v4 == v4.UnkNode.NextNode)
                                v4.UnkNode.NextNode = v24;
                            else
                                v4.UnkNode.PrevNode = v24;
                        }
                        else
                        {
                            refNode = v24;
                        }

                        v24.NextNode = v4;

                        v4.UnkNode = v24;
                        v4 = thisNode.NextNode;

                        if (v4 == null)
                            return;
                    }

                    v4.Dword0 ^= (thisNode.Dword0 ^ v4.Dword0) & 1;

                    if (v4.NextNode != null)
                        v4.NextNode.Dword0 |= 1;

                    var v28 = thisNode.NextNode;

                    thisNode.Dword0 |= 1;
                    thisNode.NextNode = v28.PrevNode;

                    if (v28.PrevNode != null)
                        v28.PrevNode.UnkNode = thisNode;

                    v28.UnkNode = thisNode.UnkNode;

                    var v30 = thisNode.UnkNode;
                    if (v30 != null)
                    {
                        if (thisNode == v30.PrevNode)
                        {
                            v30.PrevNode = v28;
                            v28.PrevNode = thisNode;
                            thisNode.UnkNode = v28;
                            node = refNode;
                        }
                        else
                        {
                            v30.NextNode = v28;
                            v28.PrevNode = thisNode;
                            thisNode.UnkNode = v28;
                            node = refNode;
                        }
                    }
                    else
                    {
                        refNode = v28;
                        v28.PrevNode = thisNode;
                        thisNode.UnkNode = v28;
                        node = refNode;
                    }
                }
            }

            if (node != null)
                node.Dword0 |= 1;
        }

        public void SubDC7F30(int a2)
        {
            Dword0 ^= (Dword0 ^ ((byte)(Dword0 & 0xF8) - 1)) & 0xF8;
            if ((Dword0 << 24) >> 27 <= 0 && a2 != 0)
            {
                // dealloc this
            }
        }

        public void SubDC7F80(ref PblTreeNode<T> refNode, int a3)
        {
            Dword0 &= -7;

            var node = PrevNode;

            if (NextNode != null && node != null)
            {
                while (node.NextNode != null)
                    node = node.NextNode;

                node.NextNode = NextNode;

                var v6 = node.PrevNode;

                NextNode.UnkNode = node;

                var v7 = node;

                if (node != PrevNode)
                {
                    if (v6 != null)
                        v6.UnkNode = node.UnkNode;

                    node.UnkNode.NextNode = v6;
                    node.PrevNode = PrevNode;
                    PrevNode.UnkNode = node;
                    v7 = node.UnkNode;
                }

                if (this == refNode)
                {
                    refNode = node;
                }
                else
                {
                    if (this == UnkNode.NextNode)
                        UnkNode.NextNode = node;
                    else
                        UnkNode.PrevNode = node;
                }

                node.UnkNode = UnkNode;

                var v10 = node.Dword0 & 1;

                node.Dword0 ^= ((byte)node.Dword0 ^ (byte)Dword0) & 1;
                node.Dword0 ^= ((byte)v10 ^ (byte)Dword0) & 1;

                if (v10 == 1)
                    v7.SubDC7C50(v6, ref refNode);

                SubDC7F30(a3);
            }
            else
            {
                var prev = PrevNode;
                if (NextNode != null)
                    prev = NextNode;

                if (prev != null)
                    prev.UnkNode = UnkNode;

                if (UnkNode != null)
                {
                    if (this == UnkNode.NextNode)
                        UnkNode.NextNode = prev;
                    else
                        UnkNode.PrevNode = prev;
                }
                else
                {
                    refNode = prev;
                }

                if ((Dword0 & 1) != 0)
                    UnkNode.SubDC7C50(prev, ref refNode);

                Dword0 ^= (Dword0 ^ ((byte)(Dword0 & 0xF8) - 1)) & 0xF8;

                if (((Dword0 & 0xFFFFFFF8) << 24) <= 0 && a3 != 0)
                {
                    // dealloc this
                }
            }
        }
    }

    public class PblTree<T>
    {
        public int Count { get; private set; }
        public PblTreeNode<T> RootNode { get; set; }
        public object Lock { get; } = new object();

        public PblTreeNode<T> GetNodeByCrc(uint crc)
        {
            lock (Lock)
            {
                var node = RootNode;

                while (node != null)
                {
                    if (crc > node.Crc)
                        node = node.NextNode;
                    else if (crc < node.Crc)
                        node = node.PrevNode;
                    else
                        break;
                }

                return node;
            }
        }

        public void Remove(PblTreeNode<T> node)
        {
            lock (Lock)
            {
                --Count;
            }
        }
    }
}
