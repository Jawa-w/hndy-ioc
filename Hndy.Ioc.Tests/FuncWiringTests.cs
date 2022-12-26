using Hndy.Ioc.Tests.FuncWiring;
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
    class FuncWiringTests
    {
        [Test]
        public void TestFuncWiring()
        {
            var container = new IocContainer(new FuncWiringRegistration());
            Assert.That(container.Get<Foobar1>().GetFoo("a").Name, Is.EqualTo("a"));
            Assert.That(container.Get<Foobar1>().GetBar(), Is.Not.Null);
            Assert.That(container.Get<Foobar2>().GetBar(), Is.Not.Null);
            Assert.That(container.Get<Foobar2>().GetBin(), Is.Not.Null);
            Assert.That(container.Get<Foobar3>().GetFoo().Name, Is.EqualTo(""));
            Assert.That(container.Get<Foobar3>().GetBar?.Invoke(), Is.Not.Null);
            Assert.Throws<IocUnregisteredException>(() => container.Get<Foobar3>("err").GetFoo());
            Assert.That(container.Get<Foobar3>("err").GetBar, Is.Null);
            Assert.That(container.Get<Foobar3>("3").GetFoo().Name, Is.EqualTo("3"));
            Assert.That(container.Get<Foobar4>("a").TryGetFoo1()?.Name, Is.EqualTo("A"));
            Assert.That(container.Get<Foobar4>("b").TryGetFoo2()?.Name, Is.EqualTo("B"));
        }

        [Test]
        public void TestNullableFuncWiring()
        {
            var container = new IocContainer(new NullableFuncWiringRegistration());
            Assert.Throws<IocUnregisteredException>(() => container.Get<Foobar1>().GetFoo(""));
            Assert.Throws<IocUnregisteredException>(() => container.Get<Foobar1>().GetBar());
            Assert.That(container.Get<Foobar2>().GetBar(), Is.Null);
            Assert.That(container.Get<Foobar2>().GetBin(), Is.Null);
            Assert.That(container.Get<Foobar3>().GetFoo().Name, Is.EqualTo("5"));
            Assert.Throws<IocUnregisteredException>(() => container.Get<Foobar3>().GetBar!());
            Assert.That(container.Get<Foobar4>("").TryGetFoo1(), Is.Null);
            Assert.Throws<IocUnregisteredException>(() => container.Get<Foobar4>("").TryGetFoo2());
        }

        [Test]
        public void TestNestedFuncWiring()
        {
            var container = new IocContainer(new NestedFuncWiringRegistration());
            var foobar = container.Get<Foobar5>();
            Assert.Throws<IocUnregisteredException>(() => foobar.GetGetFoo1()(""));
            Assert.That(foobar.GetGetFoo2("abc")().Name, Is.EqualTo("ABC"));
        }
    }
}
