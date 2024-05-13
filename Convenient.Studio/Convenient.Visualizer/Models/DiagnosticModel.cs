using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Convenient.Visualizer.Models;

public class DiagnosticModel
{
    public DiagnosticSeverity Severity { get; set; }
    public string Id { get; set; }
    public LocationModel Location { get; set; }
    public string Message { get; set; }
}

public class LocationModel
{
    public LocationKind Kind { get; set; }
    public TextSpan SourceSpan { get; set; }
}