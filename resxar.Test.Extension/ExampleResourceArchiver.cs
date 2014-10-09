// Resx Archiver Extension for Test
// https://github.com/toydev/Resxar
//
// Copyright (C) 2014 toydev All Rights Reserved.
//
// This software is released under Microsoft Public License(Ms-PL).

using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;

using resxar.Extension.Interface;

namespace resxar.Test.Extension
{

    public class ExampleResourceArchiver : IResourceArchiver
    {
        private string m_param;
        private string m_skipPattern;

        public event ResourceArchivedEventHandler ResourceArchived;
        public event ResourceArchivedEventHandler ResourceArchiveSkipped;

        public string Usage()
        {
            return "ExampleResourceArchive Usgae";
        }

        public void SetParameters(IDictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("param"))
            {
                m_param = parameters["param"];
            }
            else
            {
                m_param = "default";
            }

            if (parameters.ContainsKey("skipPattern"))
            {
                m_skipPattern = parameters["skipPattern"];
            }

            if (parameters.ContainsKey("depends"))
            {
                resxar.Test.Extension.Depends.PublicFunction.Hello();
            }

            if (parameters.ContainsKey("exception"))
            {
                throw new ApplicationParameterException("exception parameter exists.");
            }
        }

        public void AddResource(ResXResourceWriter writer, string resourceFullPath, string resourceRelativePath)
        {
            if (m_skipPattern == null || !Regex.IsMatch(resourceRelativePath, m_skipPattern))
            {
                writer.AddResource(m_param, "test");
                ResourceArchived.Invoke(this, new ResourceArchivedEventArgs(resourceFullPath, m_param, "description"));
            }
            else
            {
                ResourceArchiveSkipped.Invoke(this, new ResourceArchivedEventArgs(resourceFullPath, m_param, "description"));
            }
        }

    }

}
