using Hndy.Ioc;
using System;

namespace Hndy.Ioc.Tests
{
    namespace Basic
    {
        interface IBar { }
        interface IBarr { }
        class Foo { }
        class Bar : IBar, IBarr { }
        class Barr
        {
            public Barr(Bar bar) { }
        }
        struct Bin
        {
            public int Id { get; }
            public Bin(int id)
            {
                Id = id;
            }
        }

        class Cot
        {
            public IServiceLocator ServiceLocator { get; }

            public Cot(IServiceLocator serviceLocator)
            {
                ServiceLocator = serviceLocator;
            }
        }

        class Foobar
        {
            public Foo Foo { get; }
            public IBar Bar { get; }
            public Cot Cot { get; }
            public Foobar(Foo foo, IBar bar, Cot cot)
            {
                Foo = foo;
                Bar = bar;
                Cot = cot;
            }
        }

        class Foobar2
        {
            public Foo Foo { get; set; }
            public IBar Bar { get; set; }
            public Cot Cot { get; set; }
            public Foobar Foobar { get; set; }
            public Foobar2(Foo foo, IBar bar, Cot cot, Foobar foobar)
            {
                Foo = foo;
                Bar = bar;
                Cot = cot;
                Foobar = foobar;
            }
        }

        partial class BasicRegistration : IocRegistration
        {
            public BasicRegistration()
            {
                Singleton<Foo>();
                TransientFor<IBar>().Use<Bar>();
                Scoped<Cot>();
                Transient<Foobar>();
                Transient<Foobar2>();
                Transient<Barr>();
            }
        }

        partial class SingletonRegistration : IocRegistration
        {
            public SingletonRegistration()
            {
                Singleton(new Foo());
                Singleton(() => new Bar());
                Singleton<IBar>(() => new Bar());
                Singleton<IBarr>(() => new Bar());
                Singleton(x => new Cot(x));
                Singleton(x => new Foobar(x.Get<Foo>(), x.Get<IBar>(), x.Get<Cot>()));
                Singleton(x => new Foobar2(new Foo(), x.Get<Bar>(), new Cot(x), x.Get<Foobar>()));

                Singleton((Bin?)new Bin(7));
                Singleton(5, (Bin?)new Bin(4));
                Singleton((int?)5, new Bin(5));
                Singleton(default(int?), (Bin?)new Bin(9));
                Singleton((int?)20, () => new Bin(20));
                Singleton((int?)21, () => default(Bin?));
                Singleton((int?)22, c => new Bin(22));
                Singleton((int?)23, c => default(Bin?));
            }
        }

        partial class ScopedRegistration : IocRegistration
        {
            public ScopedRegistration()
            {
                Scoped(() => new Foo());
                Scoped(() => new Bar());
                ScopedFor<IBar>().Use<Bar>();
                ScopedFor<IBarr, int>(null).Use<Bar>();
                Scoped(x => new Cot(x));
                Scoped(x => new Foobar(x.Get<Foo>(), x.Get<IBar>(), x.Get<Cot>()));
                Scoped(x => new Foobar2(new Foo(), x.Get<Bar>(), x.Get<Cot>(), x.Get<Foobar>()));

                Scoped(x => default(Bin?));
                Scoped((int?)20, () => new Bin(20));
                Scoped((int?)21, () => default(Bin?));
                Scoped((int?)22, c => new Bin(22));
                Scoped((int?)23, c => default(Bin?));
            }
        }

        partial class TransientRegistration : IocRegistration
        {
            public TransientRegistration()
            {
                var foo = new Foo();
                Transient<IBar>(() => new Bar());
                TransientFor<IBarr, int>(null).Use<Bar>();
                Transient(x => new Cot(x));
                Transient(x => new Foobar(foo, x.Get<IBar>(), x.Get<Cot>()));
                Transient(x => new Foobar2(foo, new Bar(), x.Get<Cot>(), x.Get<Foobar>()));

                Transient(x => default(Bin?));
                Transient((int?)20, () => new Bin(20));
                Transient((int?)21, () => default(Bin?));
                Transient((int?)22, c => new Bin(22));
                Transient((int?)23, c => default(Bin?));
            }
        }
    }
}
