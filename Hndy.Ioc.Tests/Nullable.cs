using System;
using System.Collections.Generic;

namespace Hndy.Ioc.Tests
{
    namespace Nullable
    {
        class Foo { }
        class Fee { }
        struct Far { }
        struct Bar { }
        struct Car { }

        class Cot
        {
            public Foo? Foo { get; set; }
            public Far? Far { get; set; }
            public Bar? Bar { get; set; }
            public Cot(Foo? foo, Far? far, Nullable<Bar> bar)
            {
                Foo = foo;
                Far = far;
                Bar = bar;
            }
        }

        partial class NullInstancesRegistration : IocRegistration
        {
            public NullInstancesRegistration()
            {
                Singleton(new Foo());
                Singleton<Far>();

                // null instance registeration will be ignored.
                Singleton(default(Foo));
                Singleton(default(Far?));
            }
        }

        partial class NullFactoriesRegistration : IocRegistration
        {
            public NullFactoriesRegistration()
            {
                Foo? foo1 = null, foo2 = null, foo3 = null;
                Singleton(() => { var f = foo1; foo1 = new Foo(); return f; });
                Singleton("", () => { var f = foo2; foo2 = new Foo(); return f; });
                Singleton<Foo, string>(s => { var f = foo3; foo3 = new Foo(); return f; });

                Fee? fee1 = null, fee2 = null, fee3 = null;
                Dictionary<string, Fee> dictFee = new();
                Scoped(() => { var f = fee1; fee1 = new Fee(); return f; });
                Scoped("", () => { var f = fee2; fee2 = new Fee(); return f; });
                Scoped<Fee, string>(s => { var f = fee3; fee3 = new Fee(); return f; });
            }
        }

        partial class WiringRegistration : IocRegistration
        {
            public WiringRegistration()
            {
                Transient<Far>();
                Transient<Cot>();
                Transient("blue", () => new Bar());
                TransientFor<Cot, string>("red").Use<Cot>(new Wirers.Cot()
                {
                    foo = new Foo(),
                    far = null
                });
                TransientFor<Cot, string>("blue").Use<Cot>(new Wirers.Cot()
                {
                    foo = new("blue"),
                    far = new("blue"),
                    bar = new("blue")
                });
            }
        }

        partial class SingletonRegistration : IocRegistration
        {
            public SingletonRegistration()
            {
                Singleton(default(Foo?));
                Singleton(default(Far?));
                Singleton(() => default(Fee?));
                Singleton(() => default(Bar?));
                Singleton(c => default(Car?));
                Singleton(0, default(Foo?));
                Singleton(0, default(Far?));
                Singleton(0, () => default(Fee?));
                Singleton(0, () => default(Bar?));
                Singleton(false, c => default(Fee?));
                Singleton(false, c => default(Bar?));
                Singleton<Fee, string>(s => default(Fee?));
                Singleton<Bar, string>(s => default(Bar?));
                Singleton<Foo, string>((c, s) => default(Foo?));
                Singleton<Far, string>((c, s) => default(Far?));
            }
        }

        partial class ScopedRegistration : IocRegistration
        {
            public ScopedRegistration()
            {
                Scoped(() => default(Fee?));
                Scoped(() => default(Bar?));
                Scoped(0, () => default(Fee?));
                Scoped(0, () => default(Bar?));
                Scoped(false, c => default(Fee?));
                Scoped(false, c => default(Bar?));
                Scoped<Fee, string>(s => default(Fee?));
                Scoped<Bar, string>(s => default(Bar?));
                Scoped<Foo, string>((c, s) => default(Foo?));
                Scoped<Far, string>((c, s) => default(Far?));
            }
        }

        partial class TransientRegistration : IocRegistration
        {
            public TransientRegistration()
            {
                Transient(() => default(Fee?));
                Transient(() => default(Bar?));
                Transient(0, () => default(Fee?));
                Transient(0, () => default(Bar?));
                Transient(false, c => default(Fee?));
                Transient(false, c => default(Bar?));
                Transient<Fee, string>(s => default(Fee?));
                Transient<Bar, string>(s => default(Bar?));
                Transient<Foo, string>((c, s) => default(Foo?));
                Transient<Far, string>((c, s) => default(Far?));
            }
        }
    }
}
