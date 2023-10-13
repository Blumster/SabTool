using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace SabTool.Serializers.Animations;

using SabTool.Data.Animations;
using SabTool.Serializers.Json.Converters;
using SabTool.Utils;
using SabTool.Utils.Extensions;
using System.Collections.Generic;

public static class AnimationsContainerSerializer
{
    public static AnimationsContainer DeserializeRaw(Stream stream, bool isDlc)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        // TODO: hack to skip PACK header
        if (isDlc)
            stream.Position += 256;

        if (!reader.CheckHeaderString("AP0L", reversed: true))
            throw new Exception("Invalid AP0L header found!");

        var container = new AnimationsContainer
        {
            AnimationPack = AnimationPackSerializer.DeserializeRaw(stream),
            HavokData = AnimationHavokSerializer.DeserializeRaw(stream),
            BlendBoneException = reader.ReadUInt32(),
            IntervalPack = AnimationIntervalPackSerializer.DeserializeRaw(stream),
            SequencePack = AnimationSequencePackSerializer.DeserializeRaw(stream),
            TransitionContainer = AnimationTransitionPackSerializer.DeserializeContainerRaw(stream),
        };

        if (!isDlc)
        {
            container.EdgeData = DeserializeEdgeDataRaw(reader);

            if (!reader.CheckHeaderString("DIST", reversed: true))
                throw new Exception("Invalid DIST header found!");

            for (var i = 0; i < 3; ++i)
            {
                for (var j = 0; j < 4; ++j)
                {
                    container.DistData[i][j][0] = reader.ReadSingle();
                    container.DistData[i][j][1] = reader.ReadSingle();
                }
            }
        }

        container.BankPack = DeserializeBankPackRaw(reader);

        DeserializeAddonRaw(reader, container);
        DeserializeAlphasRaw(reader, container);

        return container;
    }

    private static EdgeData DeserializeEdgeDataRaw(BinaryReader reader)
    {
        if (!reader.CheckHeaderString("EDGE", reversed: true))
            throw new Exception("Invalid EDGE header found!");

        var edgeData = new EdgeData
        {
            TransSequences = reader.ReadConstArray(4680, () => new Crc(reader.ReadUInt32())),
            ClamberSequences = reader.ReadConstArray(350, () => new Crc(reader.ReadUInt32()))
        };

        return edgeData;
    }

    private static AnimationBankPack DeserializeBankPackRaw(BinaryReader reader)
    {
        if (!reader.CheckHeaderString("BANK", reversed: true))
            throw new Exception("Invalid BANK header found!");

        var animationBankPack = new AnimationBankPack();

        var bankCount = reader.ReadUInt32();
        for (var i = 0u; i < bankCount; ++i)
            animationBankPack.Banks.Add(DeserializeBankRaw(reader));

        return animationBankPack;
    }

    private static AnimationBank DeserializeBankRaw(BinaryReader reader)
    {
        var animationBank = new AnimationBank
        {
            NameCrc = new(reader.ReadUInt32()),
            Name = reader.ReadUTF8StringOn(reader.ReadInt16()),
            ParentBank = new(reader.ReadUInt32()),
        };

        var overrideCount = reader.ReadUInt32();
        for (var i = 0u; i < overrideCount; ++i)
            animationBank.Overrides.Add(DeserializeBankOverrideRaw(reader));

        return animationBank;
    }

    private static AnimationBankOverride DeserializeBankOverrideRaw(BinaryReader reader)
    {
        var bankOverride = new AnimationBankOverride
        {
            From = new(reader.ReadUInt32()),
            To = new(reader.ReadUInt32()),
        };

        var unkCount = reader.ReadUInt32();
        for (var i = 0u; i < unkCount; ++i)
            bankOverride.Unks.Add(reader.ReadUInt32());

        return bankOverride;
    }

    private static void DeserializeAddonRaw(BinaryReader reader, AnimationsContainer container)
    {
        if (!reader.CheckHeaderString("ADD1", reversed: true))
            throw new Exception("Invalid ADD1 header found!");

        var unkCount = reader.ReadUInt32();
        for (var i = 0u; i < unkCount; ++i)
        {
            container.Addon1.Add(new AnimationAddon1
            {
                Unk1 = new(reader.ReadUInt32()),
                Unk2 = new(reader.ReadUInt32()),
                Unk3 = reader.ReadUInt32()
            });
        }
    }

    private static void DeserializeAlphasRaw(BinaryReader reader, AnimationsContainer container)
    {
        if (!reader.CheckHeaderString("ALPH", reversed: true))
            throw new Exception("Invalid ALPH header found!");

        var unkCount = reader.ReadUInt32();
        for (var i = 0u; i < unkCount; ++i)
            container.Alphas.Add(new(reader.ReadUInt32()));
    }

    public static void SerializeJSON(AnimationsContainer container, Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(JsonConvert.SerializeObject(container, Formatting.Indented, new CrcConverter()));
    }
}
