using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using log4net.Config;

using Mono.Options;

namespace Resxar
{
    class Program
    {
        private static ResourceArchiverManager ResourceArchiverManager { get; set; }
            = new ResourceArchiverManager();

        static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            XmlConfigurator.Configure(
                assembly.GetManifestResourceStream("resxar.Config.log.xml")
            );

            ResourceArchiverManager.Add(new TextResourceArchiver());
            ResourceArchiverManager.Add(new DirectoryResourceArchiver());

            string inputDirectory = null;
            string outputDirectory = null;
            bool help = false;

            OptionSet options = new OptionSet()
            {
                { "i|in=", "Resource input directory path.", v => inputDirectory = v },
                { "o|out=", "*.resx files output directory path.", v => outputDirectory = v },
                { "h|help", "Show help and exit.", v => help = v != null },
            };

            ResourceArchiverManager.AddOptionSet(options);

            IList<string> extra;
            try
            {
                extra = options.Parse(args);

                if (help)
                {
                    Usage(options);
                    return;
                }

                if (0 < extra.Count
                    || inputDirectory == null
                    || outputDirectory == null
                    || !ResourceArchiverManager.ValidateOptions())
                {
                    Usage(options);
                    return;
                }

                Run(inputDirectory, outputDirectory);
            }
            catch (OptionException)
            {
                Usage(options);
            }
            catch (Exception e)
            {
                Console.Error.Write(e);
                Usage(options);
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

        static string ApplicationName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            }
        }

        static void Usage(OptionSet options)
        {
            Console.WriteLine("Usage: {0} [OPTIONS]+", ApplicationName);
            Console.WriteLine("Application Description Here.");
            Console.WriteLine();

            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }
    }
}
