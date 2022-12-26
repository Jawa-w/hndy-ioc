using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Hndy.Ioc.Gen
{
    internal class IocRegBuilder : ISourceBuilder
    {
        public IocRegInfo Info { get; }
        public string FileName => $"{Info.RegClass.GetFullName().Replace('<', '[').Replace('>', ']').Replace(" ", "")}.g.cs";

        public IocRegBuilder(IocRegInfo regInfo)
        {
            Info = regInfo;
        }

        public void Build(CsWriter w, GeneratorExecutionContext context)
        {
            var target = Info.RegClass;
            w.WriteLine("#nullable enable").WriteLine();
            w.WriteLine("using System;");
            w.WriteLine("using Hndy.Ioc;").WriteLine();
            w.BraceIndent($"namespace {target.ContainingNamespace.ToDisplayString()}", () => w
                .BraceIndentChain(target.GetNestingTypeChain(true), c => $"partial class {c.GetNameWithTypeArguments()}", () =>
                {
                    w.BraceIndent("protected override Func<IIocSession, object> GetIocFactory(Type type)", () =>
                    {
                        WriteIocFactories(w);
                        w.WriteLine("return base.GetIocFactory(type);");
                    });
                    w.BraceIndent("protected override Func<IIocSession, object, object> GetIocFactoryParameterized(Type type, Type optionsType)", () =>
                    {
                        WriteIocFactoriesParameterized(w);
                        w.WriteLine("return base.GetIocFactoryParameterized(type, optionsType);");
                    });
                    w.WriteLine();
                    WriteWirers(w);
                })
            );
        }

        private void WriteIocFactories(CsWriter w)
        {
            foreach (var g in Info.IocFactories.GroupBy(f => f.ServiceType.GetFullName(true)))
            {
                var f = g.First();
                w.BraceIndent($"if (type == typeof({g.Key}))", () =>
                {
                    w.BraceIndent($"static object create(IIocSession session)", () =>
                    {
                        if (f.Params.Any(p => p.FuncReturn is not null))
                        {
                            w.WriteLine("var scope = session.Scope;");
                        }
                        w.WriteParenthesesWrapped($"return new {g.Key}", f.Params, p => WriteIocFactoryParam(w, "session", p, ""));
                        w.WriteLine(";");
                    });
                    w.WriteLine("return create;");
                });
            }
        }

        private void WriteIocFactoriesParameterized(CsWriter w)
        {
            foreach (var g in Info.IocFactoriesParamterized.GroupBy(f => f.ServiceType.GetFullName(true)))
            {
                var f = g.First();
                w.BraceIndent($"if (type == typeof({g.Key}))", () =>
                {
                    foreach (var gg in g.GroupBy(f => f.InputType!.GetFullName(true)))
                    {
                        w.BraceIndent($"if (optionsType == typeof({gg.Key}))", () =>
                        {
                            w.BraceIndent($"static object create(IIocSession session, object opt)", () =>
                            {
                                if (f.Params.Any(p => p.FuncReturn is not null))
                                {
                                    w.WriteLine("var scope = session.Scope;");
                                }
                                w.WriteParenthesesWrapped($"return new {g.Key}", f.Params, p => WriteIocFactoryParam(w, "session", p, "", gg.Key));
                                w.WriteLine(";");
                            });
                            w.WriteLine("return create;");
                        });
                    }
                });
            }
        }

        private void WriteIocFactoryParam(CsWriter w, string serviceLocator, IocCtorParam p, string iocFacParam, string optType = "")
        {
            if (p.IsInput)
            {
                w.Write($"({optType})opt");
            }
            else if (p.FuncReturn is null)
            {
                w.Write($"{serviceLocator}.{(p.Required ? "Get" : p.IsValueType ? "TryGetNullable" : "TryGet")}<{p.NonnullTypeName}>({iocFacParam})");
            }
            else
            {
                if (p.FuncParam is null)
                {
                    w.WriteLine($"{serviceLocator}.TryGet<{p.NonnullTypeName}>({iocFacParam}) ?? (() =>")
                        .Indent(() => WriteIocFactoryParam(w, "scope", p.FuncReturn, iocFacParam)).Write(")");
                }
                else
                {
                    w.WriteLine($"{serviceLocator}.TryGet<{p.NonnullTypeName}>() ?? (a =>")
                        .Indent(() => WriteIocFactoryParam(w, "scope", p.FuncReturn, "a")).Write(")");
                }
            }
        }

        private void WriteWirers(CsWriter w)
        {
            w.BraceIndent($"static class Wirers", () =>
            {
                foreach (var gn in Info.Wirers.GroupBy<IocCtorInfo, string>(f => f.GetWirerName(false)))
                {
                    var groupsByType = gn.GroupBy(f => f.ServiceType.GetFullName(true));
                    bool fullname = groupsByType.Count() > 1;
                    foreach (var gt in groupsByType)
                    {
                        var f = gt.First();
                        var wirerClassName = fullname ? f.GetWirerName(true) : gn.Key;
                        w.BraceIndent($"internal class {wirerClassName} : IIocWirer<{f.ServiceType.GetFullName(true)}>", () =>
                        {
                            w.WriteLines(f.Params.Select(p => $"public WirerParameter<{p.Type.GetFullName(true)}>{(p.Required ? "" : "?")} {p.Name} {{ get; set; }} = WirerParameter<{p.Type.GetFullName(true)}>.Unspecified;"));
                            w.BraceIndent("public object Create(IIocSession session)", () =>
                            {
                                if (f.Params.Any(p => p.FuncReturn is not null))
                                {
                                    w.WriteLine("var scope = session.Scope;");
                                }
                                w.WriteParenthesesWrapped($"return new {f.ServiceType.GetFullName(true)}", f.Params, p => WriteWirerParam(w, p));
                                w.WriteLine(";");
                            });
                        });
                    }
                }
            });
        }

        private void WriteWirerParam(CsWriter w, IocCtorParam p)
        {
            if (p.FuncReturn is null)
            {
                if (p.Required)
                {
                    w.Write($"{p.Name}.HasValue ? {p.Name}.Value : ").WriteLine()
                        .Indent(() => w.Write($"session.Get<{p.NonnullTypeName}>({p.Name}.FactoryParameter)"));
                }
                else
                {
                    w.Write($"{p.Name} is null ? null : {p.Name}.HasValue ? {p.Name}.Value : ").WriteLine()
                        .Indent(() => w.Write($"session.{(p.IsValueType ? "TryGetNullable" : "TryGet")}<{p.NonnullTypeName}>({p.Name}.FactoryParameter)"));
                }
            }
            else if (p.FuncParam is null)
            {
                if (!p.Required)
                {
                    w.Write($"{p.Name} is null ? null : ");
                }
                w.Write($"{p.Name}.HasValue ? {p.Name}.Value : ").WriteLine()
                    .Indent(() => w.Write($"(session.TryGet<{p.NonnullTypeName}>({p.Name}.FactoryParameter) ?? (() =>").WriteLine()
                        .Indent(() => WriteIocFactoryParam(w, "scope", p.FuncReturn, $"{p.Name}.FactoryParameter")).Write("))"));
            }
            else
            {
                if (!p.Required)
                {
                    w.Write($"{p.Name} is null ? null : ");
                }
                w.Write($"{p.Name}.HasValue ? {p.Name}.Value : ").WriteLine()
                    .Indent(() => w.Write($"(session.TryGet<{p.NonnullTypeName}>({p.Name}.FactoryParameter) ?? (a =>").WriteLine()
                        .Indent(() => WriteIocFactoryParam(w, "scope", p.FuncReturn, "a")).Write("))"));
            }
        }
    }
}