using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Packs;

using SabTool.Data.Packs;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils.Extensions;

public static class FranceMapSerializer
{
    public static FranceMap DeserializeRaw(Stream stream, FranceMap? franceMap = null)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var isDLC = true;

        if (franceMap == null)
        {
            franceMap = new FranceMap();

            isDLC = false;
        }

        if (!reader.CheckHeaderString("MAP6", reversed: true))
            throw new Exception("Invalid Map header found!");

        var name = reader.ReadStringWithMaxLength(256);

        var paletteCount = 0;

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
            var count = reader.ReadInt32();

            if (count > 0)
            {
                var unk = reader.ReadInt32();

                for (var i = 0; i < count; ++i)
                {
                    var strLen = reader.ReadInt32();
                    var str = reader.ReadStringWithMaxLength(strLen + 1);
                    //Console.WriteLine($"Str: {str} (0x{Hash.FNV32string(str):X8})");

                    var len2 = reader.ReadInt32();
                    var data2 = reader.ReadBytes(len2);

                    var str2 = Encoding.UTF8.GetString(data2, 0, data2.Length - 1);
                    //Console.WriteLine($"Str2: {str2} (0x{Hash.FNV32string(str2):X8})");

                    var data3 = reader.ReadBytes(48);

                    var len4 = reader.ReadInt32();
                    var data4 = reader.ReadBytes(len4);

                    // TODO
                }
            }
            #endregion

            for (var i = 0; i < 3; ++i)
            {
                franceMap.Extents[i] = new Vector3[2];
                franceMap.Extents[i][0].X = reader.ReadSingle();
                franceMap.Extents[i][0].Y = reader.ReadSingle();
                franceMap.Extents[i][0].Z = reader.ReadSingle();

                franceMap.Extents[i][1].X = reader.ReadSingle();
                franceMap.Extents[i][1].Y = reader.ReadSingle();
                franceMap.Extents[i][1].Z = reader.ReadSingle();
            }

            for (var i = 0; i < 3; ++i)
            {
                franceMap.GridCountX[i] = reader.ReadInt16();
                franceMap.GridCountZ[i] = reader.ReadInt16();

                franceMap.GridCountX[i] = (int)((franceMap.Extents[i][1].X - franceMap.Extents[i][0].X) / FranceMap.GridLimits[i]);
                franceMap.GridCountZ[i] = (int)((franceMap.Extents[i][1].Z - franceMap.Extents[i][0].Z) / FranceMap.GridLimits[i]);
            }
        }

        for (var i = 0; i < paletteCount; ++i)
        {
            var paletteBlock = PaletteBlockSerializer.DeserializeRaw(stream);

            franceMap.Palettes[franceMap.CalculateGrid(paletteBlock)] = paletteBlock;
        }

        var interiorCount = reader.ReadInt32();

        franceMap.Interiors ??= new(interiorCount);

        for (var i = 0; i < interiorCount; ++i)
        {
            var block = StreamBlockSerializer.DeserializeFromMapRaw(stream);

            block.Flags = (block.Flags & 0xFFFFE33F) | ((uint)(block.Index & 7) << 10);
            block.Flags = (block.Flags & 0xFFFFFFF9) | 1;
            // TODO: conditionally adding flag 4
            block.Flags &= 0xFFFFFEFF;

            franceMap.Interiors.Add(block.Id, block);
        }

        var cinematicsCount = reader.ReadInt32();

        franceMap.CinematicBlocks ??= new(cinematicsCount);

        for (var i = 0; i < cinematicsCount; ++i)
        {
            var block = StreamBlockSerializer.DeserializeFromMapRaw(stream);

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
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(franceMap, Formatting.Indented, new CrcConverter()));
    }
}
