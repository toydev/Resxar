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
using System.Text.RegularExpressions;

using NUnit.Framework;

using resxar;

namespace resxar.Test
{
    [TestFixture]
    class TestResxarExtension1 : TestResxarBasicBase
    {

        [Test]
        public void TestStandard()
        {
            Environment.CreateTextFile(@"example_in\resource1.txt", "resource1");
            Environment.RunResxar(
                @"/in:example_in", "/out:example_out.resx",
                "/extensionDll:resxar.Test.Extension.dll",
                "/extensionResourceArchiver:resxar.Test.Extension.ExampleResourceArchiver"
                );
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"default", typeof(string)},
            });
        }

        [Test]
        public void TestDllIsNotFound()
        {
            Environment.RunResxar(
                @"/in:example_in", "/out:example_out.resx",
                "/extensionDll:Unknown.dll",
                "/extensionResourceArchiver:resxar.Test.Extension.ExampleResourceArchiver"
                );
            Environment.AssertResxarOutput(3, false, true);
        }

        [Test]
        public void TestDllIsNotDll()
        {
            Environment.CreateTextFile("test.dll", "text");
            Environment.RunResxar(
                @"/in:example_in", "/out:example_out.resx",
                "/extensionDll:test.dll",
                "/extensionResourceArchiver:resxar.Test.Extension.ExampleResourceArchiver"
                );
            Environment.AssertResxarOutput(3, false, true);
        }

        [Test]
        public void TestParameter()
        {
            Environment.CreateTextFile(@"example_in\resource1.txt", "resource1");
            Environment.RunResxar(
                @"/in:example_in", "/out:example_out.resx",
                "/extensionDll:resxar.Test.Extension.dll",
                "/extensionResourceArchiver:resxar.Test.Extension.ExampleResourceArchiver",
                "/param:parameter"
                );
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"parameter", typeof(string)},
            });
        }

        [Test]
        public void TestApplicationParameterException()
        {
            Environment.CreateTextFile(@"example_in\resource1.txt", "resource1");
            Environment.RunResxar(
                @"/in:example_in", "/out:example_out.resx",
                "/extensionDll:resxar.Test.Extension.dll",
                "/extensionResourceArchiver:resxar.Test.Extension.ExampleResourceArchiver",
                "/exception"
                );
            Environment.AssertResxarOutput(2, true, true);
            Assert.IsTrue(Regex.IsMatch(
                Environment.LastStandardOutput, "ExampleResourceArchive Usgae"));
        }

        [Test]
        public void TestSkip()
        {
            Environment.CreateTextFile(@"example_in\resource1.txt", "resource1");
            Environment.RunResxar(
                @"/in:example_in", "/out:example_out.resx",
                "/extensionDll:resxar.Test.Extension.dll",
                "/extensionResourceArchiver:resxar.Test.Extension.ExampleResourceArchiver",
                "/skipPattern:^resource1"
                );
            Environment.AssertResxarOutput(0, false, true);
            Assert.IsTrue(Regex.IsMatch(
                Environment.LastStandardError, "Skipped default"));
        }

        [Test]
        public void TestUseDependsFunction()
        {
            Environment.CreateTextFile(@"example_in\resource1.txt", "resource1");
            Environment.RunResxar(
                @"/in:example_in", "/out:example_out.resx",
                "/extensionDll:resxar.Test.Extension.dll",
                "/extensionResourceArchiver:resxar.Test.Extension.ExampleResourceArchiver",
                "/depends"
                );
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"default", typeof(string)},
            });
        }

        [Test]
        public void TestDependsIsNotFound()
        {
            Environment.CreateTextFile(@"example_in\resource1.txt", "resource1");
            File.Copy("resxar.Test.Extension.Depends.dll", "resxar.Test.Extension.Depends.txt", true);
            File.Delete("resxar.Test.Extension.Depends.dll");
            try
            {
                Environment.RunResxar(
                    @"/in:example_in", "/out:example_out.resx",
                    "/extensionDll:resxar.Test.Extension.dll",
                    "/extensionResourceArchiver:resxar.Test.Extension.ExampleResourceArchiver",
                    "/depends"
                    );
                Environment.AssertResxarOutput(255, false, true);
            }
            finally
            {
                File.Copy("resxar.Test.Extension.Depends.txt", "resxar.Test.Extension.Depends.dll");
                File.Delete("resxar.Test.Extension.Depends.txt");
            }
        }

    }
}
