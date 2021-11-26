namespace SabTool.Data.Cinematics.CinematicElements
{
    using Utils;

    public class CinemaObjectMotionBlur : CinemaElement
    {
        public float EndTime { get; set; }
        public Crc UnkCrc { get; set; }
        public byte Flags { get; set; }
    }
}
