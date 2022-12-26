using System;
using System.Linq;

namespace Hndy.Ioc.Tests
{
    namespace ControlFlow
    {
        class Foo
        {
            public int Num { get; }

            public Foo(int num)
            {
                Num = num;
            }
        }
        class Bar : IBar { }
        class BarDebug : IBar { }
        interface IBar { }

        partial class ControlFlowRegistration : IocRegistration
        {
            public ControlFlowRegistration(bool debug)
            {
                if(debug)
                {
                    SingletonFor<IBar>().Use<BarDebug>();
                }
                else
                {
                    SingletonFor<IBar>().Use<Bar>();
                }
                // Caution: for loop doesn't work as expected
                //for (int i = 0; i < 3; i++)
                //{
                //    Singleton(i, () => new Foo(i));
                //}
                foreach (int i in Enumerable.Range(0, 3))
                {
                    Singleton(i, () => new Foo(i));
                }
            }
        }
    }
}
