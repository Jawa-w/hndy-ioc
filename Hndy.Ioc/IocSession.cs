using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Hndy.Ioc
{
    class IocSession : IIocSession
    {
        public IocScope Scope { get; }
        IIocScope IIocSession.Scope => Scope;

        HashSet<IocItem> _hashSet = new();

        public IocSession(IocScope scope)
        {
            Scope = scope;
        }

        internal void BeginCreate(IocItem item)
        {
            if (_hashSet.Contains(item))
            {
                throw new IocCircularDependenciesException(item.Type, item.Name);
            }
            _hashSet.Add(item);
        }

        internal void EndCreate(IocItem item)
        {
            _hashSet.Remove(item);
        }

        public bool TryGet<TService>([NotNullWhen(true)] out TService? service) where TService : notnull
        {
            Type serviceType = typeof(TService);
            if (Scope.Container.Activator.TryGet(serviceType, this, out var obj))
            {
                if (obj is null)
                {
                    service = default;
                    return false;
                }
                service = (TService)obj;
                return true;
            }
            for (var c = Scope.Container.Parent; c is not null; c = c.Parent)
            {
                if (c.Activator.TryGet(serviceType, this, out obj))
                {
                    if (obj is null)
                    {
                        service = default;
                        return false;
                    }
                    service = (TService)obj;
                    return true;
                }
            }
            service = default;
            return false;
        }

        public bool TryGet<TService>(object? parameter, [NotNullWhen(true)] out TService? service) where TService : notnull
        {
            Type serviceType = typeof(TService);
            if (Scope.Container.Activator.TryGet(serviceType, this, parameter, out var obj))
            {
                if (obj is null)
                {
                    service = default;
                    return false;
                }
                service = (TService)obj;
                return true;
            }
            for (var c = Scope.Container.Parent; c is not null; c = c.Parent)
            {
                if (c.Activator.TryGet(serviceType, this, parameter, out obj))
                {
                    if (obj is null)
                    {
                        service = default;
                        return false;
                    }
                    service = (TService)obj;
                    return true;
                }
            }
            service = default;
            return false;
        }
    }
}
