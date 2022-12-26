using Hndy.Ioc.Tests.Nullable;
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
    class NullableTests
    {
        [Test]
        public void TestNullInstances_ShouldBeIgnored()
        {
            var container = new IocContainer(new NullInstancesRegistration());
            Assert.IsNotNull(container.Get<Foo>());
            Assert.IsNotNull(container.Get<Far>());
        }

        [Test]
        public void TestFactoriesReturningNull_ShouldNotBeInvokedAgain()
        {
            var container = new IocContainer(new NullFactoriesRegistration());
            Assert.IsNull(container.TryGet<Foo>());
            Assert.IsNull(container.TryGet<Foo>());
            Assert.IsNull(container.TryGet<Foo>(""));
            Assert.IsNull(container.TryGet<Foo>(""));
            Assert.IsNull(container.TryGet<Foo>("a"));
            Assert.IsNotNull(container.TryGet<Foo>("b"));
            Assert.IsNull(container.TryGet<Foo>("a"));

            var scope = container.NewScope();
            Assert.IsNull(scope.TryGet<Fee>());
            Assert.IsNull(scope.TryGet<Fee>());
            Assert.IsNull(scope.TryGet<Fee>(""));
            Assert.IsNull(scope.TryGet<Fee>(""));
            Assert.IsNull(scope.TryGet<Fee>("a"));
            Assert.IsNotNull(scope.TryGet<Fee>("b"));
            Assert.IsNull(scope.TryGet<Fee>("a"));
        }

        [Test]
        public void TestWiringWithNullable()
        {
            var container = new IocContainer(new WiringRegistration());

            Cot cot = container.Get<Cot>();
            Assert.IsNull(cot.Foo);
            Assert.IsNotNull(cot.Far);
            Assert.IsNull(cot.Bar);

            Cot red = container.Get<Cot>("red");
            Assert.IsNotNull(red.Foo);
            Assert.IsNull(red.Far);
            Assert.IsNull(red.Bar);

            Cot blue = container.Get<Cot>("blue");
            Assert.IsNull(blue.Foo);
            Assert.IsNull(blue.Far);
            Assert.IsNotNull(blue.Bar);
        }

        [Test]
        public void TestSingletonWithNullable()
        {
            var container = new IocContainer(new SingletonRegistration());
            Assert.IsFalse(container.TryGet<Foo>(out _));
            Assert.IsFalse(container.TryGet<Far>(out _));
            Assert.IsFalse(container.TryGet<Fee>(out _));
            Assert.IsFalse(container.TryGet<Bar>(out _));
            Assert.IsFalse(container.TryGet<Car>(out _));
            Assert.IsFalse(container.TryGet<Foo>(0, out _));
            Assert.IsFalse(container.TryGet<Far>(0, out _));
            Assert.IsFalse(container.TryGet<Fee>(0, out _));
            Assert.IsFalse(container.TryGet<Bar>(0, out _));
            Assert.IsFalse(container.TryGet<Fee>(false, out _));
            Assert.IsFalse(container.TryGet<Bar>(false, out _));
            Assert.IsFalse(container.TryGet<Foo>("", out _));
            Assert.IsFalse(container.TryGet<Far>("", out _));
            Assert.IsFalse(container.TryGet<Fee>("", out _));
            Assert.IsFalse(container.TryGet<Bar>("", out _));
        }

        [Test]
        public void TestScopedWithNullable()
        {
            var scope = new IocContainer(new ScopedRegistration()).NewScope();
            Assert.IsFalse(scope.TryGet<Fee>(out _));
            Assert.IsFalse(scope.TryGet<Bar>(out _));
            Assert.IsFalse(scope.TryGet<Fee>(0, out _));
            Assert.IsFalse(scope.TryGet<Bar>(0, out _));
            Assert.IsFalse(scope.TryGet<Fee>(false, out _));
            Assert.IsFalse(scope.TryGet<Bar>(false, out _));
            Assert.IsFalse(scope.TryGet<Foo>("", out _));
            Assert.IsFalse(scope.TryGet<Far>("", out _));
            Assert.IsFalse(scope.TryGet<Fee>("", out _));
            Assert.IsFalse(scope.TryGet<Bar>("", out _));
        }

        [Test]
        public void TestTransientWithNullable()
        {
            var container = new IocContainer(new TransientRegistration());
            Assert.IsFalse(container.TryGet<Fee>(out _));
            Assert.IsFalse(container.TryGet<Bar>(out _));
            Assert.IsFalse(container.TryGet<Fee>(0, out _));
            Assert.IsFalse(container.TryGet<Bar>(0, out _));
            Assert.IsFalse(container.TryGet<Fee>(false, out _));
            Assert.IsFalse(container.TryGet<Bar>(false, out _));
            Assert.IsFalse(container.TryGet<Foo>("", out _));
            Assert.IsFalse(container.TryGet<Far>("", out _));
            Assert.IsFalse(container.TryGet<Fee>("", out _));
            Assert.IsFalse(container.TryGet<Bar>("", out _));
        }
    }
}
