using Microsoft.CodeAnalysis;
using System;

namespace Hndy.Ioc.Gen
{
    [Generator]
    public class IocRegGenerator : SourceGenerator<IocRegAnalyzer>
    {
        protected override string? GeneratorIdentifier { get; } = $"Hndy.Ioc v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
    }
}
