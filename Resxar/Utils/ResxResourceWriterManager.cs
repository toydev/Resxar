using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace Resxar
{
    public class ResXResourceWriterManager : IDisposable
    {
        private IDictionary<string, ResXResourceWriter> Writers { get; set; }
            = new Dictionary<string, ResXResourceWriter>();

        public string Filepath { get; private set; }

        public ResXResourceWriterManager(string filepath)
        {
            Filepath = filepath;
        }

        public ResXResourceWriter GetWriter(string locale)
        {
            ResXResourceWriter result;
            if (Writers.TryGetValue(locale, out result))
            {
                return result;
            }
            else
            {
                result = new ResXResourceWriter(GetFilepath(locale));
                Writers[locale] = result;
                return result;
            }
        }

        public string GetFilepath(string locale)
        {
            if (locale == null || locale == "")
            {
                return Filepath;
            }
            else
            {
                return Path.Combine(
                    Path.GetDirectoryName(Filepath),
                    Path.GetFileNameWithoutExtension(Filepath)
                    + "." + locale
                    + Path.GetExtension(Filepath));
            }
        }

        public void Dispose()
        {
            foreach (ResXResourceWriter writer in Writers.Values)
            {
                writer.Close();
            }
        }
    }
}
