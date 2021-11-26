namespace SabTool.Data.Cinematics.CinematicElements
{
    using Utils;

    public class CinemaAnimation : CinemaElement
    {
        public float UnkFloat1 { get; set; }
        public float UnkFloat2 { get; set; }
        public float UnkFloat3 { get; set; }
        public string Name { get; set; }
        public Crc UnkCrc1 { get; set; }
        public Crc UnkCrc2 { get; set; }
        public byte Flags { get; set; }
    }
}
