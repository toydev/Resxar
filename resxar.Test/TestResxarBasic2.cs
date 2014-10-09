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
    class TestResxarBasic2 : TestResxarBasicBase
    {

        [Test]
        public void TestEmptyResource()
        {
            Environment.RunResxar("/in:example_in /out:example_out.resx");
            Environment.AssertResxarOutput(0, false, false);
            Assert.AreEqual(0, Environment.GetResources("example_out.resx").Count);
        }

        [Test]
        public void TestSingleStringResource()
        {
            Environment.CreateTextFile(@"example_in\resource1.txt", "resource1");
            Environment.RunResxar("/in:example_in /out:example_out.resx");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"txt_resource1", typeof(string)},
            });
        }

        [Test]
        public void TestSingleBitmapResource()
        {
            Environment.CreateBitmapFile(@"example_in\resource1.png", Resources.TestResource.box_png);
            Environment.RunResxar("/in:example_in /out:example_out.resx");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"png_resource1", typeof(Bitmap)},
            });
        }

        [Test]
        public void TestMultiformatBitmapResource()
        {
            Environment.CreateBitmapFile(@"example_in\resource1.bmp", Resources.TestResource.box_bmp);
            Environment.CreateBitmapFile(@"example_in\resource2.jpeg", Resources.TestResource.box_jpeg);
            Environment.CreateBitmapFile(@"example_in\resource3.jpg", Resources.TestResource.box_jpg);
            Environment.CreateBitmapFile(@"example_in\resource4.png", Resources.TestResource.box_png);
            Environment.CreateBitmapFile(@"example_in\resource5.tif", Resources.TestResource.box_tif);
            Environment.CreateBitmapFile(@"example_in\resource6.tiff", Resources.TestResource.box_tiff);
            Environment.RunResxar("/in:example_in /out:example_out.resx");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"bmp_resource1", typeof(Bitmap)},
                {"jpeg_resource2", typeof(Bitmap)},
                {"jpg_resource3", typeof(Bitmap)},
                {"png_resource4", typeof(Bitmap)},
                {"tif_resource5", typeof(Bitmap)},
                {"tiff_resource6", typeof(Bitmap)},
            });
        }

        [Test]
        public void TestBitmapIsFalse()
        {
            Environment.CreateBitmapFile(@"example_in\resource1.png", Resources.TestResource.box_png);
            Environment.RunResxar("/in:example_in /out:example_out.resx /bitmap:false");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"png_resource1", typeof(byte[])},
            });
        }

        [Test]
        public void TestUnknownResource()
        {
            Environment.CreateTextFile(@"example_in\resource1.unknown", "resource1");
            Environment.RunResxar("/in:example_in /out:example_out.resx");
            Environment.AssertResxarOutput(0, true, false);
            Environment.AssertResources(new Dictionary<string, Type>() {
                {"unknown_resource1", typeof(byte[])},
            });
        }

    }
}
