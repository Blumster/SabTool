using System.IO;

namespace SabTool.Data.LooseFiles
{
    public class LooseFileEntry
    {
        public uint Crc { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }

        public LooseFileEntry(uint crc, string fileName, string filePath)
        {
            Crc = crc;
            Name = fileName;
            Data = File.ReadAllBytes(filePath);
        }

        public LooseFileEntry(uint crc, string fileName, byte[] data)
        {
            Crc = crc;
            Name = fileName;
            Data = data;
        }

        public override string ToString()
        {
            return $"LooseFileEntry(Crc: 0x{Crc:X8}, Name: \"{Name}\", Size: {Data.Length})";
        }
    }
}
