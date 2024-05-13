using Microsoft.CodeAnalysis;

namespace Convenient.Visualizer.Models.Syntax;

public class SyntaxTreeModel
{
    public SyntaxTree Tree { get; set; }
    public SyntaxNodeModel Root { get; set; }
}