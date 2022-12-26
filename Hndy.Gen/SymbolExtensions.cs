using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Hndy
{
    public static class SymbolExtensions
    {
        private static SymbolDisplayFormat FullNameGlobalFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
            SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        private static readonly HashSet<string> _BuiltInTypes = new(new[]
        {
            "bool",
            "byte",
            "sbyte",
            "char",
            "decimal",
            "double",
            "float",
            "int",
            "uint",
            "nint",
            "nuint",
            "long",
            "ulong",
            "short",
            "ushort",
            "object",
            "string",
        });

        public static bool IsBuiltIn(this ITypeSymbol type) => _BuiltInTypes.Contains(type.ToDisplayString());

        public static bool IsDerivedFrom(this INamedTypeSymbol typeSymbol, string baseClassFullName)
        {
            for (var b = typeSymbol.BaseType; b is not null; b = b.BaseType)
            {
                if (b.ToDisplayString() == baseClassFullName)
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetFullName(this ITypeSymbol type, bool withGlobalNamespace = false)
        {
            return withGlobalNamespace ? type.ToDisplayString(FullNameGlobalFormat) : type.ToDisplayString();
        }

        public static string GetNameWithTypeArguments(this INamedTypeSymbol type)
        {
            return type.TypeArguments.Length <= 0 ? type.Name :
                type.Name + "<" + string.Join(", ", type.TypeArguments.Select(t => t.Name)) + ">";
        }

        public static IEnumerable<INamedTypeSymbol> GetNestingTypeChain(this INamedTypeSymbol type, bool includeThis)
        {
            var stack = new Stack<INamedTypeSymbol>();
            if (includeThis)
            {
                stack.Push(type);
            }
            for (var t = type.ContainingType; t != null; t = t.ContainingType)
            {
                stack.Push(t);
            }
            return stack;
        }

        public static IEnumerable<INamedTypeSymbol> GetAllTypes(this INamespaceSymbol ns, bool includeNested)
        {
            if (includeNested)
            {
                return ns.GetAllTypes(false).SelectMany(t => new[] { t }.Concat(t.GetNestedTypes()));
            }
            return ns.GetTypeMembers().Concat(ns.GetNamespaceMembers().SelectMany(p => p.GetAllTypes(false)));
        }

        public static IEnumerable<INamedTypeSymbol> GetNestedTypes(this INamedTypeSymbol type)
        {
            return type.GetTypeMembers().SelectMany(t => new[] { t }.Concat(t.GetNestedTypes()));
        }

        public static INamespaceSymbol? GetNamespace(this INamespaceSymbol ns, string descendantNs)
        {
            foreach (var m in ns.GetNamespaceMembers())
            {
                var s = m.ToDisplayString();
                if (s == descendantNs)
                {
                    return m;
                }
                else if(descendantNs.StartsWith(s + "."))
                {
                    return m.GetNamespace(descendantNs);
                }
            }
            return null;
        }
    }
}
