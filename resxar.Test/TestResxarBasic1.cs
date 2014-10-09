// Resx Archiver Test
// https://github.com/toydev/Resxar
//
// Copyright (C) 2014 toydev All Rights Reserved.
//
// This software is released under Microsoft Public License(Ms-PL).

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

using NUnit.Framework;

using resxar;

namespace resxar.Test
{
    [TestFixture]
    class TestResxarBasic1 : TestResxarBasicBase
    {

        [Test]
        public void TestNoArguments()
        {
            Environment.RunResxar();
            Environment.AssertResxarOutput(1, true, false);
        }

        [Test]
        public void TestInArgumentsOnly()
        {
            Environment.RunResxar("/in:example_in");
            Environment.AssertResxarOutput(2, true, true);
        }

        [Test]
        public void TestOutArgumentsOnly()
        {
            Environment.RunResxar("/out:example_out.resx");
            Environment.AssertResxarOutput(2, true, true);
        }

        [Test]
        public void TestInputDirectoryIsInvalidPath()
        {
            Environment.RunResxar("/in:*** /out:example_out.resx");
            Environment.AssertResxarOutput(255, false, true);
        }

        [Test]
        public void TestInputDirectoryIsNotFound()
        {
            Environment.RunResxar("/in:not_found /out:example_out.resx");
            Environment.AssertResxarOutput(255, false, true);
        }

        [Test]
        public void TestOutputFilenameIsInvalidPath()
        {
            Environment.RunResxar("/in:example_in /out:***");
            Environment.AssertResxarOutput(255, false, true);
        }

        [Test]
        public void TestDontCreateOutputFile()
        {
            Environment.CreateDirectory("example.out");
            try
            {
                Environment.RunResxar("/in:example_in /out:example.out");
                Environment.AssertResxarOutput(255, false, true);
            }
            finally
            {
                Environment.DeleteDirectory("example.out");
            }
        }

    }
}
