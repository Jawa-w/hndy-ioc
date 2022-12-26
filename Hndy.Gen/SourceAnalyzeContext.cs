using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Threading;

namespace Hndy
{
    public interface ISourceAnalyzeContext
    {
        SemanticModel SemanticModel { get; }
        CancellationToken CancellationToken { get; }

        void ReportDiagnostic(string id, DiagnosticSeverity severity, int warningLevel, IEnumerable<Location> locations);
    }
}