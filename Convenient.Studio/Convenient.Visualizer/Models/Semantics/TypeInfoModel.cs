using Microsoft.CodeAnalysis;

namespace Convenient.Visualizer.Models.Semantics;

public class TypeInfoModel
{
    public TypeSymbolModel TypeSymbol { get; }
    public TypeSymbolModel ConvertedType { get; }

    public TypeInfoModel(TypeInfo typeInfo)
    {
        TypeSymbol = typeInfo.Type == null ? null : new TypeSymbolModel(typeInfo.Type);
        ConvertedType = typeInfo.ConvertedType == null ? null : new TypeSymbolModel(typeInfo.ConvertedType);
    }
}