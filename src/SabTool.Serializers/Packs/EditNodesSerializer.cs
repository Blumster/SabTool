using System.Text;

using SabTool.Data.Packs;
using SabTool.Utils.Extensions;

namespace SabTool.Serializers.Packs;
public static class EditNodesSerializer
{
    public static EditNodes DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.UTF8, true);

        EditNodes nodes = new();

        if (!reader.CheckHeaderString("00ED"))
            throw new Exception("Invalid EditNodes header found!");

        int count = reader.ReadInt32();

        for (int i = 0; i < count; ++i)
        {
            EditNode node = new()
            {
                Name = new(reader.ReadUInt32()),
                Length = reader.ReadInt32(),
                Offset = reader.ReadInt32()
            };

            node.Data = reader.ReadWithPosition(node.Offset, () => reader.ReadBytes(node.Length));

            nodes.Nodes.Add(node);
        }

        return nodes;
    }
}
