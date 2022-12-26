using System;
using System.Runtime.Serialization;

namespace Hndy.Ioc
{
    [Serializable]
    public class IocUnregisteredException : Exception
    {
        public IocUnregisteredException(Type serviceType, object? nameOrOptions = null)
            : base(nameOrOptions is null ?
                  $"Cannot get instance of type '{serviceType.FullName}' from IoC container." :
                  $"Cannot get instance of type '{serviceType.FullName}' with name or options '{nameOrOptions}' from IoC container.")
        {
        }

        protected IocUnregisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}