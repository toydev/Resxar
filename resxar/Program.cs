// Resx Archiver
// https://github.com/toydev/Resxar
//
// Copyright (C) 2014 toydev All Rights Reserved.
//
// This software is released under Microsoft Public License(Ms-PL).

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;

using resxar.Extension.Interface;
using resxar.Extension.Standard;

namespace resxar
{
    class Program
    {

        public const int RETURNCODE_SUCCESSFUL = 0;
        public const int RETURNCODE_OUTPUT_USAGE = 1;
        public const int RETURNCODE_ARGUMENT_ERROR = 2;
        public const int RETURNCODE_APPLICATION_ERROR = 3;
        public const int RETURNCODE_UNKNOWN_ERROR = 255;
        
        private const string RESX_EXTENSION = "resx";

        private static string DEFAULT_RESOURCE_ARCHIVER = typeof(StandardResourceArchiver).FullName;

        private static void OutputUsage(IResourceArchiver resourceArchiver)
        {
            Console.WriteLine("{0} {1}", App.AssemblyTitle, App.Version);
            Console.WriteLine(App.AssemblyCopyright);
            Console.WriteLine();
            Console.WriteLine(Resources.ProgramMessage.usage);
            if (resourceArchiver != null && !(resourceArchiver is StandardResourceArchiver))
            {
                Console.WriteLine();
                Console.WriteLine(Resources.ProgramMessage.usage_resource_archiver_header);
                Console.WriteLine(resourceArchiver.Usage());
            }
        }

        private static void OutputError(Exception e)
        {
            Console.Error.WriteLine(GetExceptionMessage(e));
        }

        private static int Main(string[] args)
        {
            IResourceArchiver resourceArchiver = null;
            try
            {
                IDictionary<string, string> parameters = GetParameters(args);

                if (parameters.Count == 0)
                {
                    OutputUsage(resourceArchiver);
                    return RETURNCODE_OUTPUT_USAGE;
                }
                else
                {
                    IList<string> missingParameters = CheckRequiredParameters(parameters, new List<string>() {
                        "in", "out",
                    });
                    if (0 < missingParameters.Count)
                    {
                        Console.Error.WriteLine(String.Format(
                            Resources.ProgramMessage.exception_message_parameter_is_missing,
                            String.Join(",", missingParameters)));
                        OutputUsage(resourceArchiver);
                        return RETURNCODE_ARGUMENT_ERROR;
                    }
                }

                resourceArchiver = GetResourceArchiver(parameters);
                resourceArchiver.SetParameters(parameters);

                resourceArchiver.ResourceArchived += new ResourceArchivedEventHandler(ResourceArchived);
                resourceArchiver.ResourceArchiveSkipped += new ResourceArchivedEventHandler(ResourceArchiveSkipped);

                string inputDirectory = GetInputDirectory(parameters);
                string outputFilename = GetOutputFilename(parameters);
                IList<string> targetFiles = GetTargetFiles(inputDirectory);
                if (!GetCheckTimestamp(parameters) || CheckTimestamp(targetFiles, outputFilename))
                {
                    ArchiveResx(resourceArchiver, inputDirectory, outputFilename, targetFiles);
                }
                return RETURNCODE_SUCCESSFUL;
            }
            catch (ApplicationParameterException e)
            {
                Console.Error.WriteLine(e.Message);
                OutputUsage(resourceArchiver);
                return RETURNCODE_ARGUMENT_ERROR;
            }
            catch (ApplicationException e)
            {
                Console.Error.WriteLine(GetExceptionMessage(e));
                return RETURNCODE_APPLICATION_ERROR;
            }
            catch (Exception e)
            {
                OutputError(e);
                return RETURNCODE_UNKNOWN_ERROR;
            }
        }

        static string GetInputDirectory(IDictionary<string, string> parameters)
        {
            return parameters["in"];
        }

        static string GetOutputFilename(IDictionary<string, string> parameters)
        {
            string extension = Path.GetExtension(parameters["out"]);
            if (extension == String.Empty)
            {
                return String.Format("{0}.{1}", parameters["out"], RESX_EXTENSION);
            }
            else
            {
                return parameters["out"];
            }
        }

        static bool GetCheckTimestamp(IDictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("checkTimestamp"))
            {
                return Boolean.Parse(parameters["checkTimestamp"]);
            }
            else
            {
                return true;
            }
        }

