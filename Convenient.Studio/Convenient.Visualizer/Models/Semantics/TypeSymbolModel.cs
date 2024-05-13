using Microsoft.CodeAnalysis;

namespace Convenient.Visualizer.Models.Semantics;

public class TypeSymbolModel
{
    public TypeKind TypeKind { get; }
    public string Name { get; }
    public SpecialType SpecialType { get; }
    public bool IsReferenceType { get; }
    public bool IsValueType { get; }
    public NamedTypeSymbolModel BaseType { get; }

    public TypeSymbolModel(ITypeSymbol typeSymbol)
    {
        TypeKind = typeSymbol.TypeKind;
        Name = typeSymbol.Name;
        IsReferenceType = typeSymbol.IsReferenceType;
        IsValueType = typeSymbol.IsValueType;
        SpecialType = typeSymbol.SpecialType;
        BaseType = typeSymbol.BaseType == null ? null : new NamedTypeSymbolModel(typeSymbol.BaseType);
    }
}