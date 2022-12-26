using Hndy.Ioc.Tests.Basic;
using NUnit.Framework;
using static System.Formats.Asn1.AsnWriter;

namespace Hndy.Ioc.Tests
{
    [TestFixture]
    class BasicTests
    {
        [Test]
        public void TestGetInstance()
        {
            using var container = new IocContainer(new BasicRegistration());

            Assert.That(container.Get<IBar>(), Is.TypeOf<Bar>());
            Assert.That(container.TryGet<IBar>(), Is.TypeOf<Bar>());
            Assert.That(container.TryGet<IBar>(out var ibar), Is.True);
            Assert.That(ibar, Is.TypeOf<Bar>());

            Assert.Throws<IocUnregisteredException>(() => container.Get<Bar>());
            Assert.That(container.TryGet<Bar>(), Is.Null);
            Assert.That(container.TryGet<Bar>(out var bar), Is.False);
            Assert.That(bar, Is.Null);

            Assert.Throws<IocUnregisteredException>(() => container.Get<Barr>());
            Assert.That(() => container.TryGet<Barr>(), Throws.InstanceOf<IocUnregisteredException>().And.Message.Contains(".Bar'"));
        }

        [Test]
        public void TestDefaultRegistrations()
        {
            using var container = new IocContainer(new BasicRegistration());
            Assert.That(container.Get<IServiceLocator>(), Is.Not.Null);
            Assert.That(container.Get<IIocContainer>(), Is.Not.Null);
            Assert.That(container.Get<IIocScope>(), Is.Not.Null);

            Assert.That(container.Get<IServiceLocator>(), Is.SameAs(container.Get<IIocContainer>()));
            Assert.That(container.Get<IIocContainer>(), Is.SameAs(container.Get<IIocContainer>()));
            Assert.That(container.Get<Cot>().ServiceLocator, Is.SameAs(container.Get<IServiceLocator>()));
            Assert.That(container.Get<IIocScope>(), Is.Not.SameAs(container.Get<IIocScope>()));
        }

        [Test]
        public void TestLifecycle()
        {
            using var container = new IocContainer(new BasicRegistration());
            Assert.That(container.Get<Foo>(), Is.SameAs(container.Get<Foo>()));
            Assert.That(container.Get<IBar>(), Is.Not.SameAs(container.Get<IBar>()));
            Assert.That(container.Get<Cot>(), Is.Not.SameAs(container.Get<Cot>()));

            using var scope = container.NewScope();
            Assert.That(scope.Get<Foo>(), Is.SameAs(scope.Get<Foo>()));
            Assert.That(scope.Get<IBar>(), Is.Not.SameAs(scope.Get<IBar>()));
            Assert.That(scope.Get<Cot>(), Is.SameAs(scope.Get<Cot>()));

            Assert.That(scope.Get<Foo>(), Is.SameAs(container.Get<Foo>()));
            Assert.That(scope.Get<IBar>(), Is.Not.SameAs(container.Get<IBar>()));
            Assert.That(scope.Get<Cot>(), Is.Not.SameAs(container.Get<Cot>()));

            var fb2 = container.Get<Foobar2>();
            Assert.That(fb2.Foobar.Foo, Is.SameAs(fb2.Foo));
            Assert.That(fb2.Foobar.Foo, Is.SameAs(container.Get<Foobar>().Foo));
            Assert.That(fb2.Foobar.Bar, Is.Not.SameAs(fb2.Bar));
            Assert.That(fb2.Foobar.Bar, Is.Not.SameAs(container.Get<IBar>()));
            Assert.That(fb2.Foobar.Cot, Is.SameAs(fb2.Cot));
            Assert.That(fb2.Foobar.Cot, Is.Not.SameAs(container.Get<Cot>()));
            Assert.That(fb2.Cot, Is.Not.SameAs(container.Get<Foobar2>().Cot));

            Assert.That(scope.Get<Foobar2>().Foobar.Cot, Is.SameAs(scope.Get<Cot>()));
            Assert.That(scope.Get<Foobar2>().Foobar.Cot, Is.SameAs(scope.Get<Foobar>().Cot));
        }

