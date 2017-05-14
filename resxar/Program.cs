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

        const string DEFAULT_INPUT_DIRECTORY = ".";
        const string DEFAULT_OUTPUT_DIRECTORY = "resx_output";

        static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream configStream = assembly.GetManifestResourceStream("Resxar.Config.log.xml"))
            {
                XmlConfigurator.Configure(configStream);
            }

            ResourceArchiverManager.Add(new TextResourceArchiver());
            ResourceArchiverManager.Add(new DirectoryResourceArchiver());

            string inputDirectory = DEFAULT_INPUT_DIRECTORY;
            string outputDirectory = DEFAULT_OUTPUT_DIRECTORY;
            bool help = false;

            OptionSet options = new OptionSet()
            {
                { "i|in=", string.Format("Resource input directory path. The default is '{0}'.", DEFAULT_INPUT_DIRECTORY), v => inputDirectory = v },
                { "o|out=", string.Format("*.resx files output directory path. The default is '{0}'.", DEFAULT_OUTPUT_DIRECTORY), v => outputDirectory = v },
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

                Run(inputDirectory, outputDirectory);
            }
            catch (Exception e)
            {
                Console.Write("{0}: ", ApplicationName);
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", ApplicationName);
                return;
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
                if (targetDirectory != DEFAULT_OUTPUT_DIRECTORY)
                {
                    ResourceArchiverManager.ArchiveResx(targetDirectory, outputDirectory);
                }
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
            Console.WriteLine();

            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }
    }
}
