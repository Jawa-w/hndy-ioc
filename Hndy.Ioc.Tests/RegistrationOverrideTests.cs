using Hndy.Ioc.Tests.RegistrationOverride;
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
    class RegistrationOverrideTests
    {
        [Test]
        public void TestRegistrationOverride()
        {
            var container = new IocContainer(new Registration1(), new Registration3());
            Assert.That(container.Get<Foo>(), Is.Not.SameAs(container.Get<Foo>()));
            Assert.That(container.Get<IFoo>(), Is.TypeOf<Foo2>());

            container = new IocContainer(new Registration1(), new Registration2(), new Registration3());
            Assert.That(container.Get<Foo>(), Is.SameAs(container.Get<Foo>()));
            Assert.That(container.Get<IFoo>(), Is.TypeOf<Foo>());
        }
    }
}
