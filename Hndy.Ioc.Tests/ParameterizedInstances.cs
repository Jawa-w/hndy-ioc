using System;

namespace Hndy.Ioc.Tests
{
    namespace ParameterizedInstances
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

        class Bin
        {
            public int Id { get; }

            public Bin(int id)
            {
                Id = id;
            }
        }

        class Cot
        {
            public Foo Foo { get; set; }
            public Bar Bar { get; set; }
            public Bin Bin { get; }
            public DateTime Time { get; }

            public Cot(Foo foo, Bar bar, Bin bin, DateTime time)
            {
                Foo = foo;
                Bar = bar;
                Bin = bin;
                Time = time;
            }
        }

        public class CotOptions
        {
            public string FooName { get; }
            public int BinId { get; }
            public DateTime Time { get; }

            public CotOptions(string fooName, int binId, DateTime time)
            {
                FooName = fooName;
                BinId = binId;
                Time = time;
            }
        }

        partial class ParameterizedInstancesRegistration : IocRegistration
        {
            public ParameterizedInstancesRegistration()
            {
                Transient<Foo, string>(name => new Foo(name));
                Transient<Bar>();
                Transient<Bin, int>();
                TransientFor<Cot, CotOptions>().Use<Cot>(opt => new Wirers.Cot()
                {
                    foo = new(opt.FooName),
                    bin = new(opt.BinId),
                    time = opt.Time
                });
                Singleton(x => x.Get<Cot>(x.Get<CotOptions>()));
                Singleton(new CotOptions("foobar", 7, new DateTime(638041945578956391)));
            }
        }
    }
}
