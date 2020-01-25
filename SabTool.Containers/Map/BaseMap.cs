using System;
using System.Collections.Generic;
using System.IO;

namespace SabTool.Containers.Map
{
    using Utils.Extensions;

    public class BaseMap
    {
        public void LoadBlocksFromMapFile(BinaryReader br, int unkCount, string mapFileNameWithoutExtension, bool unk)
        {
            if (unkCount <= 0)
                return;

            // gibberish?

            while (true)
            {
                var unkInt = br.ReadUInt32();

                var strLen = br.ReadUInt16();
                var str = br.ReadStringWithMaxLength(strLen);
            }
        }
    }
}
