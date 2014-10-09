// Resx Archiver
// https://github.com/toydev/Resxar
//
// Copyright (C) 2014 toydev All Rights Reserved.
//
// This software is released under Microsoft Public License(Ms-PL).

using System;
using System.IO;
using System.Reflection;

namespace resxar
{
    class App
    {

        public static Assembly Assembly
        {
            get
            {
                return Assembly.GetEntryAssembly();
            }
        }

        public static string AssemblyTitle
        {
            get
            {
                return ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(
                    Assembly, typeof(AssemblyTitleAttribute))
                    ).Title;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                return ((AssemblyProductAttribute)Attribute.GetCustomAttribute(
                    Assembly, typeof(AssemblyProductAttribute))
                    ).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                return ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(
                    Assembly, typeof(AssemblyCopyrightAttribute))
                    ).Copyright;
            }
        }

        public static string Version
        {
            get
            {
                Version version = Assembly.GetEntryAssembly().GetName().Version;
                return String.Format(
                    "{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

    }
}
