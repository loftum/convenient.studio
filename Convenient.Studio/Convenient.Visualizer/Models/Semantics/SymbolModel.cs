using Microsoft.CodeAnalysis;

namespace Convenient.Visualizer.Models.Semantics;

public class SymbolModel
{
    public Accessibility DeclaredAccessibility { get; }
    public SymbolKind Kind { get; }
    public string Name { get; }
    public bool IsStatic { get; }

    public SymbolModel(ISymbol symbol)
    {
        DeclaredAccessibility = symbol.DeclaredAccessibility;
        Kind = symbol.Kind;
        Name = symbol.Name;
        IsStatic = symbol.IsStatic;
    }
}