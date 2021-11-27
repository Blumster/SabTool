using System;
using System.Collections.Generic;

namespace SabTool.Data.Cinematics
{
    using Utils;

    [Flags]
    public enum ConversationLineFlags
    {
        Interrupt = 0x01,
        CInt      = 0x02
    }

    public class ConversationLineStructure
    {
        public int Idx { get; set; }
        public Crc Name { get; set; }
        public Crc Speaker { get; set; }
        public Crc Target { get; set; }
        public Dictionary<Crc, Crc> Texts { get; } = new();
        public Dictionary<Crc, Human> Humans { get; } = new();
        public List<ushort> NextLines { get; set; }
        public Crc ConditionCrc { get; set; }
        public string[] Script { get; set; } = new string[2];
        public float Delay { get; set; }
        public short Condition { get; set; }
        public string ConditionText { get; set; }
        public ConversationLineFlags Flags { get; set; }

        public class Human
        {
            public string Tag { get; }
            public Crc Crc1 { get; set; }
            public Crc Crc2 { get; set; }
            public float Float1 { get; set; }
            public float Float2 { get; set; }
            public bool Bool1 { get; set; }

            public Human(string tag)
            {
                Tag = tag;
            }
        }
    }
}
