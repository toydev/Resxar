using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using log4net;
using log4net.Config;

using Mono.Options;

namespace Resxar
{
    class Program
    {
        private static ILog Logger { get; set; }

        private static ResourceArchiverManager ResourceArchiverManager { get; set; }
            = new ResourceArchiverManager();

        const string DEFAULT_INPUT_DIRECTORY = ".";
        const string DEFAULT_OUTPUT_DIRECTORY = "resx_output";

        public static int Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream configStream = assembly.GetManifestResourceStream("Resxar.Config.log.xml"))
            {
                XmlConfigurator.Configure(configStream);
            }
            Logger = LogManager.GetLogger(typeof(Program));

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
                    return 1;
                }

                Run(inputDirectory, outputDirectory);
                return 0;
            }
            catch (OptionException e)
            {
                Logger.Error("OptionException", e);
                OutputException(e);
                return 2;
            }
            catch (ApplicationException e)
            {
                Logger.Error("ApplicationException", e);
                OutputException(e);
                return 3;
            }
            catch (Exception e)
            {
                Logger.Error("UnknownException", e);
                OutputException(e);
                return 255;
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
                if (Path.GetFileName(targetDirectory) != DEFAULT_OUTPUT_DIRECTORY)
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
            Console.Error.WriteLine("Usage: {0} [OPTIONS]+", ApplicationName);
            Console.Error.WriteLine();

            Console.Error.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Error);
        }

        static void OutputException(Exception e)
        {
            Console.Error.Write("{0}: ", ApplicationName);
            Console.Error.WriteLine(e.Message);
            Console.Error.WriteLine("Try `{0} --help' for more information.", ApplicationName);
        }
    }
}
