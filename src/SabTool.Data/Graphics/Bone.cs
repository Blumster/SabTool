namespace SabTool.Data.Graphics
{
    using Utils;

    public class Bone
    {
        public Crc UnkNamePtr { get; set; }
        public byte UnkByte { get; set; }
        public Crc Crc { get; set; }
        public short Flags { get; set; }
        public short Index { get; set; }
        public byte UnkFlags { get; set; }
        public float Field20 { get; set; }
        public float Field24 { get; set; }
        public float Field28 { get; set; }
        public int Field2C { get; set; }
        public float Field30 { get; set; }
        public float Field34 { get; set; }
        public float Field38 { get; set; }
        public float Field3C { get; set; }
    }
}
