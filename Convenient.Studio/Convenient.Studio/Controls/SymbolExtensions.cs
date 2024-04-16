using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convenient.Studio.Controls;

public static class SymbolExtensions
{
    public static SyntaxHighlightType GetSyntaxHighlightType(this SemanticModel semantics, SyntaxNodeOrToken nodeOrToken)
    {
        return nodeOrToken.IsNode
            ? semantics.GetSyntaxHighlightTypeForNode(nodeOrToken.AsNode())
            : semantics.GetSyntaxHighlightTypeForToken(nodeOrToken.AsToken());
    }
        
    public static SyntaxHighlightType GetSyntaxHighlightTypeForToken(this SemanticModel semantics, SyntaxToken token)
    {
        //Console.WriteLine($"token {token.Kind()} {token}");
        switch (token.Kind())
        {
            case SyntaxKind.IdentifierToken: {
                var parent = token.Parent ;
                if (parent == null)
                {
                    break;
                }
                
                //Console.WriteLine($"  {parent.Kind()} {parent}");
                switch (parent.Kind())
                {
                    case SyntaxKind.ClassDeclaration:
                        return SyntaxHighlightType.Class;
                    case SyntaxKind.InterfaceDeclaration:
                        return SyntaxHighlightType.Interface;
                    case SyntaxKind.EnumDeclaration:
                        return SyntaxHighlightType.Enum;
                    case SyntaxKind.StructDeclaration:
                        return SyntaxHighlightType.Struct;
                    case SyntaxKind.PropertyDeclaration:
                        return SyntaxHighlightType.Property;
                    case SyntaxKind.FieldDeclaration:
                        return SyntaxHighlightType.Field;
                    case SyntaxKind.VariableDeclarator:
                        return SyntaxHighlightType.Field;
                    case SyntaxKind.EventFieldDeclaration:
                        return SyntaxHighlightType.Event;
                }
                
                break;
            }
        }

        return SyntaxHighlightType.Unknown;
    }
        
    public static SyntaxHighlightType GetSyntaxHighlightTypeForNode(this SemanticModel semantics, SyntaxNode node)
    {
        //Console.WriteLine($"{node.Kind()} {node}");
        if (node.IsMissing)
        {
            return SyntaxHighlightType.Error;
        }
        var kind = node.Kind();
        if (kind.ToString().EndsWith("Keyword"))
        {
            return SyntaxHighlightType.Keyword;
        }
        switch (kind)
        {
            case SyntaxKind.PredefinedType:
            case SyntaxKind.TypeVarKeyword:
                return SyntaxHighlightType.Keyword;
            case SyntaxKind.IdentifierName:
                var i = (IdentifierNameSyntax) node;
                if (i.IsVar)
                {
                    return SyntaxHighlightType.Keyword;
                }
                break;
        }

        var symbol = semantics.GetSymbolInfo(node).Symbol;
        if (symbol == null)
        {
            return SyntaxHighlightType.Unknown;
        }

        switch (symbol.Kind)
        {
            case SymbolKind.Property: return SyntaxHighlightType.Property;
            case SymbolKind.Field: return SyntaxHighlightType.Field;
            case SymbolKind.Method:
                switch (node.Kind())
                {
                    case SyntaxKind.GenericName:
                    case SyntaxKind.IdentifierName:
                        var method = (IMethodSymbol)symbol;
                        return method.IsExtensionMethod ? SyntaxHighlightType.ExtensionMethod : SyntaxHighlightType.Method;
                }
                break;
            case SymbolKind.Alias:
                return SyntaxHighlightType.Alias;
            case SymbolKind.Event:
                return SyntaxHighlightType.Event;
            case SymbolKind.NamedType:
                var namedType = (INamedTypeSymbol)symbol;
                //Console.WriteLine($"  {namedType.Kind} {namedType.TypeKind} {namedType}");
                if (namedType.IsType)
                {
                    switch (namedType.TypeKind)
                    {
                        case TypeKind.Class:
                            return namedType.IsStatic ? SyntaxHighlightType.StaticClass : SyntaxHighlightType.Class;
                        case TypeKind.Interface:
                            return SyntaxHighlightType.Interface;
                        case TypeKind.Enum:
                            return SyntaxHighlightType.Enum;
                        case TypeKind.Struct:
                            return SyntaxHighlightType.Struct;
                        case TypeKind.Delegate:
                            return SyntaxHighlightType.Delegate;
                    }
                    if (namedType.IsNamespace)
                    {
                        return SyntaxHighlightType.Namespace;
                    }

                    if (namedType.IsValueType)
                    {
                        return SyntaxHighlightType.ValueType;
                    }
                }
                break;
        }
        return SyntaxHighlightType.Unknown;
    }
}