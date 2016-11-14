using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;

namespace Resxar
{
    public class DirectoryResourceArchiver : IResourceArchiver
    {
        private Encoding m_encoding = Encoding.UTF8;
        private bool m_useBitmap = false;

        public bool IsTarget(string path)
        {
            return Directory.Exists(path);
        }

        public string OutputFilepath(string targetPath, string outputDirectory)
        {
            return Path.Combine(
                outputDirectory,
                Path.GetFileNameWithoutExtension(targetPath) + ".resx");
        }

        public void Archive(string targetPath, string outputDirectory)
        {
            string outputFilepath = OutputFilepath(targetPath, outputDirectory);
            IList<string> targetFilepaths = GetTargetFilepaths(targetPath);
            if (CheckTimestamp(targetFilepaths, outputFilepath))
            {
                Uri rootDirectory = new Uri(Path.GetFullPath(targetPath) + "/");
                using (ResXResourceWriter writer = new ResXResourceWriter(
                    new FileStream(outputFilepath, FileMode.Create)))
                {
                    foreach (string targetFilename in targetFilepaths)
                    {
                        Uri relativeUrl = rootDirectory.MakeRelativeUri(new Uri(targetFilename));
                        AddResource(writer, targetFilename, relativeUrl.ToString());
                    }
                }
            }
        }

        public bool CheckTimestamp(IList<string> targetFilepaths, string outputFilepath)
        {
            if (!File.Exists(outputFilepath))
            {
                return true;
            }

            DateTime destinationFileTimestamp = File.GetLastWriteTime(outputFilepath);

            foreach (string targetFilepath in targetFilepaths)
            {
                DateTime targetFileTimestamp = File.GetLastWriteTime(targetFilepath);
                if (destinationFileTimestamp < targetFileTimestamp)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddResource(ResXResourceWriter writer, string resourceFullPath, string resourceRelativePath)
        {
            string extension = Path.GetExtension(resourceRelativePath).Replace(".", "");

            string resourceName = GetResourceName(
                extension,
                Path.GetDirectoryName(resourceRelativePath),
                Path.GetFileNameWithoutExtension(resourceRelativePath)
                );

            if (Either(extension, "txt"))
            {
                string resource = GetTextFromFile(resourceFullPath);
                writer.AddResource(resourceName, resource);
            }
            else if (Either(extension, "png", "bmp", "jpg", "jpeg", "gif", "tif", "tiff"))
            {
                if (m_useBitmap)
                {
                    Bitmap resource = new Bitmap(resourceFullPath);
                    writer.AddResource(resourceName, resource);
                }
                else
                {
                    byte[] resource = GetBytesFromFile(resourceFullPath);
                    writer.AddResource(resourceName, resource);
                }
            }
            else
            {
                byte[] resource = GetBytesFromFile(resourceFullPath);
                writer.AddResource(resourceName, resource);
            }
        }

        private string GetTextFromFile(string filename)
        {
            return m_encoding.GetString(GetBytesFromFile(filename));
        }

        private static string GetResourceName(string extension, string path, string filenameWithoutExtension)
        {
            StringBuilder builder = new StringBuilder();
            if (0 < extension.Length)
            {
                builder.Append(extension);
                builder.Append("_");
            }

            if (0 < path.Length)
            {
                builder.Append(path);
                builder.Append("_");
            }

            builder.Append(filenameWithoutExtension);

            string result = Regex.Replace(builder.ToString(), @"[/\\.]", "_");

            if (Regex.IsMatch(result, "^[0-9]"))
            {
                return "_" + result;
            }
            else
            {
                return result;
            }
        }

        private static bool Either(string target, params string[] compares)
        {
            foreach (string compare in compares)
            {
                if (target == compare)
                {
                    return true;
                }
            }
            return false;
        }

        private static byte[] GetBytesFromFile(string filename)
        {
            List<byte> result = new List<byte>();
            using (Stream stream = new FileStream(filename, FileMode.Open))
            {
                int bufferSize = 1024;
                byte[] buffer = new byte[1024];
                int offset = 0;
                while (true)
                {
                    int length = stream.Read(buffer, offset, bufferSize);
                    if (length == 0)
                    {
                        break;
                    }
                    else
                    {
                        if (length == bufferSize)
                        {
                            result.AddRange(buffer);
                        }
                        else
                        {
                            for (int i = 0; i < length; ++i)
                            {
                                result.Add(buffer[i]);
                            }
                        }
                    }
                }
            }
            return result.ToArray();
        }

        private IList<string> GetTargetFilepaths(string inputDirectory)
        {
            IList<string> result = new List<string>();

            Stack<string> targetDirectories = new Stack<string>();
            targetDirectories.Push(Path.GetFullPath(inputDirectory));

            while (0 < targetDirectories.Count)
            {
                string targetDirectory = targetDirectories.Pop();

                foreach (string targetFilepath in Directory.GetFiles(targetDirectory))
                {
                    result.Add(targetFilepath);
                }

                foreach (string subdirectory in Directory.GetDirectories(targetDirectory))
                {
                    targetDirectories.Push(subdirectory);
                }
            }

            return result;
        }
    }
}
