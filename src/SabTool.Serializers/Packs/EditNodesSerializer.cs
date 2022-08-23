using System;
using System.IO;
using System.Text;

namespace SabTool.Serializers.Packs;

using SabTool.Data.Packs;
using SabTool.Utils.Extensions;

public static class EditNodesSerializer
{
    public static EditNodes DeserializeRaw(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var nodes = new EditNodes();

        if (!reader.CheckHeaderString("00ED"))
            throw new Exception("Invalid EditNodes header found!");

        var count = reader.ReadInt32();

        for (var i = 0; i < count; ++i)
        {
            var node = new EditNode
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