        static string GetResourceArchiverName(IDictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("extensionResourceArchiver"))
            {
                return parameters["extensionResourceArchiver"];
            }
            else
            {
                return DEFAULT_RESOURCE_ARCHIVER;
            }
        }

        static IResourceArchiver GetResourceArchiver(IDictionary<string, string> parameters)
        {
            Assembly assembly = null;
            if (parameters.ContainsKey("extensionDll") && parameters["extensionDll"] != null)
            {
                try
                {
                    assembly = Assembly.LoadFrom(parameters["extensionDll"]);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(String.Format(
                        Resources.ProgramMessage.exception_message_invalid_extension_dll,
                        parameters["extensionDll"]
                        ), e);
                }
            }

            try
            {
                Type resourceArchiverType = (assembly != null)
                    ? assembly.GetType(GetResourceArchiverName(parameters))
                    : Type.GetType(GetResourceArchiverName(parameters));
                return (IResourceArchiver) Activator.CreateInstance(resourceArchiverType);
            }
            catch (Exception e)
            {
                throw new ApplicationException(
                    Resources.ProgramMessage.exception_message_resource_archiver_initialize_error, e);
            }
        }

        static IList<string> GetTargetFiles(string inputDirectory)
        {
            IList<string> result = new List<string>();

            Stack<string> targetDirectories = new Stack<string>();
            targetDirectories.Push(Path.GetFullPath(inputDirectory));

            while (0 < targetDirectories.Count)
            {
                string targetDirectory = targetDirectories.Pop();

                foreach (string targetFilename in Directory.GetFiles(targetDirectory))
                {
                    result.Add(targetFilename);
                }

                foreach (string subdirectory in Directory.GetDirectories(targetDirectory))
                {
                    targetDirectories.Push(subdirectory);
                }
            }

            return result;
        }
        
        static void ArchiveResx(IResourceArchiver resourceArchiver, string inputDirectory, string outputFilename, IList<string> targetFiles)
        {
            Stack<string> targetDirectories = new Stack<string>();
            targetDirectories.Push(Path.GetFullPath(inputDirectory));

            Uri rootDirectory = new Uri(Path.GetFullPath(inputDirectory) + "/");
            using (ResXResourceWriter writer = new ResXResourceWriter(
                new FileStream(outputFilename, FileMode.Create)))
            {
                foreach (string targetFilename in targetFiles)
                {
                    Uri relativeUrl = rootDirectory.MakeRelativeUri(new Uri(targetFilename));
                    resourceArchiver.AddResource(writer, targetFilename, relativeUrl.ToString());
                }
            }
        }

        static bool CheckTimestamp(IList<string> targetFiles, string outputFilename)
        {
            if (!File.Exists(outputFilename))
            {
                return true;
            }

            DateTime destinationFileTimestamp = File.GetLastWriteTime(outputFilename);

            foreach (string targetFilename in targetFiles)
            {
                DateTime targetFileTimestamp = File.GetLastWriteTime(targetFilename);
                if (destinationFileTimestamp < targetFileTimestamp)
                {
                    return true;
                }
            }

            return false;
        }

        static IDictionary<string, string> GetParameters(
            string[] arguments)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            List<string> argsList = new List<string>(arguments);
            for (int i = argsList.Count - 1; 0 <= i; --i)
            {
                MatchCollection mc = Regex.Matches(argsList[i], "^/([a-zA-Z0-9_]+)(:(.+))?$");
                if (0 < mc.Count)
                {
                    Match match = mc[0];
                    switch (match.Groups.Count)
                    {
                        case 2:
                            result.Add(match.Groups[1].Value, null);
                            break;
                        case 4:
                            result.Add(match.Groups[1].Value, match.Groups[3].Value);
                            break;
                    }
                }
            }

            return result;
        }

        private static IList<string> CheckRequiredParameters(IDictionary<string, string> parameters, IList<string> requiredParameters)
        {
            IList<string> missingParameters = new List<string>();
            foreach (string requiredParameter in requiredParameters)
            {
                if (!parameters.ContainsKey(requiredParameter))
                {
                    missingParameters.Add(requiredParameter);
                }
            }
            return missingParameters;
        }

        private static void ResourceArchived(object sender, ResourceArchivedEventArgs e)
        {
            Console.WriteLine("{0}: {1} [{2}]", e.ResourceName, e.ResourceFullpath, e.ResourceDescription);
        }

        private static void ResourceArchiveSkipped(object sender, ResourceArchivedEventArgs e)
        {
            Console.Error.WriteLine("Skipped {0}: {1} [{2}]", e.ResourceName, e.ResourceFullpath, e.ResourceDescription);
        }

        private static string GetExceptionMessage(Exception e)
        {
            List<string> result = new List<string>();
            int count = 0;
            while (e != null && count <= 10)
            {
                ++count;
                result.Add(e.GetType().FullName + ": " + e.Message + Environment.NewLine + e.StackTrace.ToString());
                e = e.InnerException;
            }
            return String.Join(Environment.NewLine + "Caused by ", result);
        }
    }
}
