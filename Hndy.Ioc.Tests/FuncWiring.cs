using System;

namespace Hndy.Ioc.Tests
{
    namespace FuncWiring
    {
        class Foo
        {
            public string Name { get; }
            public Foo(string name)
            {
                Name = name;
            }
        }
        class Bar { }
        struct Bin { }

        class Foobar1
        {
            private readonly Func<string, Foo> getFoo;
            private readonly Func<Bar> getBar;

            public Foobar1(Func<string, Foo> getFoo, Func<Bar> getBar)
            {
                this.getFoo = getFoo;
                this.getBar = getBar;
            }

            public Foo GetFoo(string name) => getFoo(name);
            public Bar GetBar() => getBar();
        }

        class Foobar2
        {
            public Foobar2(Func<Bar?> getBar, Func<Bin?> getBin)
            {
                GetBar = getBar;
                GetBin = getBin;
            }

            public Func<Bar?> GetBar { get; }
            public Func<Bin?> GetBin { get; }
        }

        class Foobar3
        {
            public Foobar3(Func<Foo> getFoo, Func<Bar>? getBar)
            {
                GetFoo = getFoo;
                GetBar = getBar;
            }

            public Func<Foo> GetFoo { get; }
            public Func<Bar>? GetBar { get; }
        }

        class Foobar4
        {
            private readonly Func<string, Foo?> tryGetFoo1;
            private readonly Func<string, Foo>? tryGetFoo2;
            private readonly string name;

            public Foobar4(Func<string, Foo?> tryGetFoo1, Func<string, Foo>? tryGetFoo2, string name)
            {
                this.tryGetFoo1 = tryGetFoo1;
                this.tryGetFoo2 = tryGetFoo2;
                this.name = name;
            }

            public Foo? TryGetFoo1() => tryGetFoo1(name);
            public Foo? TryGetFoo2() => tryGetFoo2?.Invoke(name);
        }

        class Foobar5
        {
            public Foobar5(Func<Func<string, Foo>> getGetFoo1, Func<string, Func<Foo>> getGetFoo2)
            {
                GetGetFoo1 = getGetFoo1;
                GetGetFoo2 = getGetFoo2;
            }

            public Func<Func<string, Foo>> GetGetFoo1 { get; }
            public Func<string, Func<Foo>> GetGetFoo2 { get; }
        }

        partial class FuncWiringRegistration : IocRegistration
        {
            public FuncWiringRegistration()
            {
                Transient<Foo, string>();
                Transient<Bar>();
                Transient<Bin>();
                Singleton<Foobar1>();
                Singleton<Foobar2>();
                SingletonFor<Foobar3>().Use(new Wirers.Foobar3() { getFoo = new("") });
                SingletonFor<Foobar3, string>("err").Use(new Wirers.Foobar3() { getBar = null });
                SingletonFor<Foobar3, string>().Use(name => new Wirers.Foobar3() { getFoo = new(name) });
                SingletonFor<Foobar4, string>().Use(n => new Wirers.Foobar4() { name = n.ToUpperInvariant() });
            }
        }

        partial class NullableFuncWiringRegistration : IocRegistration
        {
            public NullableFuncWiringRegistration()
            {
                Singleton<Foobar1>();
                Singleton<Foobar2>();
                SingletonFor<Foobar3>().Use(new Wirers.Foobar3() { getFoo = (Func<Foo>)(() => new Foo("5")) });
                Singleton<Foobar4, string>();
            }
        }

        partial class NestedFuncWiringRegistration : IocRegistration
        {
            public NestedFuncWiringRegistration()
            {
                Transient<Foo, string>();
                Transient<Func<Foo>, string>(name => () => new Foo(name.ToUpperInvariant()));
                Singleton<Foobar5>();
            }
        }
    }
}

