using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Resources;
using System.Text;

namespace Hndy
{
    public interface ISourceBuilder
    {
        string FileName { get; }

        void Build(CsWriter writer, GeneratorExecutionContext context);
    }
}
