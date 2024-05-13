using Convenient.Visualizer.Models.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Convenient.Visualizer.Models;

public class CSharpCompilationModel
{
    public string Language { get; set; }
    public LanguageVersion LanguageVersion { get; set; }
    public DiagnosticModel[] Diagnoscics { get; set; } = [];
    public SyntaxTreeModel[] SyntaxTrees { get; set; } = [];
    public MetadataReferencesModel[] References { get; set; } = [];
}