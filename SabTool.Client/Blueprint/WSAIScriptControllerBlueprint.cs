using System;
using System.Collections.Generic;
using System.Text;

namespace SabTool.Client.Blueprint
{
    using Container;

    public class CustomWSEntityControllableBlueprint : WSEntityBlueprint, IWSControllableBlueprint
    {
        private WSControllableBlueprint _base = new WSControllableBlueprint();

        public uint Unk8
        {
            get
            {
                return _base.Unk8;
            }
            set
            {
                _base.Unk8 = value;
            }
        }

        public WSDoubleLinkedCircularList<object> UnkC
        {
            get
            {
                return _base.UnkC;
            }
            set
            {
                _base.UnkC = value;
            }
        }

        public bool SetControllableProperty(uint crc, BluaReader blua)
        {
            return _base.SetControllableProperty(crc, blua);
        }
    }

    public class WSAIScriptControllerBlueprint : CustomWSEntityControllableBlueprint
    {
        public string Unkstr40 { get; set; }
        public int UnkC0 { get; set; }
        public float UnkC4 { get; set; }
        public float UnkC8 { get; set; }
        public int[] UnkCC { get; set; } = new int[9];
        public int UnkF0 { get; set; }
        public int UnkF4 { get; set; }
        public int UnkF8 { get; set; }
        public int UnkFC { get; set; }
        public int Unk100 { get; set; }
        public int Unk104 { get; set; }
        public int Unk108 { get; set; }
        public int Unk10C { get; set; }

        // Overrides
        public override object Sub4527F0()
        {
            return Sub4A2680();
        }

        public override IWSControllableBlueprint GetControllableBlueprint()
        {
            return Sub4A26B0();
        }

        public override void Sub452900()
        {
            Sub4A26C0();
        }

        public override bool SetProperty(uint crc, BluaReader blua)
        {
            // In sub function: Sub4A26D0
            switch (crc)
            {
                case 0x0424DD32:
                    _ = blua.ReadByte();
                    return true;

                case 0x07B13063:
                    _ = blua.ReadInt();
                    return true;

                case 0x165C1FD7:
                    _ = blua.ReadInt();
                    return true;

                case 0x19BAB4C9:
                    _ = blua.ReadFloat();
                    return true;

                case 0x2F1820CC:
                    _ = blua.ReadFloat();
                    return true;

                case 0x3570F09D:
                    _ = blua.ReadInt();
                    return true;

                case 0x37FC008D:
                    _ = blua.ReadFloat();
                    return true;

                case 0x4177A478:
                    _ = blua.ReadInt();
                    return true;

                case 0x44885A78:
                    _ = blua.ReadBool();
                    return true;

                case 0x6A9AB174:
                    _ = blua.ReadFloat();
                    return true;

                case 0x7154DC63:
                    _ = blua.ReadFloat();
                    return true;

                case 0x722C0953:
                    _ = blua.ReadInt();
                    return true;

                case 0x77776D5A:
                    _ = blua.ReadInt();
                    return true;

                case 0x7A2AA5EF:
                    _ = blua.ReadFloat();
                    return true;

                case 0x85B5E29F:
                    _ = blua.ReadString(32);
                    return true;

                case 0x89A84EF5: // 4 bytes
                    _ = blua.ReadInt();
                    return true;

                case 0x97EE12C6: // 1 byte
                    _ = blua.ReadByte();
                    return true;

                case 0xA3DBF09D: // 1 byte
                    _ = blua.ReadByte();
                    return true;

                case 0xA77EDAAA: // 1 byte
                    _ = blua.ReadByte();
                    return true;

                case 0xAE350719:
                    _ = blua.ReadFloat();
                    return true;

                case 0xBCFE6314:
                    _ = blua.ReadString(128);
                    return true;

                case 0xBE5952B1:
                    _ = blua.ReadFloat();
                    return true;

                case 0xBF930289: // 4 bytes
                    _ = blua.ReadInt();
                    return true;

                case 0xC072E94A:
                    _ = blua.ReadFloat();
                    return true;

                case 0xC4C412CD:
                    _ = blua.ReadFloat();
                    return true;

                case 0xCA6B9057: // 4 bytes
                    _ = blua.ReadInt();
                    return true;

                case 0xCACFD6AA:
                    _ = blua.ReadInt();
                    return true;

                case 0xCE629E7E:
                    _ = blua.ReadInt();
                    return true;

                case 0xD6F4D903:
                    _ = blua.ReadFloat();
                    return true;

                case 0xD72AA401:
                    _ = blua.ReadInt();
                    return true;

                case 0xD730D0E1:
                    _ = blua.ReadFloat();
                    return true;

                case 0xD79C2BE9: // PblCRC
                    _ = blua.ReadInt();
                    return true;

                case 0xDDF1EE0F:
                    _ = blua.ReadFloat();
                    return true;

                case 0xE0B01D9F:
                    Unkstr40 = blua.ReadString(128);
                    return true;

                case 0xE4E34549: // 4 bytes
                    _ = blua.ReadInt();
                    return true;

                case 0xF4BADFDB:
                    _ = blua.ReadFloat();
                    return true;

                case 0xF6B4FF0E:
                    _ = blua.ReadInt();
                    return true;

                case 0xFE8BAE82: // 4 bytes
                    _ = blua.ReadInt();
                    return true;

                default:
                    return false;
            }
        }

        // Own functions
        public object Sub4A2680()
        {
            return null; // TODO: maybe entity base? maybe the whole object?
        }

        public void Sub4A2690()
        {

        }

        public IWSControllableBlueprint Sub4A26B0()
        {
            return this;
        }

        public void Sub4A26C0()
        {

        }
    }
}
