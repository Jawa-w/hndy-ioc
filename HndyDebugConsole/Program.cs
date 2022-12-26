using Hndy;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HndyDebugConsole
{
    class Program
    {
        static void Main()
        {
            ISourceGenerator gen = new Hndy.Ioc.Gen.IocRegGenerator();
            string projName = "Ioc";
            string[] testNames = new[] {
                "FuncWiring",
            };

            var bench = new GenBench(gen);
            bench.LanguageVersion = LanguageVersion.CSharp10;
            bench.AddReference(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", $"Hndy.{projName}.dll"));
            bench.AddReference<System.ComponentModel.INotifyPropertyChanged>();
            bench.Run(testNames.Select(t => GenBench.LoadSourceFile(projName, t)));

            PrintResults(bench);
            Console.ReadKey(true);
        }

        private static void PrintResults(GenBench bench)
        {
            foreach (var s in bench.Result?.GeneratedSources ?? Enumerable.Empty<GeneratedSourceResult>())
            {
                Console.WriteLine();
                Console.WriteLine($"--- {s.HintName} ---");
                Console.WriteLine();
                Console.WriteLine(s.SourceText);
            }
            Console.WriteLine();
            Console.WriteLine($"----");
            foreach (var d in bench.GeneratorDiagnostics ?? Enumerable.Empty<Diagnostic>())
            {
                PrintDiagnostic(d);
            }
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine($"----");
            foreach (var d in bench.CompilationDiagnostics ?? Enumerable.Empty<Diagnostic>())
            {
                PrintDiagnostic(d);
            }
            Console.ResetColor();
        }

        private static void PrintDiagnostic(Diagnostic d)
        {
            if (d.Severity > DiagnosticSeverity.Hidden)
            {
                Console.ForegroundColor = d.Severity switch
                {
                    DiagnosticSeverity.Error => ConsoleColor.Red,
                    _ => ConsoleColor.Yellow,
                };
                Console.WriteLine(" - " + d.ToString());
            }
        }
    }
}
