using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hndy
{
    public class GenBench
    {
        readonly ISourceGenerator[] _generators;
        readonly List<MetadataReference> _metadataReferences = new List<MetadataReference>();

        public GeneratorRunResult? Result { get; private set; }
        Compilation? _resultCompilation;
        public IReadOnlyCollection<Diagnostic>? CompilationDiagnostics => _resultCompilation?.GetDiagnostics();
        public IReadOnlyCollection<Diagnostic>? GeneratorDiagnostics => Result?.Diagnostics;

        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;
        public NullableContextOptions NullableContextOptions { get; set; } = NullableContextOptions.Enable;

        public GenBench(ISourceGenerator generator, bool addAppDomainReferences = true)
        {
            _generators = new[] { generator };

            if (addAppDomainReferences)
            {
                _metadataReferences.AddRange(AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic && File.Exists(a.Location))
                    .Select(a => MetadataReference.CreateFromFile(a.Location)));
            }
        }

        public void AddReference(string assemblyFile)
        {
            _metadataReferences.Add(MetadataReference.CreateFromFile(assemblyFile));
        }

        public void AddReference<T>()
        {
            _metadataReferences.Add(MetadataReference.CreateFromFile(typeof(T).Assembly.Location));
        }

        public void Run(IEnumerable<string> codes)
        {
            var parseOpt = new CSharpParseOptions(LanguageVersion);
            var compileOpt = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions);
            var compilation = CSharpCompilation.Create("HndyBench",
                    codes.Select(t => CSharpSyntaxTree.ParseText(t, parseOpt)),
                    _metadataReferences,
                    compileOpt);
            var driver = CSharpGeneratorDriver.Create(_generators, parseOptions: parseOpt);
            Result = driver.RunGeneratorsAndUpdateCompilation(compilation, out _resultCompilation, out _)
                .GetRunResult()
                .Results[0];
        }

        public void AssertEmptyDiagnostics(DiagnosticSeverity lowestSeverity = DiagnosticSeverity.Info)
        {
            if (Result is null || _resultCompilation is null)
            {
                throw new Exception("Compile failed or haven't run.");
            }

            var diags = Result.Value.Diagnostics.Concat(_resultCompilation.GetDiagnostics())
                .Where(d => d.Severity >= lowestSeverity)
                .ToList();
            if (diags.Count > 0)
            {
                throw new Exception($"{diags.Count} errors:{Environment.NewLine}" + string.Join(Environment.NewLine, diags));
            }
        }

        public static IEnumerable<string> LoadSourceFiles(string projName, Func<FileInfo, bool> filter)
        {
            var dir = new DirectoryInfo(Path.Combine(GetSolutionDir(), $"Hndy.{projName}.Tests"));
            return dir.GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .Where(filter)
                .Select(f =>
                {
                    using var reader = f.OpenText();
                    return reader.ReadToEnd();
                });
        }

        public static string LoadSourceFile(string projName, string fileName)
        {
            using var reader = new FileInfo(Path.Combine(GetSolutionDir(), $"Hndy.{projName}.Tests", $"{fileName}.cs")).OpenText();
            return reader.ReadToEnd();
        }

        private static string GetSolutionDir()
        {
            for(var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); 
                dir is not null; 
                dir = Path.GetDirectoryName(dir))
            {
                if(File.Exists(Path.Combine(dir, "hndy-ioc.sln")))
                {
                    return dir;
                }
            }
            throw new Exception("Get solution directory failed.");
        }
    }
}
