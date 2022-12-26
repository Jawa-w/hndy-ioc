using Hndy.Ioc.Tests.NamedInstances;
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
    class NamedInstancesTests
    {
        [Test]
        public void TestNamedInstances()
        {
            var container = new IocContainer(new NamedInstancesRegistration());
            Assert.That(container.Get<Bar>(), Is.TypeOf<Bar1>());
            Assert.That(container.Get<Bar>(default(int?)), Is.TypeOf<Bar1>());
            Assert.That(container.TryGet<Bar>(0), Is.Null);
            Assert.That(container.Get<IBar>(), Is.TypeOf<RedBar>());
            Assert.That(container.Get<IBar>("red"), Is.TypeOf<RedBar>());
            Assert.That(container.Get<IBar>("red"), Is.Not.SameAs(container.Get<IBar>()));
            Assert.That(container.Get<IBar>("green"), Is.TypeOf<GreenBar>());
            Assert.That(container.Get<IBar>("blue"), Is.TypeOf<BlueBar>());
            Assert.That(container.Get<IBar>(1), Is.TypeOf<Bar1>());
            Assert.That(((BarN)container.Get<IBar>(20)).Num, Is.EqualTo(20));
            Assert.That(container.Get<BarN>(23).Num, Is.EqualTo(23));
        }

        [Test] 
        public void TestNamedSingleton()
        {
            using var container = new IocContainer(new SingletonRegistration());

            Assert.Throws<IocUnregisteredException>(() => container.Get<Foo>());
            Assert.That(container.Get<Foo>("x").Name, Is.EqualTo("_x"));
            Assert.That(container.Get<Foo>("y").Name, Is.EqualTo("_y"));
            Assert.That(container.Get<Foo>("yy").Name, Is.EqualTo("_y"));
            Assert.That(container.Get<Foo>("z").Name, Is.EqualTo("z"));
            Assert.That(container.Get<IFoo>("x").Name, Is.EqualTo("x"));
            Assert.That(container.Get<IFoo>("abc").Name, Is.EqualTo("abc"));
            Assert.That(container.Get<Foo>("x"), Is.SameAs(container.Get<Foo>("x")));
            Assert.That(container.Get<Foo>("y"), Is.SameAs(container.Get<Foo>("y")));
            Assert.That(container.Get<Foo>("yy"), Is.SameAs(container.Get<Foo>("y")));
            Assert.That(container.Get<Foo>("z"), Is.SameAs(container.Get<Foo>("z")));
            Assert.That(container.Get<IFoo>("z"), Is.SameAs(container.Get<IFoo>("z")));
            Assert.That(container.Get<IFoo>("z"), Is.Not.SameAs(container.Get<Foo>("z")));

            Assert.Throws<IocUnregisteredException>(() => container.Get<BarN>("2"));
            Assert.That(container.Get<BarN>(2).Num, Is.EqualTo(2));
            Assert.That(container.Get<BarN>(2.8f).Num, Is.EqualTo(2));
            Assert.That(container.Get<BarN>(2.0d).Num, Is.EqualTo(2));
            Assert.That(container.Get<BarN>(2), Is.SameAs(container.Get<BarN>(2)));
            Assert.That(container.Get<BarN>(2f), Is.SameAs(container.Get<BarN>(2f)));
            Assert.That(container.Get<BarN>(2f), Is.SameAs(container.Get<BarN>(2)));
            Assert.That(container.Get<BarN>(2.0), Is.Not.SameAs(container.Get<BarN>(2)));

            Assert.That(container.Get<Foobar>(9).Foo.Name, Is.EqualTo("9"));
            Assert.That(((BarN)container.Get<Foobar>(9).Bar).Num, Is.EqualTo(18));
            Assert.That(container.Get<Foobar>(0).Foo.Name, Is.EqualTo(""));
            Assert.That(((BarN)container.Get<Foobar>(0).Bar).Num, Is.EqualTo(0));
            Assert.That(container.Get<Foobar>(0), Is.SameAs(container.Get<Foobar>(0)));
            Assert.That(container.Get<Foobar>(9), Is.SameAs(container.Get<Foobar>(9)));
        }

        [Test]
        public void TestNamedScoped()
        {
            using var scope = new IocContainer(new ScopedRegistration()).NewScope();

            Assert.Throws<IocUnregisteredException>(() => scope.Get<Foo>());
            Assert.That(scope.Get<Foo>("x").Name, Is.EqualTo("_x"));
            Assert.That(scope.Get<Foo>("y").Name, Is.EqualTo("_y"));
            Assert.That(scope.Get<Foo>("yy").Name, Is.EqualTo("_y"));
            Assert.That(scope.Get<Foo>("z").Name, Is.EqualTo("z"));
            Assert.That(scope.Get<IFoo>("x").Name, Is.EqualTo("x"));
            Assert.That(scope.Get<IFoo>("abc").Name, Is.EqualTo("abc"));
            Assert.That(scope.Get<Foo>("x"), Is.SameAs(scope.Get<Foo>("x")));
            Assert.That(scope.Get<Foo>("y"), Is.SameAs(scope.Get<Foo>("y")));
            Assert.That(scope.Get<Foo>("yy"), Is.SameAs(scope.Get<Foo>("y")));
            Assert.That(scope.Get<Foo>("z"), Is.SameAs(scope.Get<Foo>("z")));
            Assert.That(scope.Get<IFoo>("z"), Is.SameAs(scope.Get<IFoo>("z")));
            Assert.That(scope.Get<IFoo>("z"), Is.Not.SameAs(scope.Get<Foo>("z")));

            Assert.Throws<IocUnregisteredException>(() => scope.Get<BarN>("2"));
            Assert.That(scope.Get<BarN>(2).Num, Is.EqualTo(2));
            Assert.That(scope.Get<BarN>(2.8f).Num, Is.EqualTo(2));
            Assert.That(scope.Get<BarN>(2.0d).Num, Is.EqualTo(2));
            Assert.That(scope.Get<BarN>(2), Is.SameAs(scope.Get<BarN>(2)));
            Assert.That(scope.Get<BarN>(2f), Is.SameAs(scope.Get<BarN>(2f)));
            Assert.That(scope.Get<BarN>(2f), Is.SameAs(scope.Get<BarN>(2)));
            Assert.That(scope.Get<BarN>(2.0), Is.Not.SameAs(scope.Get<BarN>(2)));

            Assert.That(scope.Get<Foobar>(9).Foo.Name, Is.EqualTo("9"));
            Assert.That(((BarN)scope.Get<Foobar>(9).Bar).Num, Is.EqualTo(18));
            Assert.That(scope.Get<Foobar>(0).Foo.Name, Is.EqualTo(""));
            Assert.That(((BarN)scope.Get<Foobar>(0).Bar).Num, Is.EqualTo(0));
            Assert.That(scope.Get<Foobar>(0), Is.SameAs(scope.Get<Foobar>(0)));
            Assert.That(scope.Get<Foobar>(9), Is.SameAs(scope.Get<Foobar>(9)));
        }

        [Test]
        public void TestNamedTransient()
        {
            using var container = new IocContainer(new TransientRegistration());

            Assert.Throws<IocUnregisteredException>(() => container.Get<Foo>());
            Assert.That(container.Get<Foo>("x").Name, Is.EqualTo("_x"));
            Assert.That(container.Get<Foo>("y").Name, Is.EqualTo("_y"));
            Assert.That(container.Get<Foo>("yy").Name, Is.EqualTo("_y"));
            Assert.That(container.Get<Foo>("z").Name, Is.EqualTo("z"));
            Assert.That(container.Get<IFoo>("x").Name, Is.EqualTo("x"));
        }
    }
}
