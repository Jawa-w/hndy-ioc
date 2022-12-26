using Hndy.Ioc.Tests.Wiring;
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
    class WiringTests
    {
        [Test]
        public void TestManualWiring()
        {
            var container = new IocContainer(new ManualWiringRegistration());
            var foo = container.Get<Foo>(new FooOptions() { BarNum = 10, Name = "log" });
            Assert.That(foo.Bar.Logger.Name, Is.EqualTo("[bar10]"));
            Assert.That(foo.Logger?.Name, Is.EqualTo("[log]"));
        }

        [Test]
        public void TestAutoWiring()
        {
            var container = new IocContainer(new AutoWiringRegistration());
            var foo = container.Get<Foo>(new FooOptions() { BarNum = 10, Name = "log" });
            Assert.That(foo.Bar.Logger.Name, Is.EqualTo("[bar10]"));
            Assert.That(foo.Logger?.Name, Is.EqualTo("[log]"));
            Assert.That(container.Get<AnotherNs.Foo>(), Is.TypeOf<AnotherNs.Foo>());
        }
    }
}
