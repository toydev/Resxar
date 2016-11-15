using Mono.Options;

namespace Resxar
{
    public interface IResourceArchiver
    {
        void AddOptionSet(OptionSet options);
        bool ValidateOptions();
        bool IsTarget(string path);
        void Archive(string targetPath, string outputDirectory);
    }
}
