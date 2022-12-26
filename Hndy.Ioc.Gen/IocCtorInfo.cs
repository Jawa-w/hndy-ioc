using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Hndy.Ioc.Gen
{
    class IocCtorInfo
    {
        public INamedTypeSymbol ServiceType { get; }
        public List<IocCtorParam> Params { get; }

        public ITypeSymbol? InputType { get; }

        public IocCtorInfo(INamedTypeSymbol typeSymbol, IMethodSymbol ctor)
        {
            ServiceType = typeSymbol;
            Params = ctor.Parameters.Select(paramSymbol => new IocCtorParam(paramSymbol.Type, paramSymbol.Name)).ToList();
        }

        public IocCtorInfo(INamedTypeSymbol typeSymbol, IMethodSymbol ctor, int inputParaIndex, ITypeSymbol paraType) 
            : this(typeSymbol, ctor)
        {
            Params[inputParaIndex].IsInput = true;
            InputType = paraType;
        }

        public string GetWirerName(bool full)
        {
            if (full)
            {
                return ServiceType.ToDisplayString().Replace(".", "").Replace(">", "").Replace(" ", "").Replace('<', '_').Replace(',', '_');
            }
            else
            {
                return ServiceType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Replace(".", "").Replace(">", "").Replace(" ", "").Replace('<', '_').Replace(',', '_');
            }
        }
    }
}