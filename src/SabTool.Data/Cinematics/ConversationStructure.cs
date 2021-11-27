using System;
using System.Collections.Generic;

namespace SabTool.Data.Cinematics
{
    using Utils;

    [Flags]
    public enum ConversationFlags
    {
        Subtitle   = 0x01,
        Sound3D    = 0x02,
        RandomLine = 0x04,
        Journal    = 0x08,
        Car        = 0x10,
        Unk20      = 0x20,
    }

    public class ConversationStructure
    {
        public Crc Name { get; set; }
        public Crc Camera { get; set; }
        public Crc File { get; set; }
        public int SoundLocation { get; set; }
        public ConversationLineStructure[] Lines { get; set; }
        public List<ushort> NextLines { get; set; }
        public Dictionary<Crc, Human> Humans { get; } = new();
        public float DistanceSqr { get; set; }
        public float SoundDistance { get; set; }
        public string[] Script { get; } = new string[2];
        public string SoundBank { get; set; }
        public ConversationFlags Flags { get; set; }

        public class Human
        {
            public string Tag { get; }
            public Crc Crc1 { get; set; }
            public Crc Crc2 { get; set; }
            public float Float1 { get; set; }
            public bool Bool1 { get; set; }

            public Human(string tag)
            {
                Tag = tag;
            }
        }
    }
}
