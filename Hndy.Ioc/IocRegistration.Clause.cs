using System.Linq.Expressions;
using System;

namespace Hndy.Ioc
{
    partial class IocRegistration
    {
        protected sealed class Clause<TService> where TService : notnull
        {
            IocRegistration _registration;
            IocItem _item;

            internal Clause(IocRegistration registration, IocItem item)
            {
                _registration = registration;
                _item = item;
            }

            public void Use<T>() where T : TService
            {
                _item.IocFactory = _registration.GetIocFactory(typeof(T));
                _registration.Items.Add(_item);
            }

            public void Use<T>(IIocWirer<T> wirer) where T : TService
            {
                _item.Wirer = wirer;
                _registration.Items.Add(_item);
            }
        }
    }
}