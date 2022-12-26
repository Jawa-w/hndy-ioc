using System;

namespace Hndy.Ioc.Tests
{
    namespace CircularDependencies
    {
        class Foo
        {
            public Foo(Bar bar) { }
        }
        class Bar
        {
            public Bar(Cot cot) { }
        }
        class Cot
        {
            public Cot(Foo foo) { }
        }

        partial class CircularDependenciesRegistration : IocRegistration
        {
            public CircularDependenciesRegistration()
            {
                Singleton<Foo>();
                Singleton<Bar>();
                Singleton<Cot>();
                TransientFor<Foo, int>(0).Use<Foo>(new Wirers.Foo() { bar = new(0) });
                TransientFor<Bar, int>(0).Use<Bar>(new Wirers.Bar() { cot = new(0) });
                TransientFor<Cot, int>(0).Use<Cot>(new Wirers.Cot() { foo = new(0) });
            }
        }
    }
}
