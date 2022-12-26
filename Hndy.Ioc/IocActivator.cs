using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using static System.Collections.Specialized.BitVector32;

namespace Hndy.Ioc
{
    sealed class IocActivator : IDisposable
    {
        class TypeReg
        {
            public IocItem? Default { get; set; }
            public Dictionary<object, IocItem>? Parameterized { get; set; }
        }

        readonly Dictionary<Type, TypeReg> _registrations = new();

        public IocActivator(IocItem[] defaultRegItems, IocRegistration[] registrations)
        {
            foreach (var item in defaultRegItems.Concat(registrations.SelectMany(reg => reg.Items)))
            {
                if (!_registrations.TryGetValue(item.Type, out var tr))
                {
                    _registrations[item.Type] = tr = new TypeReg();
                }

                if (item.ParameterType is not null)
                {
                    tr.Parameterized ??= new();
                    tr.Parameterized[item.ParameterType] = item.Clone();
                }
                else if (item.Name is not null)
                {
                    tr.Parameterized ??= new();
                    tr.Parameterized[item.Name] = item.Clone();
                }
                else
                {
                    tr.Default = item.Clone();
                }
            }
        }

        public void Dispose()
        {
            lock (_registrations)
            {
                foreach (var tr in _registrations.Values)
                {
                    if (tr.Default?.Object is IDisposable obj)
                    {
                        obj.Dispose();
                    }
                    var dict = tr.Parameterized;
                    if (dict is not null)
                    {
                        lock (dict)
                        {
                            foreach (var item in dict.Values)
                            {
                                if (item.Object is IDisposable obj1)
                                {
                                    obj1.Dispose();
                                }
                            }
                        }
                    }
                }
                _registrations.Clear();
            }
        }

        object? CreateWithFactory(IocItem item, IocSession session)
        {
            var rawFactory = item.RawFactory;
            if (rawFactory is not null)
            {
                return rawFactory();
            }
            var iocFactory = item.IocFactory;
            if (iocFactory is not null)
            {
                return iocFactory(session);
            }
            var wirer = item.Wirer;
            if (wirer is not null)
            {
                return wirer.Create(session);
            }
            return null;
        }

        object? CreateWithFactoryParameterized(IocItem item, IocSession session, object parameter)
        {
            var rawFactory = item.RawFactoryParameterized;
            if (rawFactory is not null)
            {
                return rawFactory(parameter);
            }
            var iocFactory = item.IocFactoryParameterized;
            if (iocFactory is not null)
            {
                return iocFactory(session, parameter);
            }
            var wirerFactory = item.WirerFactoryParameterized;
            if (wirerFactory is not null)
            {
                return wirerFactory(parameter).Create(session);
            }
            return null; // never
        }

        object? GetWithCacheOrFactory(IocItem item, IocSession session)
        {
            switch (item.Lifecycle)
            {
                case IocLifecycle.Singleton:
                    var cache = item.Object;
                    if (cache is not null)
                    {
                        return cache;
                    }
                    lock (item)
                    {
                        cache = CreateWithFactory(item, session);
                        item.Singletonize(cache);
                        return cache;
                    }
                case IocLifecycle.Scoped:
                    lock (item)
                    {
                        var caches = session.Scope.Caches;
                        lock (caches)
                        {
                            if (caches.TryGetValue(item, out cache))
                            {
                                return cache;
                            }
                        }
                        var service = CreateWithFactory(item, session);
                        lock (caches)
                        {
                            return caches[item] = service;
                        }
                    }
                default:
                    return CreateWithFactory(item, session);
            }
        }

        object? GetWithFatoryParameterized(IocItem parameterizedItem, IocSession session, object parameter, Dictionary<object, IocItem> paraDict)
        {
            switch (parameterizedItem.Lifecycle)
            {
                case IocLifecycle.Singleton:
                    lock (parameterizedItem)
                    {
                        var service = CreateWithFactoryParameterized(parameterizedItem, session, parameter);
                        paraDict[parameter] = parameterizedItem.ToNamedCache(service, parameter);
                        return service;
                    }
                case IocLifecycle.Scoped:
                    lock (parameterizedItem)
                    {
                        var service = CreateWithFactoryParameterized(parameterizedItem, session, parameter);
                        var cacheItem = paraDict[parameter] = parameterizedItem.ToNamedCache(service, parameter);
                        var caches = session.Scope.Caches;
                        lock (caches)
                        {
                            caches[cacheItem] = service;
                        }
                        return service;
                    }
                default:
                    return CreateWithFactoryParameterized(parameterizedItem, session, parameter);
            }
        }

        public bool TryGet(Type type, IocSession session, out object? service)
        {
            TypeReg? tr;
            lock (_registrations)
            {
                if (!_registrations.TryGetValue(type, out tr))
                {
                    service = default;
                    return false;
                }
            }

            var item = tr.Default;
            if (item is not null)
            {
                session.BeginCreate(item);
                service = GetWithCacheOrFactory(item, session);
                session.EndCreate(item);
                return true;
            }
            service = default;
            return false;
        }

        public bool TryGet(Type type, IocSession session, object? parameter, out object? service)
        {
            TypeReg? tr;
            lock (_registrations)
            {
                if (!_registrations.TryGetValue(type, out tr))
                {
                    service = default;
                    return false;
                }
            }

            if (parameter is not null)
            {
                var dict = tr.Parameterized;
                if (dict is not null)
                {
                    lock (dict)
                    {
                        if (dict.TryGetValue(parameter, out var namedItem))
                        {
                            session.BeginCreate(namedItem);
                            service = GetWithCacheOrFactory(namedItem, session);
                            session.EndCreate(namedItem);
                            return true;
                        }
                        if (dict.TryGetValue(parameter.GetType(), out var parameterizedItem))
                        {
                            session.BeginCreate(parameterizedItem);
                            service = GetWithFatoryParameterized(parameterizedItem, session, parameter, dict);
                            session.EndCreate(parameterizedItem);
                            return true;
                        }
                    }
                }
            }
            else
            {
                var item = tr.Default;
                if (item is not null)
                {
                    session.BeginCreate(item);
                    service = GetWithCacheOrFactory(item, session);
                    session.EndCreate(item);
                    return true;
                }
            }
            service = default;
            return false;
        }

        public void InjectSingleton(Type type, object service)
        {
            TypeReg? tr;
            lock (_registrations)
            {
                if (!_registrations.TryGetValue(type, out tr))
                {
                    _registrations[type] = tr = new TypeReg();
                }
            }

            var item = tr.Default;
            if (item is null)
            {
                tr.Default = new IocItem(type, IocLifecycle.Singleton) { Object = service };
            }
            else
            {
                lock (item)
                {
                    item.Singletonize(service);
                }
            }
        }

        public void InjectSingleton(Type type, object name, object service)
        {
            TypeReg? tr;
            lock (_registrations)
            {
                if (!_registrations.TryGetValue(type, out tr))
                {
                    _registrations[type] = tr = new TypeReg();
                }
            }

            Dictionary<object, IocItem>? dict;
            lock (tr)
            {
                dict = tr.Parameterized;
                if (dict is null)
                {
                    tr.Parameterized = dict = new();
                }
            }

            IocItem? item;
            lock (dict)
            {
                if (!dict.TryGetValue(name, out item))
                {
                    dict[name] = new IocItem(type, name, IocLifecycle.Singleton) { Object = service };
                    return;
                }
            }

            lock (item)
            {
                item.Singletonize(service);
            }
        }
    }
}