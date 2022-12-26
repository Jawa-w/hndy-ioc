using Microsoft.CodeAnalysis;
using System;

namespace Hndy.Ioc.Gen
{
    internal class IocCtorParam
    {
        public ITypeSymbol Type { get; }
        public string Name { get; }
        public bool Required { get; }
        public bool IsValueType { get; }
        public bool IsBuiltInType { get; }
        public string NonnullTypeName { get; }
        public bool IsInput { get; set; }

        public ITypeSymbol? FuncParam { get; }
        public IocCtorParam? FuncReturn { get; }

        public IocCtorParam(ITypeSymbol type, string name, bool detectFunc = true)
        {
            Type = type;
            Name = name;
            IsValueType = type.IsValueType;
            IsBuiltInType = Type.IsBuiltIn();

            var fullName = Type.GetFullName(true);
            if (fullName.EndsWith("?"))
            {
                NonnullTypeName = fullName.Substring(0, fullName.Length - 1);
            }
            else
            {
                NonnullTypeName = fullName;
                Required = true;
            }

            if (detectFunc && fullName.StartsWith("global::System.Func<") && type is INamedTypeSymbol namedType)
            {
                if (namedType.TypeArguments.Length == 1)
                {
                    FuncReturn = new IocCtorParam(namedType.TypeArguments[0], "", false);
                }
                else if (namedType.TypeArguments.Length == 2)
                {
                    FuncParam = namedType.TypeArguments[0];
                    FuncReturn = new IocCtorParam(namedType.TypeArguments[1], "", false);
                }
            }
        }
    }
}