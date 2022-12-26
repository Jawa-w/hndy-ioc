using System;
using System.Runtime.Serialization;

namespace Hndy.Ioc
{
    [Serializable]
    public class IocCircularDependenciesException : Exception
    {
        public IocCircularDependenciesException(Type serviceType, object? name = null)
            : base(name is null ?
                  $"Circular dependencies are detected when get instance of type '{serviceType.FullName}' from IoC container." :
                  $"Circular dependencies are detected when get instance of type '{serviceType.FullName}' with name '{name}' from IoC container.")
        {
        }

        protected IocCircularDependenciesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}