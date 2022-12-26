using System;
using System.Diagnostics.CodeAnalysis;

namespace Hndy.Ioc
{
    public interface IIocScope : IServiceLocator, IDisposable
    {
        IIocContainer Container { get; }
    }
}