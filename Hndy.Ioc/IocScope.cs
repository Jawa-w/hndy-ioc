using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Hndy.Ioc
{
    sealed class IocScope : IIocScope
    {
        public IocContainer Container { get; }
        IIocContainer IIocScope.Container => Container;

        internal Dictionary<IocItem, object?> Caches = new();

        public IocScope(IocContainer container)
        {
            Container = container;
        }

        public void Dispose()
        {
            lock (Caches)
            {
                foreach (var obj in Caches.Values)
                {
                    (obj as IDisposable)?.Dispose();
                }
            }
        }

        public bool TryGet<TService>([NotNullWhen(true)] out TService? service) where TService : notnull
        {
            return new IocSession(this).TryGet(out service);
        }

        public bool TryGet<TService>(object? parameter, [NotNullWhen(true)] out TService? service) where TService : notnull
        {
            return new IocSession(this).TryGet(parameter, out service);
        }
    }
}
