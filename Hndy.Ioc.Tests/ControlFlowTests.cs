using Hndy.Ioc.Tests.ControlFlow;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hndy.Ioc.Tests
{
    [TestFixture]
    class ControlFlowTests
    {
        [Test]
        public void TestControlFlow()
        {
            var container = new IocContainer(new ControlFlowRegistration(true));
            Assert.That(container.Get<IBar>(), Is.TypeOf<BarDebug>());
            Assert.That(container.Get<Foo>(1).Num, Is.EqualTo(1));
        }
    }
}
