using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;

namespace Hndy.Ioc.Gen
{
    public partial class IocRegAnalyzer : SourceAnalyzer
    {
        public IocRegAnalyzer()
            : base(DiagMessages.ResourceManager, typeof(DiagMessages))
        {
        }

        public override void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclSyntax)
            {
                SemanticModel = context.SemanticModel;
                Analyze(classDeclSyntax);
            }
        }

        private void Analyze(ClassDeclarationSyntax classDeclSyntax)
        {
            if (classDeclSyntax.BaseList is not null)
            {
                if (SemanticModel!.GetDeclaredSymbol(classDeclSyntax, CancellationToken) is INamedTypeSymbol classSymbol &&
                    classSymbol.IsDerivedFrom("Hndy.Ioc.IocRegistration"))
                {
                    AddClass(classSymbol);
                }
            }
        }

        private void AddClass(INamedTypeSymbol classSymbol)
        {
            var regInfo = new IocRegInfo(classSymbol);
            foreach (var member in classSymbol.GetMembers())
            {
                if (member is IMethodSymbol methodSymbol)
                {
                    foreach (var sr in methodSymbol.DeclaringSyntaxReferences)
                    {
                        var sn = sr.GetSyntax(CancellationToken);
                        var body = sn is ConstructorDeclarationSyntax ctorDeclSyntax ? ctorDeclSyntax.Body :
                            sn is MethodDeclarationSyntax methodDeclSyntax ? methodDeclSyntax.Body : null;
                        if (body is not null)
                        {
                            foreach (var n in body.DescendantNodes())
                            {
                                if (n is InvocationExpressionSyntax invocation)
                                {
                                    if (invocation.Expression is GenericNameSyntax gn)
                                    {
                                        AddIocFactory(regInfo, invocation, () => LocateFirstTypeArgumentFromName(gn));
                                    }
                                    else if (invocation.Expression is MemberAccessExpressionSyntax ma)
                                    {
                                        IocCtorInfo? wirer = null;
                                        if (ma.Name.ToString() == "Use")
                                        {
                                            AddWirer(regInfo, ma, null, out wirer);
                                        }
                                        else if (ma.Name is GenericNameSyntax gn1)
                                        {
                                            if (gn1.ToString().StartsWith("Use<"))
                                            {
                                                AddWirer(regInfo, ma, gn1.TypeArgumentList.Arguments[0], out wirer);
                                            }
                                            AddIocFactory(regInfo, invocation, () => LocateFirstTypeArgumentFromName(gn1));
                                        }

                                        if (wirer is not null)
                                        {
                                            DiagnoseWirer(wirer, invocation);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            AddBuilder(new IocRegBuilder(regInfo));
        }

        private void AddIocFactory(IocRegInfo regInfo, InvocationExpressionSyntax invocation, Func<Location> locateTypeSyntax)
        {
            if (invocation.ArgumentList.Arguments.Count == 0)
            {
                if (SemanticModel!.GetSymbolInfo(invocation, CancellationToken).Symbol is IMethodSymbol methodSymbol)
                {
                    if (methodSymbol.Name is "Singleton" or "Scoped" or "Transient" && methodSymbol.ReceiverType?.ToDisplayString() == "Hndy.Ioc.IocRegistration")
                    {
                        if (methodSymbol.TypeArguments[0] is INamedTypeSymbol svcTypeSymbol && CheckConstructor(svcTypeSymbol, locateTypeSyntax, out var ctor))
                        {
                            if (methodSymbol.TypeArguments.Length == 1)
                            {
                                regInfo.IocFactories.Add(CreateCtorInfo(svcTypeSymbol, ctor, locateTypeSyntax));
                            }
                            else if (methodSymbol.TypeArguments.Length == 2)
                            {
                                var paraType = methodSymbol.TypeArguments[1];
                                if (GetParameterIndexByType(ctor, paraType, out var paraIndex, locateTypeSyntax))
                                {
                                    regInfo.IocFactoriesParamterized.Add(CreateCtorInfo(svcTypeSymbol, ctor, paraIndex, paraType, locateTypeSyntax));
                                }
                            }
                        }
                    }
                    else if (methodSymbol.Name is "Use" && methodSymbol.TypeArguments.Length > 0)
                    {
                        var methodStr = methodSymbol.ToDisplayString();
                        if (methodSymbol.TypeArguments[0] is INamedTypeSymbol svcTypeSymbol && CheckConstructor(svcTypeSymbol, locateTypeSyntax, out var ctor))
                        {
                            if (methodStr.StartsWith("Hndy.Ioc.IocRegistration.Clause<"))
                            {
                                regInfo.IocFactories.Add(CreateCtorInfo(svcTypeSymbol, ctor, locateTypeSyntax));
                            }
                            else if (methodStr.StartsWith("Hndy.Ioc.IocRegistration.ClauseNamed<") && methodSymbol.ReceiverType is INamedTypeSymbol clauseNamedType)
                            {
                                var paraType = clauseNamedType.TypeArguments[1];
                                if (GetParameterIndexByType(ctor, paraType, out var paraIndex, locateTypeSyntax))
                                {
                                    regInfo.IocFactoriesParamterized.Add(CreateCtorInfo(svcTypeSymbol, ctor, paraIndex, paraType, locateTypeSyntax));
                                }
                            }
                            else if (methodStr.StartsWith("Hndy.Ioc.IocRegistration.ClauseParameterized<") && methodSymbol.ReceiverType is INamedTypeSymbol clauseParameterizedType)
                            {
                                var paraType = clauseParameterizedType.TypeArguments[1];
                                if (GetParameterIndexByType(ctor, paraType, out var paraIndex, locateTypeSyntax))
                                {
                                    regInfo.IocFactoriesParamterized.Add(CreateCtorInfo(svcTypeSymbol, ctor, paraIndex, paraType, locateTypeSyntax));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddWirer(IocRegInfo regInfo, MemberAccessExpressionSyntax maSyntax, TypeSyntax? targetTypeSyntax, out IocCtorInfo? wirer)
        {
            wirer = null;
            var expSymbol = SemanticModel!.GetSymbolInfo(maSyntax.Expression, CancellationToken).Symbol;
            if (expSymbol is IMethodSymbol methodSymbol)
            {
                var methodStr = methodSymbol.ReturnType.ToDisplayString();
                if (!methodStr.StartsWith("Hndy.Ioc.IocRegistration.Clause<") &&
                    !methodStr.StartsWith("Hndy.Ioc.IocRegistration.ClauseParameterized<") &&
                    !methodStr.StartsWith("Hndy.Ioc.IocRegistration.ClauseNamed<"))
                {
                    return;
                }
            }
            else
            {
                ReportDiagnostic("HI0004", DiagnosticSeverity.Error, 0, maSyntax.Name.GetLocation());
                return;
            }

            if (targetTypeSyntax is null)
            {
                if (methodSymbol.ReturnType is INamedTypeSymbol clauseTypeSymbol &&
                    clauseTypeSymbol.TypeArguments.Length > 0 &&
                    clauseTypeSymbol.TypeArguments[0] is INamedTypeSymbol svcTypeSymbol)
                {
                    if (CheckConstructor(svcTypeSymbol, () => LocateFirstTypeArgumentFromInvocation(maSyntax.Expression), out var ctor))
                    {
                        regInfo.Wirers.Add(wirer = new IocCtorInfo(svcTypeSymbol, ctor));
                    }
                }
            }
            else
            {
                if (SemanticModel!.GetSymbolInfo(targetTypeSyntax, CancellationToken).Symbol is INamedTypeSymbol svcTypeSymbol)
                {
                    if (CheckConstructor(svcTypeSymbol, () => targetTypeSyntax.GetLocation(), out var ctor))
                    {
                        regInfo.Wirers.Add(wirer = new IocCtorInfo(svcTypeSymbol, ctor));
                    }
                }
            }
        }

        private void DiagnoseWirer(IocCtorInfo wirer, InvocationExpressionSyntax invocation)
        {
            foreach (var node in invocation.DescendantNodes())
            {
                if (node is ObjectCreationExpressionSyntax objectCreation)
                {
                    var t = objectCreation.Type.ToString();
                    if (t.EndsWith($"Wirers.{wirer.GetWirerName(false)}") ||
                        t.EndsWith($"Wirers.{wirer.GetWirerName(true)}"))
                    {
                        foreach (var p in wirer.Params)
                        {
                            if (p.IsBuiltInType)
                            {
                                if (objectCreation.Initializer?.Expressions.Any(exp =>
                                    exp is AssignmentExpressionSyntax assign && assign.Left.ToString() == p.Name) != true)
                                {
                                    ReportDiagnostic("HI1001", DiagnosticSeverity.Warning, 1, objectCreation.Type.GetLocation());
                                    break;
                                }
                            }
                        }

                    }
                }
            }
        }

        private bool CheckConstructor(INamedTypeSymbol svcTypeSymbol, Func<Location> locateTypeSyntax, [NotNullWhen(true)] out IMethodSymbol? ctor)
        {
            ctor = null;
            foreach (var c in svcTypeSymbol.Constructors)
            {
                if (c.DeclaredAccessibility == Accessibility.Public)
                {
                    if (ctor is null)
                    {
                        ctor = c;
                    }
                    else
                    {
                        ReportDiagnostic("HI0001", new[] { svcTypeSymbol.Name }, DiagnosticSeverity.Error, 0, locateTypeSyntax());
                        return false;
                    }
                }
            }
            if (ctor is null)
            {
                foreach (var c in svcTypeSymbol.Constructors)
                {
                    if (c.DeclaredAccessibility is Accessibility.Internal or Accessibility.ProtectedOrInternal)
                    {
                        if (ctor is null)
                        {
                            ctor = c;
                        }
                        else
                        {
                            ctor = null;
                            break;
                        }
                    }
                }
                if (ctor is null)
                {
                    ReportDiagnostic("HI0002", new[] { svcTypeSymbol.Name }, DiagnosticSeverity.Error, 0, locateTypeSyntax());
                    return false;
                }
            }
            return true;
        }

        private bool GetParameterIndexByType(IMethodSymbol ctor, ITypeSymbol paraType, out int paraIndex, Func<Location> locateTypeSyntax)
        {
            paraIndex = -1;
            for (int i = 0; i < ctor.Parameters.Length; i++)
            {
                if (SymbolEqualityComparer.Default.Equals(ctor.Parameters[i].Type, paraType))
                {
                    if (paraIndex < 0)
                    {
                        paraIndex = i;
                    }
                    else
                    {
                        paraIndex = -1;
                        break;
                    }
                }
            }
            if (paraIndex >= 0)
            {
                return true;
            }
            else
            {
                ReportDiagnostic("HI0003", DiagnosticSeverity.Error, 0, locateTypeSyntax());
                return false;
            }
        }

        private IocCtorInfo CreateCtorInfo(INamedTypeSymbol typeSymbol, IMethodSymbol ctor, Func<Location> locateTypeSyntax)
        {
            IocCtorInfo ctorInfo = new IocCtorInfo(typeSymbol, ctor);
            if (ctorInfo.Params.Any(p => p.IsBuiltInType))
            {
                ReportDiagnostic("HI1001", DiagnosticSeverity.Warning, 1, locateTypeSyntax());
            }
            return ctorInfo;
        }

        private IocCtorInfo CreateCtorInfo(INamedTypeSymbol typeSymbol, IMethodSymbol ctor, int inputParaIndex, ITypeSymbol paraType, Func<Location> locateTypeSyntax)
        {
            IocCtorInfo ctorInfo = new IocCtorInfo(typeSymbol, ctor, inputParaIndex, paraType);
            if (ctorInfo.Params.Any(p => !p.IsInput && p.IsBuiltInType))
            {
                ReportDiagnostic("HI1001", DiagnosticSeverity.Warning, 1, locateTypeSyntax());
            }
            return ctorInfo;
        }

        private Location LocateFirstTypeArgumentFromName(GenericNameSyntax node)
        {
            return node.TypeArgumentList.Arguments[0].GetLocation();
        }

        private Location LocateFirstTypeArgumentFromInvocation(SyntaxNode node)
        {
            if (node is InvocationExpressionSyntax invoc)
            {
                if (invoc.Expression is GenericNameSyntax gname)
                {
                    return gname.TypeArgumentList.Arguments.First().GetLocation();
                }
                else if (invoc.Expression is MemberAccessExpressionSyntax ma && ma.Name is GenericNameSyntax gname1)
                {
                    return gname1.TypeArgumentList.Arguments.First().GetLocation();
                }
            }
            return node.GetLocation();
        }
    }
}