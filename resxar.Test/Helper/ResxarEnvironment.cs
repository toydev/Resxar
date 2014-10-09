using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Text;

using NUnit.Framework;

namespace resxar.Test
{
    class ResxarEnvironment
    {

        private const string RESXAR_COMMAND_NAME = "resxar.exe";
        private const int RESXAR_TIME_LIMIT = 10 * 0000;

        public void CreateDirectory(string relativePath)
        {
            DeleteDirectory(relativePath);
            Directory.CreateDirectory(relativePath);
        }

        public void CreateFileDirectory(string relativePath)
        {
            string directory = Path.GetDirectoryName(relativePath);
            if (0 < directory.Length && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void DeleteDirectory(string relativePath)
        {
            if (Directory.Exists(relativePath))
            {
                Directory.Delete(relativePath, true);
            }
        }

        public void CreateTextFile(string relativePath, string content)
        {
            CreateFileDirectory(relativePath);

            using (StreamWriter writer = new StreamWriter(new FileStream(relativePath, FileMode.Create)))
            {
                writer.Write(content);
            }
        }

        public void CreateBitmapFile(string relativePath, Bitmap bitmap)
        {
            CreateFileDirectory(relativePath);

            bitmap.Save(relativePath);
        }

        public void DeleteFile(string relativePath)
        {
            if (File.Exists(relativePath))
            {
                File.Delete(relativePath);
            }
        }

        public void RunResxar(params string[] arguments)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = RESXAR_COMMAND_NAME;
            processInfo.Arguments = String.Join(" ", arguments);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            Process process = Process.Start(processInfo);
            m_lastStandardOutput = process.StandardOutput.ReadToEnd();
            m_lastStandardError = process.StandardError.ReadToEnd();
            process.WaitForExit();
            m_lastExitCode = process.ExitCode;
        }

        public IDictionary<string, Type> GetResources(string relativePath)
        {
            IDictionary<string, Type> result = new Dictionary<string, Type>();
            using (ResXResourceReader reader = new ResXResourceReader(relativePath))
            {
                IDictionaryEnumerator enumerator = reader.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    result.Add((string)enumerator.Key, enumerator.Value.GetType());
                }
            }
            return result;
        }


        
        private int m_lastExitCode;
        private string m_lastStandardOutput;
        private string m_lastStandardError;

        public int LastExitCode
        {
            get
            {
                return m_lastExitCode;
            }
        }

        public string LastStandardOutput
        {
            get
            {
                return m_lastStandardOutput;
            }
        }

        public string LastStandardError
        {
            get
            {
                return m_lastStandardError;
            }
        }


        public void AssertResxarOutput(int exitCode, bool existOutput, bool existError)
        {
            Console.WriteLine("ExitCode: {0}", LastExitCode);
            Console.WriteLine("StadardOutput:");
            Console.WriteLine(this.LastStandardOutput);
            Console.WriteLine("StadardError:");
            Console.WriteLine(this.LastStandardError);

            Assert.AreEqual(exitCode, LastExitCode);
            if (existOutput)
            {
                Assert.Less(0, this.LastStandardOutput.Length);
            }
            else
            {
                Assert.AreEqual(0, this.LastStandardOutput.Length);
            }
            if (existError)
            {
                Assert.Less(0, this.LastStandardError.Length);
            }
            else
            {
                Assert.AreEqual(0, this.LastStandardError.Length);
            }
        }

        public void AssertResources(IDictionary<string, Type> expected)
        {
            IDictionary<string, Type> resources = GetResources("example_out.resx");
            Assert.AreEqual(expected.Count, resources.Count);

            foreach (KeyValuePair<string, Type> resource in resources)
            {
                Assert.IsTrue(expected.ContainsKey(resource.Key), String.Format("{0} is nothing.", resource.Key));
                Assert.AreEqual(expected[resource.Key], resource.Value);
            }

        }
    }
}
