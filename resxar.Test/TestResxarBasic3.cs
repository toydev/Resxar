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
    class TestResxarBasic3 : TestResxarBasicBase
    {

        [Test]
        public void TestSubdirectoryIsInputDirectory()
        {
            Environment.CreateTextFile(@"example_in\sub1\sub2\resource1.txt", "resource1");
            Environment.RunResxar(@"/in:example_in\sub1\sub2", "/out:example_out.resx");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"txt_resource1", typeof(string)},
            });
        }

        [Test]
        public void TestResourceInSubdirectory()
        {
            Environment.CreateTextFile(@"example_in\sub1\sub2\resource1.txt", "resource1");
            Environment.RunResxar(@"/in:example_in", "/out:example_out.resx");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"txt_sub1_sub2_resource1", typeof(string)},
            });
        }

        [Test]
        public void TestCheckTimestampIsDefault()
        {
            Environment.CreateTextFile(@"example_in\resource1.txt", "resource1");
            Environment.RunResxar(@"/in:example_in", "/out:example_out.resx");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"txt_resource1", typeof(string)},
            });

            Environment.RunResxar(@"/in:example_in", "/out:example_out.resx");
            Environment.AssertResxarOutput(0, false, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"txt_resource1", typeof(string)},
            });

            Environment.CreateTextFile(@"example_in\resource1.txt", "resource1");
            Environment.RunResxar(@"/in:example_in", "/out:example_out.resx");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"txt_resource1", typeof(string)},
            });
        }

        [Test]
        public void TestCheckTimestampIsTrue()
        {
            Environment.CreateTextFile(@"example_in\resource1.txt", "resource1");
            Environment.RunResxar(@"/in:example_in", "/out:example_out.resx", "/checkTimestamp:true");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"txt_resource1", typeof(string)},
            });

            Environment.RunResxar(@"/in:example_in", "/out:example_out.resx", "/checkTimestamp:true");
            Environment.AssertResxarOutput(0, false, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"txt_resource1", typeof(string)},
            });

            Environment.CreateTextFile(@"example_in\resource1.txt", "resource1");
            Environment.RunResxar(@"/in:example_in", "/out:example_out.resx", "/checkTimestamp:true");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"txt_resource1", typeof(string)},
            });
        }

        [Test]
        public void TestCheckTimestampIsFalse()
        {
            Environment.CreateTextFile(@"example_in\resource1.txt", "resource1");
            Environment.RunResxar(@"/in:example_in", "/out:example_out.resx", "/checkTimestamp:false");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"txt_resource1", typeof(string)},
            });

            Environment.RunResxar(@"/in:example_in", "/out:example_out.resx", "/checkTimestamp:false");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"txt_resource1", typeof(string)},
            });
        }

    }
}
