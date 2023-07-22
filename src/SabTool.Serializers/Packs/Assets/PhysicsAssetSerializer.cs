using SabTool.Data.Packs;
using SabTool.Data.Packs.Assets;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Packs.Assets;
public static class PhysicsAssetSerializer
{
    public static PhysicsAsset DeserializeRaw(StreamBlock.Entry entry)
    {
        if (entry.CompressedSize == entry.UncompressedSize)
            throw new Exception("asd");

        byte[] uncompressedData;

        using (BinaryReader compressedReader = new(new MemoryStream(entry.Payload, false)))
            uncompressedData = compressedReader.ReadDecompressedBytes(entry.CompressedSize);

        using BinaryReader reader = new(new MemoryStream(uncompressedData, false));

        return null;
    }
}
