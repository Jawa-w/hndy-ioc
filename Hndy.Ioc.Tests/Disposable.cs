using System;

namespace Hndy.Ioc.Tests
{
    namespace Disposable
    {
        class Foo : IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }
        class Bar : Foo
        {
        }

        class Foobar : Foo
        {
            public Foo Foo { get; }

            public Foobar(Foo foo)
            {
                Foo = foo;
            }
        }

        partial class DisposableRegistration : IocRegistration
        {
            public DisposableRegistration()
            {
                Singleton<Foo>();
                Transient<Bar>();
                Scoped<Foobar>();
                Singleton("a", new Foo());
                Scoped("b", () => new Foo());
            }
        }
    }
}

