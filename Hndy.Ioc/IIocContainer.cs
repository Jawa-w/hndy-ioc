using System;
using System.Diagnostics.CodeAnalysis;

namespace Hndy.Ioc
{
    public interface IIocContainer : IServiceLocator, IDisposable
    {
        IIocScope NewScope();
    }
}