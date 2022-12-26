using System;

namespace Hndy.Ioc
{
    class IocItem
    {
        public Type Type { get; }
        public object? Name { get; private set; }
        public Type? ParameterType { get; set; }
        public IocLifecycle Lifecycle { get; }
        public object? Object { get; set; }
        public Func<object?>? RawFactory { get; set; }
        public Func<object, object?>? RawFactoryParameterized { get; set; }
        public Func<IIocSession, object?>? IocFactory { get; set; }
        public Func<IIocSession, object, object?>? IocFactoryParameterized { get; set; }
        public IIocWirer? Wirer { get; set; }
        public Func<object, IIocWirer>? WirerFactoryParameterized { get; set; }

        public IocItem(Type type, IocLifecycle lifecycle)
        {
            Type = type;
            Lifecycle = lifecycle;
        }

        public IocItem(Type type, object? name, IocLifecycle lifecycle)
        {
            Type = type;
            Name = name;
            Lifecycle = lifecycle;
        }

        public IocItem(Type type, Type parameterType, IocLifecycle lifecycle)
        {
            Type = type;
            ParameterType = parameterType;
            Lifecycle = lifecycle;
        }

        public IocItem Clone()
        {
            return (IocItem)MemberwiseClone();
        }

        public IocItem ToNamedCache(object? service, object name)
        {
            return new IocItem(Type, Lifecycle)
            {
                Name = name,
                Object = service,
            };
        }

        public void Singletonize(object? service)
        {
            Object = service;
            RawFactory = null;
            RawFactoryParameterized = null;
            IocFactory = null;
            IocFactoryParameterized = null;
            Wirer = null;
            WirerFactoryParameterized = null;
        }

    }
}