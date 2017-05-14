using System.Collections.Generic;

using Mono.Options;

namespace Resxar
{
    public class ResourceArchiverManager
    {
        private IList<IResourceArchiver> Archivers { get; set; } = new List<IResourceArchiver>();

        public void Add(IResourceArchiver archiver)
        {
            Archivers.Add(archiver);
        }

        public void AddOptionSet(OptionSet options)
        {
            foreach (IResourceArchiver archiver in Archivers)
            {
                archiver.AddOptionSet(options);
            }
        }

        public void ArchiveResx(string targetPath, string outputDirectory)
        {
            foreach (IResourceArchiver archiver in Archivers)
            {
                if (archiver.IsTarget(targetPath))
                {
                    archiver.Archive(targetPath, outputDirectory);
                    return;
                }
            }
        }
    }
}
