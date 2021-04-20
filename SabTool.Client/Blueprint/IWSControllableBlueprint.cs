using System;
using System.Collections.Generic;
using System.Text;

namespace SabTool.Client.Blueprint
{
    using Container;

    public interface IWSControllableBlueprint
    {
        uint Unk8 { get; set; }
        WSDoubleLinkedCircularList<object> UnkC { get; set; }

        bool SetControllableProperty(uint crc, BluaReader blua);
    }
}
