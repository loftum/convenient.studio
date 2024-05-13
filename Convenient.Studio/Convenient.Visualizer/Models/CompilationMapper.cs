using Convenient.Visualizer.Models.Emit;
using Convenient.Visualizer.Models.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Convenient.Visualizer.Models;

public class CompilationMapper
{
    public static CSharpCompilationModel Map(CSharpCompilation compilation)
    {
        return new CSharpCompilationModel
        {
            Language = compilation.Language,
            LanguageVersion = compilation.LanguageVersion,
            SyntaxTrees = compilation.SyntaxTrees.Select(SyntaxMapper.Map).ToArray(),
            References = compilation.References.Select(Map).ToArray(),
            Diagnoscics = compilation.GetDiagnostics().Select(Map).ToArray()
        };
    }

    public static MetadataReferencesModel Map(MetadataReference reference)
    {
        return new MetadataReferencesModel
        {
            Display = reference.Display
        };
    }

    public static DiagnosticModel Map(Diagnostic diagnostic)
    {
        return new DiagnosticModel
        {
            Id = diagnostic.Id,
            Severity = diagnostic.Severity,
            Location = Map(diagnostic.Location),
            Message = diagnostic.ToString()
        };
    }

    private static LocationModel Map(Location location)
    {
        return new LocationModel
        {
            Kind = location.Kind,
            SourceSpan = location.SourceSpan
        };
    }

    public static EmitResultModel Map(EmitResult result)
    {
        return new EmitResultModel
        {
            Success = result.Success,
            Diagnostics = result.Diagnostics.Select(Map).ToArray()
        };
    }
}