using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Serializers.Misc;

using SabTool.Data.Misc;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class Hei5Serializer
{
    public static Hei5Container DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        if (!reader.CheckHeaderString("HEI5", reversed: true))
            throw new Exception("Invalid HEI5 header found!");

        var hei5Container = new Hei5Container();

        var count = reader.ReadInt32();
        hei5Container.Unk40 = reader.ReadInt32();
        hei5Container.Unk44 = reader.ReadInt32();
        hei5Container.Unk48 = reader.ReadSingle();
        hei5Container.Unk4C = reader.ReadSingle();
        hei5Container.Unk50 = reader.ReadSingle();
        hei5Container.Unk54 = reader.ReadSingle();

        for (var i = 0; i < count; ++i)
        {
            if (!reader.CheckHeaderString("HEI1", reversed: true))
                throw new Exception("Invalid HEI1 header found!");

            var hei1 = new Hei1();
            hei1.Unk32 = reader.ReadInt32();
            hei1.Unk36 = reader.ReadInt32();
            hei1.Unk40 = reader.ReadSingle();
            hei1.Unk44 = reader.ReadSingle();
            hei1.Unk44 = (hei1.Unk44 - hei1.Unk40) / 255.0f;
            hei1.Unk48 = reader.ReadBytes(hei1.Unk32 * hei1.Unk36);
            hei1.Unk0 = reader.ReadInt32();
            hei1.Unk4 = reader.ReadInt32();
            hei1.Unk8 = reader.ReadSingle();
            hei1.Unk16 = reader.ReadSingle();
            hei1.Unk20 = reader.ReadSingle();
            hei1.Unk24 = reader.ReadSingle();

            hei5Container.Hei1s.Add(hei1);
        }

        if (stream.Position != stream.Length)
            throw new Exception("Under reading HEI5 file!");

        return hei5Container;
    }

    public static void SerializeRaw(Hei5Container hei5Container, Stream stream)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
    }
}
