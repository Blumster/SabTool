using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabTool.Serializers.Packs.Assets;

using SabTool.Data.Packs;
using SabTool.Data.Packs.Assets;
using SabTool.Utils;
using SabTool.Utils.Extensions;

public static class PhysicsAssetSerializer
{
    public static PhysicsAsset DeserializeRaw(StreamBlock.Entry entry)
    {
        if (entry.CompressedSize == entry.UncompressedSize)
            throw new Exception("asd");

        byte[] uncompressedData;

        using (var compressedReader = new BinaryReader(new MemoryStream(entry.Payload, false)))
            uncompressedData = compressedReader.ReadDecompressedBytes(entry.CompressedSize);

        using var reader = new BinaryReader(new MemoryStream(uncompressedData, false));

        return null;
    }
}
