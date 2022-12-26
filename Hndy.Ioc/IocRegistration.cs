using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace Hndy.Ioc
{
    public abstract partial class IocRegistration
    {
        internal List<IocItem> Items { get; } = new List<IocItem>();

        protected void Singleton<TService>() where TService : notnull
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Singleton)
            {
                IocFactory = GetIocFactory(typeof(TService))
            });
        }

        protected void Singleton<TService>(TService? service) where TService : notnull
        {
            if (service is not null)
            {
                Items.Add(new IocItem(typeof(TService), IocLifecycle.Singleton)
                {
                    Object = service
                });
            }
        }

        protected void Singleton<TService>(TService? service) where TService : struct
        {
            if (service is not null)
            {
                Items.Add(new IocItem(typeof(TService), IocLifecycle.Singleton)
                {
                    Object = service
                });
            }
        }

        protected void Singleton<TService>(Func<TService?> createService) where TService : class
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Singleton)
            {
                RawFactory = () => createService()
            });
        }

        protected void Singleton<TService>(Func<TService?> createService) where TService : struct
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Singleton)
            {
                RawFactory = () => createService()
            });
        }

        protected void Singleton<TService>(Func<IIocSession, TService?> createService) where TService : notnull
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Singleton)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Singleton<TService>(Func<IIocSession, TService?> createService) where TService : struct
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Singleton)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Singleton<TService, TName>() where TService : notnull where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), typeof(TName), IocLifecycle.Singleton)
            {
                IocFactoryParameterized = GetIocFactoryParameterized(typeof(TService), typeof(TName))
            });
        }

        protected void Singleton<TService, TName>(TName? name, TService? service) where TService : notnull where TName : IEquatable<TName>
        {
            if (service is not null)
            {
                Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Singleton)
                {
                    Object = service
                });
            }
        }

        protected void Singleton<TService, TName>(TName? name, TService? service) where TService : struct where TName : IEquatable<TName>
        {
            if (service is not null)
            {
                Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Singleton)
                {
                    Object = service
                });
            }
        }

        protected void Singleton<TService, TName>(TName? name, TService? service) where TService : notnull where TName : struct, IEquatable<TName>
        {
            if (service is not null)
            {
                Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Singleton)
                {
                    Object = service
                });
            }
        }

        protected void Singleton<TService, TName>(TName? name, TService? service) where TService : struct where TName : struct, IEquatable<TName>
        {
            if (service is not null)
            {
                Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Singleton)
                {
                    Object = service
                });
            }
        }

        protected void Singleton<TService, TName>(TName? name, Func<TService?> createService) where TService : notnull where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Singleton)
            {
                RawFactory = () => createService()
            });
        }

        protected void Singleton<TService, TName>(TName? name, Func<TService?> createService) where TService : struct where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Singleton)
            {
                RawFactory = () => createService()
            });
        }

        protected void Singleton<TService, TName>(TName? name, Func<TService?> createService) where TService : notnull where TName : struct, IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Singleton)
            {
                RawFactory = () => createService()
            });
        }

        protected void Singleton<TService, TName>(TName? name, Func<TService?> createService) where TService : struct where TName : struct, IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Singleton)
            {
                RawFactory = () => createService()
            });
        }

        protected void Singleton<TService, TName>(TName? name, Func<IIocSession, TService?> createService) where TService : notnull where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Singleton)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Singleton<TService, TName>(TName? name, Func<IIocSession, TService?> createService) where TService : struct where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Singleton)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Singleton<TService, TName>(TName? name, Func<IIocSession, TService?> createService) where TService : notnull where TName : struct, IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Singleton)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Singleton<TService, TName>(TName? name, Func<IIocSession, TService?> createService) where TService : struct where TName : struct, IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Singleton)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Singleton<TService, TName>(Func<TName, TService?> createService) where TService : notnull where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), typeof(TName), IocLifecycle.Singleton)
            {
                RawFactoryParameterized = n => createService((TName)n)
            });
        }

        protected void Singleton<TService, TName>(Func<TName, TService?> createService) where TService : struct where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), typeof(TName), IocLifecycle.Singleton)
            {
                RawFactoryParameterized = n => createService((TName)n)
            });
        }

        protected void Singleton<TService, TName>(Func<IIocSession, TName, TService?> createService) where TService : notnull where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), typeof(TName), IocLifecycle.Singleton)
            {
                IocFactoryParameterized = (s, n) => createService(s, (TName)n)
            });
        }

        protected void Singleton<TService, TName>(Func<IIocSession, TName, TService?> createService) where TService : struct where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), typeof(TName), IocLifecycle.Singleton)
            {
                IocFactoryParameterized = (s, n) => createService(s, (TName)n)
            });
        }

        protected Clause<TService> SingletonFor<TService>() where TService : notnull
        {
            var item = new IocItem(typeof(TService), IocLifecycle.Singleton);
            return new Clause<TService>(this, item);
        }

        protected Clause<TService> SingletonFor<TService, TName>(TName? name) where TService : notnull where TName : IEquatable<TName>
        {
            var item = new IocItem(typeof(TService), name, IocLifecycle.Singleton);
            return new Clause<TService>(this, item);
        }

        protected Clause<TService> SingletonFor<TService, TName>(TName? name) where TService : notnull where TName : struct, IEquatable<TName>
        {
            var item = new IocItem(typeof(TService), name, IocLifecycle.Singleton);
            return new Clause<TService>(this, item);
        }

        protected ClauseNamed<TService, TName> SingletonFor<TService, TName>() where TService : notnull where TName : IEquatable<TName>
        {
            var item = new IocItem(typeof(TService), typeof(TName), IocLifecycle.Singleton);
            return new ClauseNamed<TService, TName>(this, item);
        }

        protected void Scoped<TService>() where TService : notnull
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Scoped)
            {
                IocFactory = GetIocFactory(typeof(TService))
            });
        }

        protected void Scoped<TService>(Func<TService?> createService) where TService : notnull
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Scoped)
            {
                RawFactory = () => createService()
            });
        }

        protected void Scoped<TService>(Func<TService?> createService) where TService : struct
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Scoped)
            {
                RawFactory = () => createService()
            });
        }

        protected void Scoped<TService>(Func<IIocSession, TService?> createService) where TService : notnull
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Scoped)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Scoped<TService>(Func<IIocSession, TService?> createService) where TService : struct
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Scoped)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Scoped<TService, TName>() where TService : notnull where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), typeof(TName), IocLifecycle.Scoped)
            {
                IocFactoryParameterized = GetIocFactoryParameterized(typeof(TService), typeof(TName))
            });
        }

        protected void Scoped<TService, TName>(TName? name, Func<TService?> createService) where TService : notnull where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Scoped)
            {
                RawFactory = () => createService()
            });
        }

        protected void Scoped<TService, TName>(TName? name, Func<TService?> createService) where TService : struct where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Scoped)
            {
                RawFactory = () => createService()
            });
        }

        protected void Scoped<TService, TName>(TName? name, Func<TService?> createService) where TService : notnull where TName : struct, IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Scoped)
            {
                RawFactory = () => createService()
            });
        }

        protected void Scoped<TService, TName>(TName? name, Func<TService?> createService) where TService : struct where TName : struct, IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Scoped)
            {
                RawFactory = () => createService()
            });
        }

        protected void Scoped<TService, TName>(TName? name, Func<IIocSession, TService?> createService) where TService : notnull where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Scoped)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Scoped<TService, TName>(TName? name, Func<IIocSession, TService?> createService) where TService : struct where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Scoped)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Scoped<TService, TName>(TName? name, Func<IIocSession, TService?> createService) where TService : notnull where TName : struct, IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Scoped)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Scoped<TService, TName>(TName? name, Func<IIocSession, TService?> createService) where TService : struct where TName : struct, IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Scoped)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Scoped<TService, TName>(Func<TName, TService?> createService) where TService : notnull where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), typeof(TName), IocLifecycle.Scoped)
            {
                RawFactoryParameterized = n => createService((TName)n)
            });
        }

        protected void Scoped<TService, TName>(Func<TName, TService?> createService) where TService : struct where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), typeof(TName), IocLifecycle.Scoped)
            {
                RawFactoryParameterized = n => createService((TName)n)
            });
        }

        protected void Scoped<TService, TName>(Func<IIocSession, TName, TService?> createService) where TService : notnull where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), typeof(TName), IocLifecycle.Scoped)
            {
                IocFactoryParameterized = (s, n) => createService(s, (TName)n)
            });
        }

        protected void Scoped<TService, TName>(Func<IIocSession, TName, TService?> createService) where TService : struct where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), typeof(TName), IocLifecycle.Scoped)
            {
                IocFactoryParameterized = (s, n) => createService(s, (TName)n)
            });
        }

        protected Clause<TService> ScopedFor<TService>() where TService : notnull
        {
            var item = new IocItem(typeof(TService), IocLifecycle.Scoped);
            return new Clause<TService>(this, item);
        }

        protected Clause<TService> ScopedFor<TService, TName>(TName? name) where TService : notnull where TName : IEquatable<TName>
        {
            var item = new IocItem(typeof(TService), name, IocLifecycle.Scoped);
            return new Clause<TService>(this, item);
        }

        protected Clause<TService> ScopedFor<TService, TName>(TName? name) where TService : notnull where TName : struct, IEquatable<TName>
        {
            var item = new IocItem(typeof(TService), name, IocLifecycle.Scoped);
            return new Clause<TService>(this, item);
        }

        protected ClauseNamed<TService, TName> ScopedFor<TService, TName>() where TService : notnull where TName : IEquatable<TName>
        {
            var item = new IocItem(typeof(TService), typeof(TName), IocLifecycle.Scoped);
            return new ClauseNamed<TService, TName>(this, item);
        }

        protected void Transient<TService>() where TService : notnull
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Transient)
            {
                IocFactory = GetIocFactory(typeof(TService))
            });
        }

        protected void Transient<TService>(Func<TService?> createService) where TService : notnull
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Transient)
            {
                RawFactory = () => createService()
            });
        }

        protected void Transient<TService>(Func<TService?> createService) where TService : struct
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Transient)
            {
                RawFactory = () => createService()
            });
        }

        protected void Transient<TService>(Func<IIocSession, TService?> createService) where TService : notnull
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Transient)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Transient<TService>(Func<IIocSession, TService?> createService) where TService : struct
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Transient)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Transient<TService, TName>(TName? name, Func<TService?> createService) where TService : notnull where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Transient)
            {
                RawFactory = () => createService()
            });
        }

        protected void Transient<TService, TName>(TName? name, Func<TService?> createService) where TService : struct where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Transient)
            {
                RawFactory = () => createService()
            });
        }

        protected void Transient<TService, TName>(TName? name, Func<TService?> createService) where TService : notnull where TName : struct, IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Transient)
            {
                RawFactory = () => createService()
            });
        }

        protected void Transient<TService, TName>(TName? name, Func<TService?> createService) where TService : struct where TName : struct, IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Transient)
            {
                RawFactory = () => createService()
            });
        }

        protected void Transient<TService, TName>(TName? name, Func<IIocSession, TService?> createService) where TService : notnull where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Transient)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Transient<TService, TName>(TName? name, Func<IIocSession, TService?> createService) where TService : struct where TName : IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Transient)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Transient<TService, TName>(TName? name, Func<IIocSession, TService?> createService) where TService : notnull where TName : struct, IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Transient)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Transient<TService, TName>(TName? name, Func<IIocSession, TService?> createService) where TService : struct where TName : struct, IEquatable<TName>
        {
            Items.Add(new IocItem(typeof(TService), name, IocLifecycle.Transient)
            {
                IocFactory = s => createService(s)
            });
        }

        protected void Transient<TService, TOptions>() where TService : notnull
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Transient)
            {
                IocFactoryParameterized = GetIocFactoryParameterized(typeof(TService), typeof(TOptions)),
                ParameterType = typeof(TOptions)
            });
        }

        protected void Transient<TService, TOptions>(Func<TOptions, TService?> createService) where TService : notnull
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Transient)
            {
                RawFactoryParameterized = p => createService((TOptions)p),
                ParameterType = typeof(TOptions)
            });
        }

        protected void Transient<TService, TOptions>(Func<TOptions, TService?> createService) where TService : struct
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Transient)
            {
                RawFactoryParameterized = p => createService((TOptions)p),
                ParameterType = typeof(TOptions)
            });
        }

        protected void Transient<TService, TOptions>(Func<IIocSession, TOptions, TService?> createService) where TService : notnull
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Transient)
            {
                IocFactoryParameterized = (s, p) => createService(s, (TOptions)p),
                ParameterType = typeof(TOptions)
            });
        }

        protected void Transient<TService, TOptions>(Func<IIocSession, TOptions, TService?> createService) where TService : struct
        {
            Items.Add(new IocItem(typeof(TService), IocLifecycle.Transient)
            {
                IocFactoryParameterized = (s, p) => createService(s, (TOptions)p),
                ParameterType = typeof(TOptions)
            });
        }

        protected Clause<TService> TransientFor<TService>() where TService : notnull
        {
            var item = new IocItem(typeof(TService), IocLifecycle.Transient);
            return new Clause<TService>(this, item);
        }

        protected Clause<TService> TransientFor<TService, TName>(TName? name) where TService : notnull where TName : IEquatable<TName>
        {
            var item = new IocItem(typeof(TService), name, IocLifecycle.Transient);
            return new Clause<TService>(this, item);
        }

        protected Clause<TService> TransientFor<TService, TName>(TName? name) where TService : notnull where TName : struct, IEquatable<TName>
        {
            var item = new IocItem(typeof(TService), name, IocLifecycle.Transient);
            return new Clause<TService>(this, item);
        }

        protected ClauseParameterized<TService, TOptions> TransientFor<TService, TOptions>() where TService : notnull
        {
            var item = new IocItem(typeof(TService), typeof(TOptions), IocLifecycle.Transient);
            return new ClauseParameterized<TService, TOptions>(this, item);
        }

        protected internal virtual Func<IIocSession, object> GetIocFactory(Type type)
        {
            throw new InvalidOperationException($"IocFactory is missing for type {type.FullName}.");
        }

        protected internal virtual Func<IIocSession, object, object> GetIocFactoryParameterized(Type type, Type optionsType)
        {
            throw new InvalidOperationException($"IocFactory is missing for type {type.FullName} with '{optionsType.FullName}' options.");
        }
    }
}
