using System;
using System.Collections.Generic;

namespace Hndy.Ioc.Tests
{
    namespace InjectSingletons
    {
        class Foo
        {
            public Foo(int v)
            {
                Value = v;
            }

            public int Value { get; }
        }
        class Fee
        {
            public Fee(int v)
            {
                Value = v;
            }

            public int Value { get; }
        }
        struct Bar
        {
            public Bar(int v)
            {
                Value = v;
            }

            public int Value { get; set; }
        }

        partial class InjectSingletonsRegistration : IocRegistration
        {
            public InjectSingletonsRegistration()
            {
                Singleton(new Foo(0));
                Singleton(99, new Foo(-1));
                Singleton<Foo, int>(n => new Foo(n));
            }
        }
    }
}
