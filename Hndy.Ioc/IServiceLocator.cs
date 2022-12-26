using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Hndy.Ioc
{
    public interface IServiceLocator
    {
        bool TryGet<TService>([NotNullWhen(true)] out TService? service) where TService : notnull;
        bool TryGet<TService>(object? nameOrOptions, [NotNullWhen(true)] out TService? service) where TService : notnull;
    }

    public static class ServiceLocatorExtensions
    {
        public static TService Get<TService>(this IServiceLocator serviceLocator) where TService : notnull
        {
            return serviceLocator.TryGet<TService>(out var service) ? service :
                throw new IocUnregisteredException(typeof(TService));
        }

        public static TService Get<TService>(this IServiceLocator serviceLocator, object? nameOrOptions) where TService : notnull
        {
            return serviceLocator.TryGet<TService>(nameOrOptions, out var service) ? service :
                throw new IocUnregisteredException(typeof(TService), nameOrOptions);
        }

        public static TService? TryGet<TService>(this IServiceLocator serviceLocator) where TService : notnull
        {
            return serviceLocator.TryGet<TService>(out var service) ? service : default;
        }

        public static TService? TryGetNullable<TService>(this IServiceLocator serviceLocator) where TService : struct
        {
            return serviceLocator.TryGet<TService>(out var service) ? service : null;
        }

        public static TService? TryGet<TService>(this IServiceLocator serviceLocator, object? nameOrOptions) where TService : notnull
        {
            return serviceLocator.TryGet<TService>(nameOrOptions, out var service) ? service : default;
        }

        public static TService? TryGetNullable<TService>(this IServiceLocator serviceLocator, object? nameOrOptions) where TService : struct
        {
            return serviceLocator.TryGet<TService>(nameOrOptions, out var service) ? service : null;
        }
    }
}
