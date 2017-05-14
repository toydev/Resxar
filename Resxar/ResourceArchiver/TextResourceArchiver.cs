using System;
using System.IO;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;

using log4net;

using Mono.Options;

namespace Resxar
{
    public class TextResourceArchiver : IResourceArchiver
    {
        private ILog logger = LogManager.GetLogger(typeof(TextResourceArchiver));

        public void AddOptionSet(OptionSet options)
        {
        }

        public bool IsTarget(string path)
        {
            return File.Exists(path) && Path.GetExtension(path) == ".txt";
        }

        public string OutputFilepath(string targetPath, string outputDirectory)
        {
            return Path.Combine(
                outputDirectory,
                Path.GetFileNameWithoutExtension(targetPath) + ".resx");
        }

        private static Regex COMMENT_REGEX
            = new Regex(@"^\s*#");
        private static Regex ONE_LINE_TEXT_REGEX
            = new Regex(@"^\s*([a-zA-Z_][0-9a-zA-Z_]*)\s*(\.\s*([a-zA-Z_][0-9a-zA-Z\-_]*))?\s*=(.*)$");
        private static Regex MULTI_LINE_TEXT_REGEX
            = new Regex(@"^\s*([a-zA-Z_][0-9a-zA-Z_]*)\s*(\.\s*([a-zA-Z_][0-9a-zA-Z\-_]*))?\s*<<\s*([a-zA-Z_][0-9a-zA-Z_]*)\s*$");
        private static Regex DELIMITER_REGEX
            = new Regex(@"^\s*([a-zA-Z_][0-9a-zA-Z_]*)\s*$");

        public void Archive(string targetPath, string outputDirectory)
        {
            logger.Info(string.Format("Archive ... {0}", Path.GetFileName(targetPath)));

            string outputFilepath = OutputFilepath(targetPath, outputDirectory);
            using (StreamReader reader = new StreamReader(new FileStream(targetPath, FileMode.Open)))
            using (ResXResourceWriterManager writerManager = new ResXResourceWriterManager(outputFilepath))
            {
                string line;
                string multilineName = null;
                string multilineLocale = null;
                string multilineDelimiter = null;
                int multilineCount = 0;
                StringBuilder multilineValue = null;
                while ((line = reader.ReadLine()) != null)
                {
                    // コメント
                    if (Regex.IsMatch(line, "^#"))
                    {
                        continue;
                    }

                    if (multilineName == null)
                    {
                        Match oneLineTextMatch = ONE_LINE_TEXT_REGEX.Match(line);
                        if (oneLineTextMatch.Success)
                        {
                            string name = oneLineTextMatch.Groups[1].Value;
                            string locale = oneLineTextMatch.Groups[3].Value;
                            string value = oneLineTextMatch.Groups[4].Value;
                            ResXResourceWriter writer = writerManager.GetWriter(locale);
                            writer.AddResource(name, value);
                            continue;
                        }

                        Match multiLineTextMatch = MULTI_LINE_TEXT_REGEX.Match(line);
                        if (multiLineTextMatch.Success)
                        {
                            multilineName = multiLineTextMatch.Groups[1].Value;
                            multilineLocale = multiLineTextMatch.Groups[3].Value;
                            multilineDelimiter = multiLineTextMatch.Groups[4].Value;
                            multilineCount = 0;
                            multilineValue = new StringBuilder();
                            continue;
                        }
                    }
                    else
                    {
                        Match delimiterMatcher = DELIMITER_REGEX.Match(line);
                        if (delimiterMatcher.Success)
                        {
                            string delimiter = delimiterMatcher.Groups[1].Value;
                            if (multilineDelimiter == delimiter)
                            {
                                ResXResourceWriter writer = writerManager.GetWriter(multilineLocale);
                                writer.AddResource(multilineName, multilineValue.ToString());

                                multilineName = null;
                                multilineLocale = null;
                                multilineDelimiter = null;
                                multilineCount = 0;
                                multilineValue = null;
                                continue;
                            }
                        }

                        if (0 < multilineCount)
                        {
                            multilineValue.Append(Environment.NewLine);
                        }
                        multilineValue.Append(line);
                        ++multilineCount;
                    }
                }
            }
        }
    }
}
