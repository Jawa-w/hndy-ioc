using Hndy.Ioc;
using NUnit.Framework;
using System.Linq;

namespace Hndy.Ioc.Gen.Tests
{
    [TestFixture]
    class DiagnosticTests
    {

        const string file = @"
        class Foo
        {
            public Foo() { }
            public Foo(bool v) { }
        }
        class Bar
        {
            internal Bar() { }
            public Bar(float v) { }
        }
        class Cot
        {
            private Cot() { }
        }
        class Dot
        {
            public Dot(string name, int v) { }
        }
        class Eot
        {
            internal Eot() { }
            internal Eot(string name, int v) { }
        }
        partial class MyReg : Hndy.Ioc.IocRegistration
        {
            public MyReg()
            {
                Singleton<Foo>();
                SingletonFor<Foo, bool>().Use(b => new Wirer.Foo() { v = b });
                this.TransientFor<Cot>().Use(() => new Wirer.Cot());
                Transient<Eot>();
                Scoped<Bar, int>();
                Scoped<Bar>();
                Transient<Dot, string>();

                ScopedFor<Bar>().Use(() => new Wirers.Bar() { v = 3.14f });
                ScopedFor<Bar>().Use(() => new Wirers.Bar());
                var temp = ScopedFor<Bar>();
                temp.Use(() => new Wirers.Bar() { v = 3.14f });
            }
        }
";

        [Test]
        public void TestDiag()
        {
            var bench = new GenBench(new IocRegGenerator());
            bench.AddReference<IocRegistration>();

            bench.Run(new[] { file });
            var diags = bench.Result!.Value.Diagnostics;
            int i = 0;
            Assert.That(diags[i++].ToString(), Does.StartWith("(29,27): error HI0001:"));
            Assert.That(diags[i++].ToString(), Does.StartWith("(30,30): error HI0001:"));
            Assert.That(diags[i++].ToString(), Does.StartWith("(31,35): error HI0002:"));
            Assert.That(diags[i++].ToString(), Does.StartWith("(32,27): error HI0002:"));
            Assert.That(diags[i++].ToString(), Does.StartWith("(33,24): error HI0003:"));
            Assert.That(diags[i++].ToString(), Does.StartWith("(34,24): warning HI1001:"));
            Assert.That(diags[i++].ToString(), Does.StartWith("(35,27): warning HI1001:"));
            Assert.That(diags[i++].ToString(), Does.StartWith("(38,48): warning HI1001:"));
            Assert.That(diags[i++].ToString(), Does.StartWith("(40,22): error HI0004:"));
            Assert.That(diags.Length, Is.EqualTo(i));
        }
    }
}
