using System;

namespace Hndy.Ioc.Tests
{
    namespace Generic
    {
        interface IFoo { }
        interface IFoo<T1, T2> { }
        class Foo<T> : IFoo { }
        class Foo<T1, T2, T3> : IFoo, IFoo<T1, T2>
        { 
            public T3 Value3 { get; }

            public Foo(T3 value3)
            {
                Value3 = value3;
            }
        }

        partial class GenericRegistration : IocRegistration
        {
            public GenericRegistration()
            {
                Singleton<Foo<string>>();
                Singleton<Foo<int?, bool, Foo<string>?>>();
                SingletonFor<Foo<string>, string>("a").Use<Foo<string>>();
                SingletonFor<IFoo<string, int>, double>().Use<Foo<string, int, double>>();
                SingletonFor<IFoo<string, double>, double>().Use<Foo<string, double, double>>(d => new Wirers.Foo_string_double_double()
                {
                    value3 = -d
                });
            }
        }
    }
}
