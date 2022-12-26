using Hndy.Ioc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hndy.Ioc.Gen.Tests
{
    [TestFixture]
    class SamplesTests
    {
        [Test]
        public void TestSamples()
        {
            var bench = new GenBench(new IocRegGenerator());
            bench.AddReference<IocRegistration>();
            var files = GenBench.LoadSourceFiles("Ioc", f => !f.Name.EndsWith("Tests.cs"));

            bench.Run(files);
            bench.AssertEmptyDiagnostics();
        }
    }
}
