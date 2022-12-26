using System;

namespace Hndy.Ioc.Tests
{
    namespace ChildContainers
    {
        class Cot { }
        class Dot { }
        class Foo1 { }
        class Foo2 { }
        class Bar1
        {
            public Foo1 Foo { get; }
            public Cot Cot { get; }

            public Bar1(Foo1 foo, Cot cot)
            {
                Foo = foo;
                Cot = cot;
            }
        }
        class Bar2
        {
            public Foo2 Foo { get; }
            public Cot Cot { get; }

            public Bar2(Foo2 foo, Cot cot)
            {
                Foo = foo;
                Cot = cot;
            }
        }
        interface IFoobar { }
        class Foobar : IFoobar { }

        partial class ChildContainersRegistration0 : IocRegistration
        {
            public ChildContainersRegistration0()
            {
                Transient(() => default(IFoobar));
                Transient(0, () => default(IFoobar));
                Transient<IFoobar, int>(1, () => new Foobar());
            }
        }

        partial class ChildContainersRegistration1 : IocRegistration
        {
            public ChildContainersRegistration1()
            {
                Singleton<Foo2>();
                Singleton<Bar1>();
                Singleton<Cot>();
                Singleton<Dot>();
            }
        }

        partial class ChildContainersRegistration2 : IocRegistration
        {
            public ChildContainersRegistration2()
            {
                Transient<Foo1>();
                Transient<Bar2>();
                Transient<Cot>();
                Singleton<Dot>();
            }
        }
    }
}
