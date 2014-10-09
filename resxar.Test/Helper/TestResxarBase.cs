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
    class TestResxarBasicBase
    {
        ResxarEnvironment environment;

        protected ResxarEnvironment Environment
        {
            get
            {
                return environment;
            }
        }

        [SetUp]
        public void Setup()
        {
            environment = new ResxarEnvironment();
            environment.CreateDirectory("example_in");
            environment.DeleteFile("example_out.resx");
        }

        [TearDown]
        public void TearDown()
        {
            environment = new ResxarEnvironment();
        }
    }
}
