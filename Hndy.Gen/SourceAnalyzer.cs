using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;

namespace Hndy
{
    public abstract class SourceAnalyzer : ISyntaxContextReceiver, ISourceAnalyzeContext
    {
        readonly List<Diagnostic> _list = new();
        public IEnumerable<Diagnostic> Diagnostics => _list;

        private readonly ResourceManager? _diagResourceManager;
        private readonly Type? _diagResourceSource;

        public SemanticModel? SemanticModel { get; set; }
        SemanticModel ISourceAnalyzeContext.SemanticModel => SemanticModel!;
        public CancellationToken CancellationToken { get; internal set; }

        internal List<ISourceBuilder> SourceBuilders { get; } = new List<ISourceBuilder>();

        public SourceAnalyzer()
        {
        }

        public SourceAnalyzer(ResourceManager diagResourceManager, Type diagResourceSource)
        {
            _diagResourceManager = diagResourceManager;
            _diagResourceSource = diagResourceSource;
        }

        public abstract void OnVisitSyntaxNode(GeneratorSyntaxContext context);
        public virtual void OnExecute(GeneratorExecutionContext context) { }

        protected void AddBuilder(ISourceBuilder builder)
        {
            SourceBuilders.Add(builder);
        }

        public void ReportDiagnostic(string id, DiagnosticSeverity severity, int warningLevel, Location location)
        {
            _list.Add(Diagnostic.Create(id, "Compiler",
                new LocalizableResourceString(id,
                    _diagResourceManager ?? throw new InvalidOperationException("No diagnostic localization resource."),
                    _diagResourceSource ?? throw new InvalidOperationException("No diagnostic localization resource.")),
                severity, severity, true, warningLevel,
                location: location));
        }

        public void ReportDiagnostic(string id, string[] formatArgs, DiagnosticSeverity severity, int warningLevel, Location location)
        {
            _list.Add(Diagnostic.Create(id, "Compiler",
                new LocalizableResourceString(id,
                    _diagResourceManager ?? throw new InvalidOperationException("No diagnostic localization resource."),
                    _diagResourceSource ?? throw new InvalidOperationException("No diagnostic localization resource."),
                    formatArgs),
                severity, severity, true, warningLevel,
                location: location));
        }

        public void ReportDiagnostic(string id, DiagnosticSeverity severity, int warningLevel, IEnumerable<Location> locations)
        {
            _list.Add(Diagnostic.Create(id, "Compiler",
                new LocalizableResourceString(id,
                    _diagResourceManager ?? throw new InvalidOperationException("No diagnostic localization resource."),
                    _diagResourceSource ?? throw new InvalidOperationException("No diagnostic localization resource.")),
                severity, severity, true, warningLevel,
                location: locations?.FirstOrDefault(),
                additionalLocations: locations?.Skip(1)));
        }

        public void ReportDiagnostic(string id, string[] formatArgs, DiagnosticSeverity severity, int warningLevel, IEnumerable<Location> locations)
        {
            _list.Add(Diagnostic.Create(id, "Compiler",
                new LocalizableResourceString(id,
                    _diagResourceManager ?? throw new InvalidOperationException("No diagnostic localization resource."),
                    _diagResourceSource ?? throw new InvalidOperationException("No diagnostic localization resource."),
                    formatArgs),
                severity, severity, true, warningLevel,
                location: locations?.FirstOrDefault(),
                additionalLocations: locations?.Skip(1)));
        }
    }
}
