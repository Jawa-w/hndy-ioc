using System;

namespace Hndy.Ioc.Tests
{
    namespace NamedInstances
    {
        interface IBar { }
        class Bar : IBar { }
        class RedBar : IBar { }
        class GreenBar : IBar { }
        class BlueBar : IBar { }
        class Bar1 : Bar, IBar { }
        class BarN : IBar 
        {
            public int Num { get; }
            public BarN(int num)
            {
                Num = num;
            }
        }

        internal interface IFoo
        {
            string Name { get; }
        }
        class Foo : IFoo
        {
            public string Name { get; }
            public Foo(string name)
            {
                Name = name;
            }
        }
        class Foobar
        {
            public Foo Foo { get; }
            public IBar Bar { get; }
            public Foobar(Foo foo, IBar bar)
            {
                Foo = foo;
                Bar = bar;
            }
        }

        partial class NamedInstancesRegistration : IocRegistration
        {
            public NamedInstancesRegistration()
            {
                Singleton<Bar>();
                SingletonFor<Bar, int>(default(int?)).Use<Bar1>();
                SingletonFor<IBar>().Use<Bar>();
                SingletonFor<IBar, string>(null).Use<RedBar>();

                SingletonFor<IBar, string>("red").Use<RedBar>();
                SingletonFor<IBar, string>("green").Use<GreenBar>();
                ScopedFor<IBar, string>("blue").Use<BlueBar>();
                SingletonFor<IBar, int>(1).Use<Bar1>();

                Singleton<IBar, int>(n => new BarN(n));
                Singleton<BarN, int>();
            }
        }

        partial class SingletonRegistration : IocRegistration
        {
            public SingletonRegistration()
            {
                Singleton<Foo, string>();
                SingletonFor<IFoo, string>().Use<Foo>();
                Singleton("x", new Foo("_x"));
                Singleton("y", () => new Foo("_y"));
                Singleton("yy", s => s.Get<Foo>("y"));
                Singleton<BarN, int>(n => new BarN(n));
                Singleton<BarN, float>((s, f) => s.Get<BarN>((int)f));
                Singleton<BarN, double>(d => new BarN((int)d));
                SingletonFor<IBar, int>().Use<BarN>();
                SingletonFor<Foobar, int>(0).Use<Foobar>(new Wirers.Foobar()
                {
                    foo = new(""),
                    bar = new(0)
                });
                SingletonFor<Foobar, int>().Use<Foobar>(id => new Wirers.Foobar()
                {
                    foo = new(id.ToString()),
                    bar = new(Math.Abs(id + id))
                });
            }
        }

        partial class ScopedRegistration : IocRegistration
        {
            public ScopedRegistration()
            {
                Scoped<Foo, string>();
                ScopedFor<IFoo, string>().Use<Foo>();
                Scoped("x", () => new Foo("_x"));
                Scoped("y", () => new Foo("_y"));
                Scoped("yy", s => s.Get<Foo>("y"));
                Scoped<BarN, int>(n => new BarN(n));
                Scoped<BarN, float>((s, f) => s.Get<BarN>((int)f));
                Scoped<BarN, double>(d => new BarN((int)d));
                ScopedFor<IBar, int>().Use<BarN>();
                ScopedFor<Foobar, int>(0).Use<Foobar>(new Wirers.Foobar()
                {
                    foo = new(""),
                    bar = new(0)
                });
                ScopedFor<Foobar, int>().Use<Foobar>(id => new Wirers.Foobar()
                {
                    foo = new(id.ToString()),
                    bar = new(Math.Abs(id + id))
                });
            }
        }

        partial class TransientRegistration : IocRegistration
        {
            public TransientRegistration()
            {
                Transient<Foo, string>();
                TransientFor<IFoo, string>().Use<Foo>();
                Transient("x", () => new Foo("_x"));
                Transient("y", () => new Foo("_y"));
                Transient("yy", s => s.Get<Foo>("y"));
            }
        }
    }
}
