using Hndy.Ioc.Tests.ChildContainers;
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
    class ChildContainersTests
    {
        [Test]
        public void TestChildContainer()
        {
            var c0 = new IocContainer(new ChildContainersRegistration0());
            var c1 = new IocContainer(c0, new ChildContainersRegistration1());
            var c2 = new IocContainer(c1, new ChildContainersRegistration2());

            Assert.That(c1.Get<Cot>(), Is.SameAs(c1.Get<Cot>()));
            Assert.That(c2.Get<Cot>(), Is.Not.SameAs(c2.Get<Cot>()));
            Assert.That(c2.Get<Cot>(), Is.Not.SameAs(c1.Get<Cot>()));
            Assert.That(c2.Get<Dot>(), Is.Not.SameAs(c1.Get<Dot>()));
            Assert.That(c2.Get<Foo2>(), Is.SameAs(c1.Get<Foo2>()));

            Assert.That(c2.Get<Bar2>(), Is.Not.SameAs(c2.Get<Bar2>()));
            Assert.That(c2.Get<Bar2>().Foo, Is.SameAs(c2.Get<Bar2>().Foo));
            Assert.That(c2.Get<Bar2>().Cot, Is.Not.SameAs(c2.Get<Bar2>().Cot));
            Assert.That(c2.Get<Bar1>(), Is.SameAs(c2.Get<Bar1>()));

            Assert.That(c2.TryGet<IFoobar>(), Is.Null);
            Assert.That(c2.TryGet<IFoobar>(0), Is.Null);
            Assert.That(c2.TryGet<IFoobar>(1), Is.Not.Null);
        }
    }
}
