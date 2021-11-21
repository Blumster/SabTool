namespace SabTool.Depot
{
    using Data;
    using Serializers;

    public partial class ResourceDepot
    {
        private const string LooseFilesFileName = @"France\loosefiles_BinPC.pack";
        private ICollection<LooseFile>? LooseFiles { get; set; }
        private FileStream? LooseFilesFileStream { get; set; }

        private bool LoadLooseFiles()
        {
            try
            {
                Console.WriteLine($"Loading LooseFiles from {LooseFilesFileName}...");

                LooseFilesFileStream = new FileStream(GetGameFilePath(LooseFilesFileName), FileMode.Open, FileAccess.Read, FileShare.Read);

                LooseFiles = LooseFileSerializer.DeserializeRaw(LooseFilesFileStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while loading LooseFiles: {ex}");
                return false;
            }

            LoadedResources |= Resource.LooseFiles;

            Console.WriteLine("LooseFiles loaded!");
            return true;
        }

        public MemoryStream? GetLooseFile(string name)
        {
            var entry = LooseFiles?.FirstOrDefault(file => file.Name == name);
            if (entry == null || LooseFilesFileStream == null)
                return null;

            var memoryStream = new MemoryStream(entry.Size);
            memoryStream.SetLength(entry.Size);

            lock (LooseFilesFileStream)
            {
                LooseFilesFileStream.Position = entry.DataOffset;

                if (LooseFilesFileStream.Read(memoryStream.GetBuffer(), 0, entry.Size) != entry.Size)
                    return null;
            }

            return memoryStream;
        }

        public IEnumerable<LooseFile>? GetLooseFiles()
        {
            return LooseFiles;
        }
    }
}
