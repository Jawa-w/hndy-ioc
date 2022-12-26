using Hndy.Ioc.Tests.InjectSingletons;
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
    class InjectSingletonsTests
    {
        [Test]
        public void TestInjectSingletons()
        {
            var container = new IocContainer(new InjectSingletonsRegistration());

            container.InjectSingleton<Foo>(null);
            Assert.That(container.Get<Foo>().Value, Is.EqualTo(0));
            container.InjectSingleton(new Foo(1));
            Assert.That(container.Get<Foo>().Value, Is.EqualTo(1));
            container.InjectSingleton(default(int?), new Foo(2));
            Assert.That(container.Get<Foo>().Value, Is.EqualTo(2));
            container.InjectSingleton(default(string), new Foo(3));
            Assert.That(container.Get<Foo>().Value, Is.EqualTo(3));
            container.InjectSingleton(99, new Foo(999));
            Assert.That(container.Get<Foo>(99).Value, Is.EqualTo(999));
            container.InjectSingleton(20, new Foo(299));
            Assert.That(container.Get<Foo>(20).Value, Is.EqualTo(299));

            container.InjectSingleton<Fee>(null);
            Assert.That(() => container.Get<Fee>(), Throws.InstanceOf<IocUnregisteredException>());
            container.InjectSingleton(new Fee(1));
            Assert.That(container.Get<Fee>().Value, Is.EqualTo(1));
            container.InjectSingleton(99, new Fee(999));
            Assert.That(container.Get<Fee>(99).Value, Is.EqualTo(999));
            container.InjectSingleton((int?)20, new Fee(299));
            Assert.That(container.Get<Fee>(20).Value, Is.EqualTo(299));

            container.InjectSingleton((int?)99, (Bar?)new Bar(999));
            Assert.That(container.Get<Bar>(99).Value, Is.EqualTo(999));
            container.InjectSingleton((int?)20, new Bar(299));
            Assert.That(container.Get<Bar>(20).Value, Is.EqualTo(299));
            container.InjectSingleton("7", (Bar?)new Bar(7));
            Assert.That(container.Get<Bar>("7").Value, Is.EqualTo(7));
            container.InjectSingleton<Bar>(null);
            Assert.That(() => container.Get<Bar>(), Throws.InstanceOf<IocUnregisteredException>());
            container.InjectSingleton((Bar?)new Bar(1));
            Assert.That(container.Get<Bar>().Value, Is.EqualTo(1));
            container.InjectSingleton(default(string), (Bar?)new Bar(2));
            Assert.That(container.Get<Bar>().Value, Is.EqualTo(2));
            container.InjectSingleton(default(int?), (Bar?)new Bar(3));
            Assert.That(container.Get<Bar>().Value, Is.EqualTo(3));
            container.InjectSingleton(default(int?), new Bar(4));
            Assert.That(container.Get<Bar>().Value, Is.EqualTo(4));
        }
    }
}
