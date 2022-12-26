using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Hndy
{
    public abstract class SourceGenerator<TAnalyzer> : ISourceGenerator
        where TAnalyzer : SourceAnalyzer, new()
    {
        protected virtual string? GeneratorIdentifier => null;

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is SourceAnalyzer analyzer)
            {
                analyzer.CancellationToken = context.CancellationToken;
                analyzer.OnExecute(context);

                foreach (var gen in analyzer.SourceBuilders)
                {
                    var writer = new CsWriter(context.ParseOptions, GeneratorIdentifier);
                    gen.Build(writer, context);
                    context.AddSource(gen.FileName, writer.ToString());
                }
                foreach (var diag in analyzer.Diagnostics)
                {
                    context.ReportDiagnostic(diag);
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new TAnalyzer() { CancellationToken = context.CancellationToken });
        }
    }
}
