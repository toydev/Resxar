using System;
using System.Collections.Generic;
using System.IO;

using Mono.Options;

namespace Resxar
{
    class Program
    {
        static void Main(string[] args)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();

            OptionSet options = new OptionSet()
            {
                { "i|in=", "", v => parameters["in"] = v },
                { "o|out=", "", v => parameters["out"] = v },
            };

            IList<string> extra;
            try
            {
                extra = options.Parse(args);

                Run(parameters);
            }
            catch (OptionException e)
            {
                // output some error message
                Console.Write("greet: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `greet --help' for more information.");
            }
            catch (Exception e)
            {
                Console.Error.Write(e);
            }
        }

        private static IList<IResourceArchiver> Archivers { get; set; } = new List<IResourceArchiver>();

        static void Run(IDictionary<string, string> parameters)
        {
            string inputDirectory = parameters["in"];
            string outputDirectory = parameters["out"];

            Archivers.Add(new TextResourceArchiver());
            Archivers.Add(new DirectoryResourceArchiver());

            foreach (string targetFilename in Directory.GetFiles(inputDirectory))
            {
                ArchiveResx(targetFilename, outputDirectory);
            }

            foreach (string targetDirectory in Directory.GetDirectories(inputDirectory))
            {
                ArchiveResx(targetDirectory, outputDirectory);
            }
        }

        static void ArchiveResx(string targetPath, string outputDirectory)
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
