using System;
using System.Collections.Generic;
using System.IO;

using CommandLineParser.Exceptions;

namespace Resxar
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineParser.CommandLineParser parser = new CommandLineParser.CommandLineParser();
            try
            {
                CommandLineOptions options = new CommandLineOptions();
                parser.ExtractArgumentAttributes(options);
                parser.ParseCommandLine(args);

                Run(options);
            }
            catch (CommandLineException)
            {
                parser.ShowUsage();
            }
            catch (Exception e)
            {
                Console.Error.Write(e);
            }
        }

        private static IList<IResourceArchiver> Archivers { get; set; } = new List<IResourceArchiver>();

        static void Run(CommandLineOptions options)
        {
            Archivers.Add(new TextResourceArchiver());
            Archivers.Add(new DirectoryResourceArchiver());

            foreach (string targetFilename in Directory.GetFiles(options.InputDirectory))
            {
                ArchiveResx(targetFilename, options.OutputDirectory);
            }

            foreach (string targetDirectory in Directory.GetDirectories(options.InputDirectory))
            {
                ArchiveResx(targetDirectory, options.OutputDirectory);
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
