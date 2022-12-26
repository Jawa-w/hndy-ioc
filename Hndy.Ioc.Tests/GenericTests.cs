using Hndy.Ioc.Tests.Generic;
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
    class GenericTests
    {
        [Test]
        public void TestGeneric()
        {
            var container = new IocContainer(new GenericRegistration());
            Assert.That(container.Get<Foo<string>>(),
                Is.SameAs(container.Get<Foo<int?, bool, Foo<string>?>>().Value3));
            Assert.That(container.Get<Foo<string>>("a"),
                Is.Not.SameAs(container.Get<Foo<string>>()));
            Assert.That(((Foo<string, int, double>)container.Get<IFoo<string, int>>(2.1)).Value3, Is.EqualTo(2.1));
            Assert.That(((Foo<string, double, double>)container.Get<IFoo<string, double>>(0.7)).Value3, Is.EqualTo(-0.7));
        }
    }
}
