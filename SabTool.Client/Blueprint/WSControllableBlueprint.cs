using System;
using System.Collections.Generic;
using System.Text;

namespace SabTool.Client.Blueprint
{
    using Container;

    public class WSControllableBlueprint : WSBlueprint, IWSControllableBlueprint
    {
        public uint Unk8 { get; set; }
        public WSDoubleLinkedCircularList<object> UnkC { get; set; }

        public bool SetControllableProperty(uint crc, BluaReader blua)
        {
            return Sub1634800(crc, blua);
        }

        // TODO: read done, data storage isn't
        public bool Sub1634800(uint crc, BluaReader blua)
        {
            uint field;

            if (crc == 0x404D1343U)
            {
                blua.ReadInt(); // Not using the value

                return true;
            }

            if (crc != 0xDB0F705CU)
            {
                if (crc != 0xFB31F1EFU)
                    return false;

                field = blua.ReadUInt();

                if (field != 0x4FF9F863U)
                    Unk8 = field;

                return true;
            }

            var str2 = blua.ReadString(128);

            field = blua.ReadUInt();

            if (field <= 0x3290BA65U)
            {
                if (field == 0x3290BA65U)
                {
                    field = blua.ReadUInt();

                    if (field != 0x4FF9F863U)
                    {
                        // TODO Sub656A10
                    }

                    return true;
                }

                if (field == 0x1B444F31)
                {
                    var b = blua.ReadByte();
                    if (b != 0)
                    {
                        // TODO: Sub656870
                    }

                    return true;
                }

                if (field == 0x2B93BDA9)
                {
                    field = blua.ReadUInt();
                    // TODO: Sub656940

                    return true;
                }
            }

            if (field != 0xB9059DF9)
            {
                if (field != 0xF0676C78)
                {
                    if (field == 0xF5B799C2)
                    {
                        var str1 = blua.ReadString(128);
                        if (str1 != "none" && !string.IsNullOrEmpty(str1))
                        {
                            // TODO: Sub690650
                        }
                    }

                    return true;
                }

                field = blua.ReadUInt();
                // TODO: Sub656940

                return true;
            }

            var cnt = blua.ReadUInt();
            var i = 0;

            while (i < cnt)
            {
                var str1 = blua.ReadString(128);

                ++i;

                // TODO: Sub656D20
            }

            return true;
        }
    }
}
