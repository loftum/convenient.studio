using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Convenient.Studio.Scripting.Completion;

public class CodeAnalyzer
{
    public SemanticModel SemanticModel { get; }

    public SyntaxTree SyntaxTree { get; }

    public CodeAnalyzer(SyntaxTree tree, Compilation compilation)
    {
        SyntaxTree = tree;
        var semantics = compilation.GetSemanticModel(tree);
        SemanticModel = semantics;
    }

    public ISymbol GetSymbolAt(int location)
    {
        var nodeOrToken = SyntaxTree
            .GetRoot()
            .GetMostSpecificNodeOrTokenAt(location > 0 ? location - 1 : 0);

        while (nodeOrToken != null && nodeOrToken.AsNode() == null)
        {
            nodeOrToken = nodeOrToken.Parent;
        }
        
        var node = nodeOrToken.AsNode();
        if (node != null)
        {
            SemanticModel.GetSymbolInfo(node);    
        }

        return null;
    }

    public IEnumerable<CompletionThing> GetCompletionsAt(int location)
    {
        var completionSyntax = GetCompletionSyntax(SyntaxTree.GetRoot().GetMostSpecificNodeOrTokenAt(location > 0 ? location - 1 : 0));
        var prefix = completionSyntax.Prefix?.GetText().ToString() ?? "";
        var symbols = SemanticModel.GetCompletionSymbols(location, completionSyntax);
        
        return symbols
            .Where(s => s.Name.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
            .Select(s => GetCompletionThing(s, prefix.Length));
    }

    private static CompletionThing GetCompletionThing(ISymbol s, int location)
    {
        return new CompletionThing(s.Name[..location],
            s.ToDisplayString(DisplayFormats.CompletionFormat)[location..],
            s.ToDisplayString(DisplayFormats.ContentFormat),
            s.ToDisplayString(DisplayFormats.DescriptionFormat));
    }

    private static CompletionSyntax GetCompletionSyntax(SyntaxNodeOrToken nodeOrToken)
    {
        SyntaxNodeOrToken dot;
        SyntaxNode prefix = null;

        if (nodeOrToken.IsNode)
        {
                
            prefix = nodeOrToken.AsNode();
            dot = nodeOrToken.GetPreviousSibling();
            if (dot.Kind() != SyntaxKind.DotToken)
            {
                return new CompletionSyntax(null, prefix);
            }
        }
        else
        {
            if (nodeOrToken.Kind() != SyntaxKind.DotToken)
            {
                return new CompletionSyntax(null, null);
            }
            dot = nodeOrToken;
        }

        var previous = dot.GetPreviousSibling();
        return previous.IsNode ? new CompletionSyntax(previous.AsNode(), prefix) : new CompletionSyntax(null, prefix);
    }
}