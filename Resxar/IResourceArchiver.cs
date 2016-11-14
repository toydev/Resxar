namespace Resxar
{
    public interface IResourceArchiver
    {
        bool IsTarget(string path);
        void Archive(string targetPath, string outputDirectory);
    }
}
