using System.Text;

using SabTool.Utils.Extensions;

namespace SabTool.Utils.Images;
public class DDSImage
{
    public class Header
    {
        public struct PixelFormat
        {
            public const int PixelSize = 32;

            public uint Flags { get; set; }
            public uint FourCC { get; set; }
            public uint RGBBitCount { get; set; }
            public uint RBitMask { get; set; }
            public uint GBitMask { get; set; }
            public uint BBitMask { get; set; }
            public uint ABitMask { get; set; }

            public void Read(BinaryReader reader)
            {
                uint pixelFormatSize = reader.ReadUInt32();
                if (pixelFormatSize != PixelSize)
                    throw new Exception("Invalid Pixel Format size!");

                Flags = reader.ReadUInt32();
                FourCC = reader.ReadUInt32();
                RGBBitCount = reader.ReadUInt32();
                RBitMask = reader.ReadUInt32();
                GBitMask = reader.ReadUInt32();
                BBitMask = reader.ReadUInt32();
                ABitMask = reader.ReadUInt32();
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(PixelSize);
                writer.Write(Flags);
                writer.Write(FourCC);
                writer.Write(RGBBitCount);
                writer.Write(RBitMask);
                writer.Write(GBitMask);
                writer.Write(BBitMask);
                writer.Write(ABitMask);
            }
        }

        public const uint Magic = 0x20534444;
        public const int Size = 124;

        public void Read(BinaryReader reader)
        {
            if (reader.ReadUInt32() != Magic)
                throw new Exception("Invalid DDS header magic!");

            long startOffset = reader.BaseStream.Position;

            uint size = reader.ReadUInt32();
            if (size != Size)
                throw new Exception("Invalid header size!");

            Flags = reader.ReadUInt32();
            Height = reader.ReadUInt32();
            Width = reader.ReadUInt32();
            PitchOrLinearSize = reader.ReadUInt32();
            Depth = reader.ReadUInt32();
            MipMapCount = reader.ReadUInt32();
            Reserved = reader.ReadConstArray(11, reader.ReadUInt32);

            PixelFormatData.Read(reader);

            SurfaceFlags = reader.ReadUInt32();
            CubemapFlags = reader.ReadUInt32();
            Caps3 = reader.ReadUInt32();
            Caps4 = reader.ReadUInt32();
            Reserved2 = reader.ReadUInt32();

            if (reader.BaseStream.Position - startOffset != Size)
                throw new Exception("Under or over read of the DDS header!");
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Magic);

            long startOffset = writer.BaseStream.Position;

            writer.Write(Size);
            writer.Write(Flags);
            writer.Write(Height);
            writer.Write(Width);
            writer.Write(Depth);
            writer.Write(MipMapCount);
            writer.WriteConstArray(Reserved, 11, writer.Write);

            PixelFormatData.Write(writer);

            writer.Write(SurfaceFlags);
            writer.Write(CubemapFlags);
            writer.Write(Caps3);
            writer.Write(Caps4);
            writer.Write(Reserved2);

            if (writer.BaseStream.Position - startOffset != Size)
                throw new Exception("Under or over write of the DDS header!");
        }

        public uint Flags { get; set; }
        public uint Height { get; set; }
        public uint Width { get; set; }
        public uint PitchOrLinearSize { get; set; }
        public uint Depth { get; set; }
        public uint MipMapCount { get; set; }
        public uint[] Reserved { get; set; }
        public PixelFormat PixelFormatData { get; }
        public uint SurfaceFlags { get; set; }
        public uint CubemapFlags { get; set; }
        public uint Caps3 { get; set; }
        public uint Caps4 { get; set; }
        public uint Reserved2 { get; set; }
    }

    public static DDSImage ReadFrom(Stream stream)
    {
        DDSImage image = new();

        image.Read(stream);

        return image;
    }

    public void Read(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        Read(reader);
    }

    public void Read(BinaryReader reader)
    {
        // TODO
    }

    public void Write(Stream stream)
    {
        using BinaryWriter writer = new(stream, Encoding.UTF8, true);

        Write(writer);
    }

    public void Write(BinaryWriter writer)
    {
        // TODO
    }
}
