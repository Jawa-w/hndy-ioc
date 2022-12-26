using System;
using System.Collections.Generic;
using System.Text;

namespace Hndy.Ioc
{
    partial class IocRegistration
    {
        protected sealed class WirerParameter<T>
        {
            public static readonly WirerParameter<T> Unspecified = new();

            public bool HasValue { get; private set; }
            public T Value { get; private set; } = default!;
            public object? FactoryParameter { get; }

            public WirerParameter(object? factoryParameter = default)
            {
                FactoryParameter = factoryParameter;
            }

            public static implicit operator WirerParameter<T>(T value)
            {
                return new WirerParameter<T>(default)
                {
                    HasValue = true,
                    Value = value
                };
            }
        }
    }
}
