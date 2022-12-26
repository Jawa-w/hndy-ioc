using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Hndy.Ioc
{
    public class IocContainer : IIocContainer
    {
        public IocContainer? Parent { get; }
        internal IocActivator Activator { get; }

        public bool IsDisposed { get; private set; }

        public IocContainer(params IocRegistration[] registrations)
        {
            Activator = new IocActivator(GetDefaultRegItems(), registrations);
        }

        public IocContainer(IocContainer? parent, params IocRegistration[] registrations)
        {
            Parent = parent;
            Activator = new IocActivator(GetDefaultRegItems(), registrations);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true; // avoid reenter dispose() method
                if (disposing)
                {
                    Activator.Dispose();
                }
            }
        }

        private IocItem[] GetDefaultRegItems()
        {
            return new[]
            {
                new IocItem(typeof(IIocContainer), IocLifecycle.Singleton) { Object = this },
                new IocItem(typeof(IServiceLocator), IocLifecycle.Singleton) { Object = this },
                new IocItem(typeof(IIocScope), IocLifecycle.Transient) { RawFactory = NewScope },
            };
        }

        public IIocScope NewScope()
        {
            return new IocScope(this);
        }

        public bool TryGet<TService>([NotNullWhen(true)] out TService? service) where TService : notnull
        {
            var implicitScope = NewScope(); // never dispose
            return implicitScope.TryGet(out service);
        }

        public bool TryGet<TService>(object? nameOrOptions, [NotNullWhen(true)] out TService? service) where TService : notnull
        {
            var implicitScope = NewScope(); // never dispose
            return implicitScope.TryGet(nameOrOptions, out service);
        }

        public void InjectSingleton<TService>(TService? service) where TService : notnull
        {
            if (service is not null)
            {
                Activator.InjectSingleton(typeof(TService), service);
            }
        }

        public void InjectSingleton<TService>(TService? service) where TService : struct
        {
            if (service is not null)
            {
                Activator.InjectSingleton(typeof(TService), service);
            }
        }

        public void InjectSingleton<TService, TName>(TName? name, TService? service) where TService : notnull where TName : IEquatable<TName>
        {
            if (service is not null)
            {
                if(name is null)
                {
                    Activator.InjectSingleton(typeof(TService), service);
                }
                else
                {
                    Activator.InjectSingleton(typeof(TService), name, service);
                }
            }
        }

        public void InjectSingleton<TService, TName>(TName? name, TService? service) where TService : struct where TName : IEquatable<TName>
        {
            if (service is not null)
            {
                if (name is null)
                {
                    Activator.InjectSingleton(typeof(TService), service);
                }
                else
                {
                    Activator.InjectSingleton(typeof(TService), name, service);
                }
            }
        }

        public void InjectSingleton<TService, TName>(TName? name, TService? service) where TService : notnull where TName : struct, IEquatable<TName>
        {
            if (service is not null)
            {
                if (name is null)
                {
                    Activator.InjectSingleton(typeof(TService), service);
                }
                else
                {
                    Activator.InjectSingleton(typeof(TService), name, service);
                }
            }
        }

        public void InjectSingleton<TService, TName>(TName? name, TService? service) where TService : struct where TName : struct, IEquatable<TName>
        {
            if (service is not null)
            {
                if (name is null)
                {
                    Activator.InjectSingleton(typeof(TService), service);
                }
                else
                {
                    Activator.InjectSingleton(typeof(TService), name, service);
                }
            }
        }
    }
}
