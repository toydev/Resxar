using System;
using System.Collections.Generic;
using System.IO;

using Mono.Options;

namespace Resxar
{
    class Program
    {
        private static ResourceArchiverManager ResourceArchiverManager { get; set; }
            = new ResourceArchiverManager();

        static void Main(string[] args)
        {
            ResourceArchiverManager.Add(new TextResourceArchiver());
            ResourceArchiverManager.Add(new DirectoryResourceArchiver());

            string inputDirectory = null;
            string outputDirectory = null;

            OptionSet options = new OptionSet()
            {
                { "i|in=", "", v => inputDirectory = v },
                { "o|out=", "", v => outputDirectory = v },
            };

            ResourceArchiverManager.AddOptionSet(options);

            IList<string> extra;
            try
            {
                extra = options.Parse(args);

                if (0 < extra.Count
                    || inputDirectory == null
                    || outputDirectory == null
                    || !ResourceArchiverManager.ValidateOptions())
                {
                    ShowUsage();
                    return;
                }

                Run(inputDirectory, outputDirectory);
            }
            catch (OptionException)
            {
                ShowUsage();
            }
            catch (Exception e)
            {
                Console.Error.Write(e);
                ShowUsage();
            }
        }

        static void Run(string inputDirectory, string outputDirectory)
        {
            foreach (string targetFile in Directory.GetFiles(inputDirectory))
            {
                ResourceArchiverManager.ArchiveResx(targetFile, outputDirectory);
            }

            foreach (string targetDirectory in Directory.GetDirectories(inputDirectory))
            {
                ResourceArchiverManager.ArchiveResx(targetDirectory, outputDirectory);
            }
        }

        static void ShowUsage()
        {

        }
    }
}
