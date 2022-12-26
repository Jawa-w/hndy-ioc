using System.Linq.Expressions;
using System;

namespace Hndy.Ioc
{
    partial class IocRegistration
    {
        protected sealed class ClauseNamed<TService, TName> where TService : notnull where TName : IEquatable<TName>
        {
            IocRegistration _registration;
            IocItem _item;

            internal ClauseNamed(IocRegistration registration, IocItem item)
            {
                _registration = registration;
                _item = item;
            }

            public void Use<T>() where T : TService
            {
                _item.IocFactoryParameterized = _registration.GetIocFactoryParameterized(typeof(T), typeof(TName));
                _registration.Items.Add(_item);
            }

            public void Use<T>(Func<TName, IIocWirer<T>> wiring) where T : TService
            {
                _item.WirerFactoryParameterized = n => wiring((TName)n);
                _registration.Items.Add(_item);
            }
        }
    }
}