        [Test]
        public void TestSingleton()
        {
            var container = new IocContainer(new SingletonRegistration());
            Assert.That(container.Get<Foo>(), Is.SameAs(container.Get<Foo>()));
            Assert.That(container.Get<Bar>(), Is.SameAs(container.Get<Bar>()));
            Assert.That(container.Get<IBar>(), Is.SameAs(container.Get<IBar>()));
            Assert.That(container.Get<IBarr>(), Is.Not.SameAs(container.Get<IBar>()));
            Assert.That(container.Get<Bar>(), Is.Not.SameAs(container.Get<IBar>()));
            Assert.That(container.Get<Cot>(), Is.SameAs(container.Get<Cot>()));
            Assert.That(container.Get<Foobar>(), Is.SameAs(container.Get<Foobar>()));
            Assert.That(container.Get<Foobar2>(), Is.SameAs(container.Get<Foobar2>()));
            Assert.That(container.Get<Foobar2>().Bar, Is.Not.SameAs(container.Get<Foobar2>().Foobar.Bar));

            Assert.That(container.Get<Bin>(), Is.EqualTo(container.Get<Bin>()));
            Assert.That(container.Get<Bin>().Id, Is.EqualTo(9));
            Assert.That(container.Get<Bin>(5).Id, Is.EqualTo(5));
            Assert.That(container.Get<Bin>(20).Id, Is.EqualTo(20));
            Assert.That(container.TryGetNullable<Bin>(21), Is.Null);
            Assert.That(container.Get<Bin>(22).Id, Is.EqualTo(22));
            Assert.That(container.TryGetNullable<Bin>(23), Is.Null);
        }

        [Test]
        public void TestScoped()
        {
            var scope = new IocContainer(new ScopedRegistration()).NewScope();
            Assert.That(scope.Get<Foo>(), Is.SameAs(scope.Get<Foo>()));
            Assert.That(scope.Get<Bar>(), Is.SameAs(scope.Get<Bar>()));
            Assert.That(scope.Get<IBar>(), Is.SameAs(scope.Get<IBar>()));
            Assert.That(scope.Get<IBarr>(), Is.SameAs(scope.Get<IBarr>()));
            Assert.That(scope.Get<Bar>(), Is.Not.SameAs(scope.Get<IBar>()));
            Assert.That(scope.Get<Bar>(), Is.Not.SameAs(scope.Get<IBarr>()));
            Assert.That(scope.Get<Cot>(), Is.SameAs(scope.Get<Cot>()));
            Assert.That(scope.Get<Foobar>(), Is.SameAs(scope.Get<Foobar>()));
            Assert.That(scope.Get<Foobar2>(), Is.SameAs(scope.Get<Foobar2>()));
            Assert.That(scope.Get<Foobar2>().Bar, Is.Not.SameAs(scope.Get<Foobar2>().Foobar.Bar));

            Assert.That(scope.TryGetNullable<Bin>(), Is.Null);
            Assert.That(scope.Get<Bin>(20).Id, Is.EqualTo(20));
            Assert.That(scope.TryGetNullable<Bin>(21), Is.Null);
            Assert.That(scope.Get<Bin>(22).Id, Is.EqualTo(22));
            Assert.That(scope.TryGetNullable<Bin>(23), Is.Null);
        }

        [Test]
        public void TestTransient()
        {
            var container = new IocContainer(new TransientRegistration());
            Assert.That(container.Get<IBar>(), Is.Not.SameAs(container.Get<IBar>()));
            Assert.That(container.Get<IBarr>(), Is.Not.SameAs(container.Get<IBarr>()));
            Assert.That(container.Get<Cot>(), Is.Not.SameAs(container.Get<Cot>()));
            Assert.That(container.Get<Foobar>(), Is.Not.SameAs(container.Get<Foobar>()));
            Assert.That(container.Get<Foobar2>(), Is.Not.SameAs(container.Get<Foobar2>()));
            Assert.That(container.Get<Foobar2>().Foobar, Is.Not.SameAs(container.Get<Foobar2>().Foobar));
            Assert.That(container.Get<Foobar2>().Bar, Is.Not.SameAs(container.Get<Foobar2>().Bar));

            Assert.That(container.TryGetNullable<Bin>(), Is.Null);
            Assert.That(container.Get<Bin>(20).Id, Is.EqualTo(20));
            Assert.That(container.TryGetNullable<Bin>(21), Is.Null);
            Assert.That(container.Get<Bin>(22).Id, Is.EqualTo(22));
            Assert.That(container.TryGetNullable<Bin>(23), Is.Null);
        }
    }
}
