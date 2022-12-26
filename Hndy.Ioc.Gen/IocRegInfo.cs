using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hndy.Ioc.Gen
{
    internal class IocRegInfo
    {
        public INamedTypeSymbol RegClass { get; }

        public List<IocCtorInfo> Wirers = new();
        public List<IocCtorInfo> IocFactories = new();
        public List<IocCtorInfo> IocFactoriesParamterized = new();

        public IocRegInfo(INamedTypeSymbol classSymbol)
        {
            RegClass = classSymbol;
        }
    }
}
