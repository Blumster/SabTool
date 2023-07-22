using System.Numerics;
using System.Text;

using Newtonsoft.Json;

using SabTool.Data.Packs;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Packs;
public static class FranceMapSerializer
{
    public static FranceMap DeserializeRaw(Stream stream, FranceMap? franceMap = null)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        bool isDLC = true;

        if (franceMap == null)
        {
            franceMap = new FranceMap();

            isDLC = false;
        }

        if (!reader.CheckHeaderString("MAP6", reversed: true))
            throw new Exception("Invalid Map header found!");

        string name = reader.ReadStringWithMaxLength(256);

        int paletteCount = 0;

        if (isDLC)
        {
            franceMap.FieldDA88 = reader.ReadInt32();

            paletteCount = reader.ReadInt32();
        }
        else
        {
            paletteCount = franceMap.NumStreamBlocks = reader.ReadInt32();
            franceMap.FieldDA84 = reader.ReadInt32();

            #region Garbage
            int count = reader.ReadInt32();

            if (count > 0)
            {
                int unk = reader.ReadInt32();

                for (int i = 0; i < count; ++i)
                {
                    int strLen = reader.ReadInt32();
                    string str = reader.ReadStringWithMaxLength(strLen + 1);
                    //Console.WriteLine($"Str: {str} (0x{Hash.FNV32string(str):X8})");

                    int len2 = reader.ReadInt32();
                    byte[] data2 = reader.ReadBytes(len2);

                    string str2 = Encoding.UTF8.GetString(data2, 0, data2.Length - 1);
                    //Console.WriteLine($"Str2: {str2} (0x{Hash.FNV32string(str2):X8})");

                    byte[] data3 = reader.ReadBytes(48);

                    int len4 = reader.ReadInt32();
                    byte[] data4 = reader.ReadBytes(len4);

                    // TODO
                }
            }
            #endregion

            for (int i = 0; i < 3; ++i)
            {
                franceMap.Extents[i] = new Vector3[2];
                franceMap.Extents[i][0].X = reader.ReadSingle();
                franceMap.Extents[i][0].Y = reader.ReadSingle();
                franceMap.Extents[i][0].Z = reader.ReadSingle();

                franceMap.Extents[i][1].X = reader.ReadSingle();
                franceMap.Extents[i][1].Y = reader.ReadSingle();
                franceMap.Extents[i][1].Z = reader.ReadSingle();
            }

            for (int i = 0; i < 3; ++i)
            {
                franceMap.GridCountX[i] = reader.ReadInt16();
                franceMap.GridCountZ[i] = reader.ReadInt16();

                franceMap.GridCountX[i] = (int)((franceMap.Extents[i][1].X - franceMap.Extents[i][0].X) / FranceMap.GridLimits[i]);
                franceMap.GridCountZ[i] = (int)((franceMap.Extents[i][1].Z - franceMap.Extents[i][0].Z) / FranceMap.GridLimits[i]);
            }
        }

        for (int i = 0; i < paletteCount; ++i)
        {
            PaletteBlock paletteBlock = PaletteBlockSerializer.DeserializeRaw(stream);

            franceMap.Palettes[franceMap.CalculateGrid(paletteBlock)] = paletteBlock;
        }

        int interiorCount = reader.ReadInt32();

        franceMap.Interiors ??= new(interiorCount);

        for (int i = 0; i < interiorCount; ++i)
        {
            StreamBlock block = StreamBlockSerializer.DeserializeFromMapRaw(stream);

            block.Flags = (block.Flags & 0xFFFFE33F) | ((uint)(block.Index & 7) << 10);
            block.Flags = (block.Flags & 0xFFFFFFF9) | 1;
            // TODO: conditionally adding flag 4
            block.Flags &= 0xFFFFFEFF;

            franceMap.Interiors.Add(block.Id, block);
        }

        int cinematicsCount = reader.ReadInt32();

        franceMap.CinematicBlocks ??= new(cinematicsCount);

        for (int i = 0; i < cinematicsCount; ++i)
        {
            StreamBlock block = StreamBlockSerializer.DeserializeFromMapRaw(stream);

            block.FileName = $"France\\{block.FileName}";
            block.Flags = (block.Flags & 0xFFFFE33F) | ((uint)(block.Index & 7) << 10);
            block.Flags = (block.Flags & 0xFFFFFEFA) | 2;

            franceMap.CinematicBlocks.Add(block.Id, block);
        }

        return franceMap;
    }

    public static FranceMap? DeserializeJSON(Stream stream)
    {
        return null;
    }

    public static void SerializeJSON(FranceMap franceMap, Stream stream)
    {
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(franceMap, Formatting.Indented, new CrcConverter()));
    }
}
