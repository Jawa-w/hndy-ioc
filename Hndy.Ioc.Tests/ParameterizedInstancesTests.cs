using Hndy.Ioc.Tests.ParameterizedInstances;
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
    class ParameterizedInstancesTests
    {
        [Test]
        public void TestParameterizedInstances()
        {
            var container = new IocContainer(new ParameterizedInstancesRegistration());
            Cot cot = container.Get<Cot>();
            var opt = container.Get<CotOptions>();
            Assert.That(cot.Foo.Name, Is.EqualTo(opt.FooName));
            Assert.That(cot.Bin.Id, Is.EqualTo(opt.BinId));
            Assert.That(cot.Time, Is.EqualTo(opt.Time));
            Assert.That(container.Get<Cot>(opt), Is.Not.SameAs(cot));
        }
    }
}
