using System;
using System.IO;

namespace SabTool.Data.Packs.Assets;

using Utils;

public class TextureAsset
{
    public Crc NameCrc { get; }
    public string Name { get; set; }

    public byte[][] DDSFiles { get; set; }

    public TextureAsset(Crc nameCrc)
    {
        NameCrc = nameCrc;
    }

    public void Import(string filePath)
    {
        throw new NotImplementedException();
    }

    public void Export(string outputPath)
    {
        if (DDSFiles == null)
            return;

        if (DDSFiles.Length == 1)
        {
            var outputFilePath = Path.Combine(outputPath, $"{Name}.dds");

            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

            File.WriteAllBytes(outputFilePath, DDSFiles[0]);

            return;
        }

        for (var i = 0; i < DDSFiles.Length; ++i)
        {
            if (DDSFiles[i] == null)
                continue;

            var localName = $"{Name}{i}.dds";
            var outputFilePath = Path.Combine(outputPath, localName);

            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

            File.WriteAllBytes(outputFilePath, DDSFiles[i]);
        }
    }
}
