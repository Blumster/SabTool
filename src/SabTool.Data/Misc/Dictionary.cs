using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SabTool.Data.Misc;

using SabTool.Utils;

public class Dictionary
{
    public Crc Name { get; set; }
    public List<DictionaryObject> Objects { get; } = new();
}

public class DictionaryObject
{
    public Crc Name { get; set; }
    public List<Property> Properties { get; } = new();
}

public static class DictionaryPropertyTypes
{
    // this is a hack, remove it...
    public static Dictionary<Crc, Type> Types { get; } = new()
    {
        { new(0x5D78EDE2), typeof(string) }, // ClassName
        { new(0x0012BDC3), typeof(string) }, // BaseName
        { new(0xDBAA9984), typeof(string) }, // Traffic
        { new(0x28E10525), typeof(string) }, // Type
        { new(0x79450675), typeof(string) }, // Parent
        { new(0x05D7FC60), typeof(Vector3) }, // Position
        { new(0x7F451191), typeof(Vector3) }, // Position1
        { new(0xCB289CD9), typeof(Vector3) }, // Tangent1
        { new(0xFBB4BDA8), typeof(Vector3) }, // XAxis
        { new(0xD0C26E7D), typeof(Vector3) }, // YAxis
        { new(0x4FB76002), typeof(Vector3) }, // ZAxis
        { new(0xCBE8ED58), typeof(float) }, // Distance
        { new(0x99327D6F), typeof(float) }, // Width
        { new(0xC4197FF4), typeof(int) }, // LaneCount
        { new(0xA09A4DFD), typeof(int) }, // Reverse
        { new(0xFB8065AA), typeof(int) }, // AttachedCount
        { new(0x28AACCAE), typeof(Crc) }, // ID0
        { new(0x8AADA58B), typeof(Crc) }, // ID1
        { new(0xB0B01FF4), typeof(Crc) }, // ID2
        { new(0x8AB222B9), typeof(Crc) }, // ID3
        { new(0xD25F1637), typeof(Crc) }, // PrevID0
        { new(0xF05D06DA), typeof(Crc) }, // PrevID1
        { new(0x52645CE5), typeof(Crc) }, // PrevID2
        { new(0xE8617770), typeof(Crc) }, // PrevID3
        { new(0x7F63924F), typeof(Crc) }, // NextID0
        { new(0x5D611E32), typeof(Crc) }, // NextID1
        { new(0xDF68A69D), typeof(Crc) }, // NextID2
        { new(0xF5668AA8), typeof(Crc) }, // NextID3
        { new(0xC3B329FE), typeof(Crc) }, // AttachedID0
        { new(0xE5B59E1B), typeof(Crc) }, // AttachedID1
        { new(0x4BB87D44), typeof(Crc) }, // AttachedID2
        { new(0x65BAE4C9), typeof(Crc) }, // AttachedID3
        { new(0x404D1343), typeof(Crc) }, // LuaTable
        { new(0x8CDA47A7), typeof(bool) }, // BubbleProof
        { new(0x42498680), typeof(bool) }, // Script

        
        { new(0xB100447A), typeof(int) },
        { new(0xA1F61387), typeof(int) }, // Version
        { new(0x381F4A7E), typeof(Crc) }, // Blueprint
        { new(0x6F28D499), typeof(int) },
        { new(0xF4E58AAE), typeof(Vector3) },
        { new(0xBBBDA49B), typeof(Vector3) },
        { new(0xE42680F4), typeof(Vector3) },
        { new(0xF0ED6EBD), typeof(Vector3) },
    };

    public static object? ConvertData(Crc crc, byte[] data)
    {
        static void CheckDataSize(byte[] data, int expectedSize)
        {
            if (data == null || data.Length != expectedSize)
                throw new Exception("");
        }

        if (!Types.ContainsKey(crc))
        {
            Console.Error.WriteLine($"Missing crc: {crc} in DictionaryPropertyTypes.ConvertData!");
            return null;
        }

        switch (Types[crc].Name)
        {
            case nameof(Boolean):
                CheckDataSize(data, 1);

                return data[0] != 0;

            case nameof(Int32):
                CheckDataSize(data, 4);

                return BitConverter.ToInt32(data, 0);

            case nameof(Single):
                CheckDataSize(data, 4);

                return BitConverter.ToSingle(data, 0);

            case nameof(String):
                return Encoding.UTF8.GetString(data, 0, data.Length - 1);

            case nameof(Vector3):
                CheckDataSize(data, 12);

                return new Vector3(BitConverter.ToSingle(data, 0), BitConverter.ToSingle(data, 4), BitConverter.ToSingle(data, 8));

            case nameof(Crc):
                CheckDataSize(data, 4);

                return new Crc(BitConverter.ToUInt32(data, 0));

            default:
                throw new Exception($"Unhandled type {Types[crc].Name} in DictionaryPropertyTypes.ConvertData!");
        }
    }
}
