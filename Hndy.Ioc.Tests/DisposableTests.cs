using Hndy.Ioc.Tests.Disposable;
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
    class DisposableTests
    {
        [Test]
        public void TestDisposable()
        {
            var container = new IocContainer(new DisposableRegistration());

            var bar = container.Get<Bar>();
            var fb = container.Get<Foobar>();
            Assert.That(fb.Disposed, Is.False);

            var scope = container.NewScope();
            var foo1 = scope.Get<Foo>();
            var bar1 = scope.Get<Bar>();
            var fb1 = scope.Get<Foobar>();
            var fooa = scope.Get<Foo>("a");
            var foob = scope.Get<Foo>("b");
            scope.Dispose();

            Assert.That(foo1.Disposed, Is.False);
            Assert.That(bar1.Disposed, Is.False);
            Assert.That(fb1.Disposed, Is.True);
            Assert.That(fb1.Foo.Disposed, Is.False);
            Assert.That(bar.Disposed, Is.False);
            Assert.That(fb.Disposed, Is.False);
            Assert.That(fooa.Disposed, Is.False);
            Assert.That(foob.Disposed, Is.True);

            container.Dispose();
            Assert.That(foo1.Disposed, Is.True);
            Assert.That(bar1.Disposed, Is.False);
            Assert.That(bar.Disposed, Is.False);
            Assert.That(fb.Disposed, Is.False);
            Assert.That(fb.Foo.Disposed, Is.True);
            Assert.That(fb1.Foo.Disposed, Is.True);
            Assert.That(fooa.Disposed, Is.True);
        }
    }
}
