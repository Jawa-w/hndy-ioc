using System;

namespace Hndy.Ioc.Tests
{
    namespace RegistrationOverride
    {
        interface IFoo { }
        class Foo : IFoo
        {
            public Foo(Bar bar) { }
        }
        class Foo2 : IFoo { }
        public class Bar { }

        partial class Registration1 : IocRegistration
        {
            public Registration1()
            {
                Singleton<Foo>();
                Singleton<Foo>();
                Transient<Foo>();

                SingletonFor<IFoo>().Use<Foo>();
                SingletonFor<IFoo>().Use<Foo2>();
            }
        }

        partial class Registration2 : IocRegistration
        {
            public Registration2()
            {
                Singleton<Foo>();
                SingletonFor<IFoo>().Use<Foo>();
            }
        }

        partial class Registration3 : IocRegistration
        {
            public Registration3()
            {
                Singleton<Bar>();
            }
        }
    }
}
