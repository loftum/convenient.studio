using Microsoft.CodeAnalysis;

namespace Convenient.Visualizer.Models.Semantics;

public class NamedTypeSymbolModel : TypeSymbolModel
{
    public string[] MemberNames { get; }

    public NamedTypeSymbolModel(INamedTypeSymbol namedTypeSymbol) : base(namedTypeSymbol)
    {
        MemberNames = namedTypeSymbol.MemberNames.ToArray();
    }
}