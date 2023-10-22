using System.IO;
using System.Text;

namespace SabTool.Serializers.Misc;

using SabTool.Data.Misc;

public class BinkPOVSerializer
{
    public static BinkPOVFile DeserializeRaw(Stream stream)
    {
        using BinaryReader reader = new(stream, Encoding.ASCII, true);

        var povFile = new BinkPOVFile
        {
            Data = reader.ReadBytes(128)
        };

        //if (stream.Position != stream.Length)
        //    throw new System.Exception($"Not reading POV properly!");

        return povFile;
    }
}
