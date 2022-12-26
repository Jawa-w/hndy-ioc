using Hndy.Ioc.Tests.CircularDependencies;
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
    class CircularDependenciesTests
    {
        [Test]
        public void TestCircularDependencies()
        {
            var container = new IocContainer(new CircularDependenciesRegistration());
            Assert.Throws<IocCircularDependenciesException>(() => container.Get<Foo>());
            Assert.Throws<IocCircularDependenciesException>(() => container.Get<Foo>(0));
        }
    }
}
