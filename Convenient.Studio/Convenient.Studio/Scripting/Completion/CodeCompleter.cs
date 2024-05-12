using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Convenient.Studio.Scripting.Completion;

public class CodeCompleter
{
    public IEnumerable<ISymbol> Symbols { get; }
    private readonly string _prefix;
    private readonly int _location;
    private readonly SemanticModel _semanticModel;

    public CodeCompleter(SyntaxTree tree, Compilation compilation,  int location)
    {
        var semantics = compilation.GetSemanticModel(tree);
        var nodes = GetCompletionSyntax(tree.GetRoot().GetMostSpecificNodeOrTokenAt(location > 0 ? location - 1 : 0));
        _prefix = nodes.Prefix?.GetText().ToString() ?? "";
        Symbols = semantics.GetCompletionSymbols(location, nodes);
        _location = location;
        _semanticModel = semantics;
    }

    public IEnumerable<CompletionThing> GetCompletions()
    {
        return Symbols
            .Where(s => s.Name.StartsWith(_prefix, StringComparison.InvariantCultureIgnoreCase))
            .Select(GetCompletionThing);
    }

    private CompletionThing GetCompletionThing(ISymbol s)
    {
        return new CompletionThing(s.Name.Substring(0, _prefix.Length),
            s.ToDisplayString(DisplayFormats.CompletionFormat).Substring(_prefix.Length),
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