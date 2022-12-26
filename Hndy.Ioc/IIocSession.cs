using System.Diagnostics.CodeAnalysis;

namespace Hndy.Ioc
{
    public interface IIocSession : IServiceLocator
    {
        IIocScope Scope { get; }
    }
}