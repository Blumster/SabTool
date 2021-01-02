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

        public bool Sub6900D0(uint crc, BluaStruct blua)
        {
            return _base.Sub6900D0(crc, blua);
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

        public override IWSControllableBlueprint Sub452840()
        {
            return Sub4A26B0();
        }

        public override void Sub452900()
        {
            Sub4A26C0();
        }

        public override bool Sub450FD0(uint crc, BluaStruct blua)
        {
            return Sub4A26D0(crc, blua);
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

        public bool Sub4A26D0(uint crc, BluaStruct blua)
        {
            if (crc == 0x19BAB4C9)
            {
                var v29 = blua.ReadInt(); // most likely read into a field
                return true;
            }

            if (crc == 0x424DD32)
            {
                var v10 = blua.ReadByte(); // stored into a field

                return true;
            }

            if (crc == 0xE0B01D9F)
            {
                Unkstr40 = blua.ReadString(128);

                return true;
            }

            return false;
        }
    }
}
