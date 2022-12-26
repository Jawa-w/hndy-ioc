using System.Linq.Expressions;
using System;

namespace Hndy.Ioc
{
    partial class IocRegistration
    {
        protected sealed class ClauseParameterized<TService, TOptions> where TService : notnull
        {
            IocRegistration _registration;
            IocItem _item;

            internal ClauseParameterized(IocRegistration registration, IocItem item)
            {
                _registration = registration;
                _item = item;
            }

            public void Use<T>() where T : TService
            {
                _item.IocFactoryParameterized = _registration.GetIocFactoryParameterized(typeof(T), typeof(TOptions));
                _registration.Items.Add(_item);
            }

            public void Use<T>(Func<TOptions, IIocWirer<T>> wiring) where T : TService
            {
                _item.WirerFactoryParameterized = p => wiring((TOptions)p);
                _registration.Items.Add(_item);
            }
        }
    }
}