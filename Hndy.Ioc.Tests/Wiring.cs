using Hndy.Ioc;
using System;

namespace Hndy.Ioc.Tests
{
    namespace Wiring
    {
        internal interface ILogger
        {
            string Name { get; }
        }
        internal interface ILogger2 { }
        class Logger : ILogger, ILogger2
        {
            public string Name { get; }
            public Logger(string name)
            {
                Name = name;
            }
        }
        class Bar
        {
            public ILogger Logger { get; }
            internal Bar(ILogger logger) // internal ctor will be used to wiring when public ctor not exists.
            {
                Logger = logger;
            }
        }
        class Cot { }
        class Foo
        {
            public Bar Bar { get; }
            public Cot Cot { get; }
            public ILogger? Logger { get; }
            public Foo(Bar bar, Cot cot, ILogger? logger)
            {
                Bar = bar;
                Cot = cot;
                Logger = logger;
            }

            internal Foo(Bar bar, Cot cot) // internal ctor won't be used to wiring when public ctor exists.
            {
                throw new NotImplementedException();
            }
        }
        class FooOptions
        {
            public int BarNum { get; set; }
            public string? Name { get; set; }
        }

        partial class ManualWiringRegistration : IocRegistration
        {
            public ManualWiringRegistration()
            {
                Transient<ILogger, string>(name => new Logger($"[{name}]"));
                Transient<Cot>();
                Singleton<Bar, int>((c, n) => new Bar(c.Get<ILogger>($"bar{n}")));
                Transient<Foo, FooOptions>((c, opt) => new Foo(
                    c.Get<Bar>(opt.BarNum),
                    c.Get<Cot>(),
                    c.TryGet<ILogger>(opt.Name)));
            }
        }

        partial class AutoWiringRegistration : IocRegistration
        {
            public AutoWiringRegistration()
            {
                TransientFor<ILogger, string>().Use<Logger>(name => new Wirers.Logger() { name = $"[{name}]" });
                TransientFor<ILogger2, string>().Use<Logger>(name => new Wirers.Logger() { name = $"[{name}]" });
                Transient<Cot>();
                SingletonFor<Bar, int>().Use<Bar>(n => new Wirers.Bar() { logger = new($"bar{n}") });
                TransientFor<Foo, FooOptions>().Use<Foo>(opt => new Wirers.HndyIocTestsWiringFoo()
                {
                    logger = new(opt.Name),
                    bar = new(opt.BarNum)
                });
                this.SingletonFor<AnotherNs.Foo>().Use(new Wirers.AnotherNsFoo() { safe = true });
            }
        }
    }
}

namespace AnotherNs
{
    class Foo 
    {
        public Foo(bool safe) { }
    }
}
