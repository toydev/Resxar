// Resx Archiver
// https://github.com/toydev/Resxar
//
// Copyright (C) 2014 toydev All Rights Reserved.
//
// This software is released under Microsoft Public License(Ms-PL).

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;

using resxar.Extension.Interface;

namespace resxar.Extension.Standard
{
    class StandardResourceArchiver : IResourceArchiver
    {

        private Encoding m_encoding;
        private bool m_useBitmap;

        public event ResourceArchivedEventHandler ResourceArchived;
        public event ResourceArchivedEventHandler ResourceArchiveSkipped;

        public string Usage()
        {
            return Resources.StandardResourceArchiverMessage.usage;
        }

        public void SetParameters(IDictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("encoding"))
            {
                m_encoding = Encoding.GetEncoding(parameters["encoding"]);
            }
            else
            {
                m_encoding = Encoding.UTF8;
            }

            if (parameters.ContainsKey("bitmap"))
            {
                m_useBitmap = Boolean.Parse(parameters["bitmap"]);
            }
            else
            {
                m_useBitmap = true;
            }
        }

        public void AddResource(ResXResourceWriter writer, string resourceFullPath, string resourceRelativePath)
        {
            string extension = Path.GetExtension(resourceRelativePath).Replace(".", "");

            string resourceName = GetResourceName(
                extension,
                Path.GetDirectoryName(resourceRelativePath),
                Path.GetFileNameWithoutExtension(resourceRelativePath)
                );

            if (Either(extension, "txt"))
            {
                string resource = GetTextFromFile(resourceFullPath);
                writer.AddResource(resourceName, resource);
                ResourceArchived.Invoke(this,
                    new ResourceArchivedEventArgs(
                        resourceFullPath,
                        resourceName,
                        String.Format("type=string, chars={0}, encoding={1}", resource.Length, m_encoding.WebName)
                        ));
            }
            else if (Either(extension, "png", "bmp", "jpg", "jpeg", "gif", "tif", "tiff"))
            {
                if (m_useBitmap)
                {
                    Bitmap resource = new Bitmap(resourceFullPath);
                    writer.AddResource(resourceName, resource);
                    ResourceArchived.Invoke(this,
                        new ResourceArchivedEventArgs(
                            resourceFullPath,
                            resourceName,
                            String.Format("type=Bitmap, width={0}, height={1}", resource.Width, resource.Height)
                            ));
                }
                else
                {
                    byte[] resource = GetBytesFromFile(resourceFullPath);
                    writer.AddResource(resourceName, resource);
                    ResourceArchived.Invoke(this,
                        new ResourceArchivedEventArgs(
                            resourceFullPath,
                            resourceName,
                            String.Format("type=byte[], byte={0}", resource.Length)
                            ));
                }
            }
            else
            {
                byte[] resource = GetBytesFromFile(resourceFullPath);
                writer.AddResource(resourceName, resource);
                ResourceArchived.Invoke(this,
                    new ResourceArchivedEventArgs(
                        resourceFullPath,
                        resourceName,
                        String.Format("type=byte[], byte={0}", resource.Length)
                        ));
            }
        }

        private string GetTextFromFile(string filename)
        {
            return m_encoding.GetString(GetBytesFromFile(filename));
        }

        private static string GetResourceName(string extension, string path, string filenameWithoutExtension)
        {
            StringBuilder builder = new StringBuilder();
            if (0 < extension.Length)
            {
                builder.Append(extension);
                builder.Append("_");
            }

            if (0 < path.Length)
            {
                builder.Append(path);
                builder.Append("_");
            }

            builder.Append(filenameWithoutExtension);

            string result = Regex.Replace(builder.ToString(), @"[/\\.]", "_");

            if (Regex.IsMatch(result, "^[0-9]"))
            {
                return "_" + result;
            }
            else
            {
                return result;
            }
        }

        private static bool Either(string target, params string[] compares)
        {
            foreach (string compare in compares)
            {
                if (target == compare)
                {
                    return true;
                }
            }
            return false;
        }

        private static byte[] GetBytesFromFile(string filename)
        {
            List<byte> result = new List<byte>();
            using (Stream stream = new FileStream(filename, FileMode.Open))
            {
                int bufferSize = 1024;
                byte[] buffer = new byte[1024];
                int offset = 0;
                while (true)
                {
                    int length = stream.Read(buffer, offset, bufferSize);
                    if (length == 0)
                    {
                        break;
                    }
                    else
                    {
                        if (length == bufferSize)
                        {
                            result.AddRange(buffer);
                        }
                        else
                        {
                            for (int i = 0; i < length; ++i)
                            {
                                result.Add(buffer[i]);
                            }
                        }
                    }
                }
            }
            return result.ToArray();
        }

    }
}
