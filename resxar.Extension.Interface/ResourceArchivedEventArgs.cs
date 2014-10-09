// Resx Archiver Extension Interface
// https://github.com/toydev/Resxar
//
// Copyright (C) 2014 toydev All Rights Reserved.
//
// This software is released under Microsoft Public License(Ms-PL).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace resxar.Extension.Interface
{

    public class ResourceArchivedEventArgs : EventArgs
    {

        private string m_resourceFullpath;
        private string m_resourceName;
        private string m_resourceDescription;

        public ResourceArchivedEventArgs(
            string resourceFullpath, string resourceName, string resourceDescription)
        {
            if (resourceFullpath == null)
            {
                throw new ArgumentNullException("resourceFullpath");
            }
            if (resourceName == null)
            {
                throw new ArgumentNullException("resourceName");
            }
            if (resourceDescription == null)
            {
                throw new ArgumentNullException("resourceDescription");
            }

            m_resourceFullpath = resourceFullpath;
            m_resourceName = resourceName;
            m_resourceDescription = resourceDescription;
        }

        public string ResourceFullpath
        {
            get
            {
                return m_resourceFullpath;
            }
        }

        public string ResourceName
        {
            get
            {
                return m_resourceName;
            }
        }

        public string ResourceDescription
        {
            get
            {
                return m_resourceDescription;
            }
        }
    }

    public delegate void ResourceArchivedEventHandler(object sender, ResourceArchivedEventArgs e);

}
