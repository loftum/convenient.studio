using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Convenient.Visualizer.Models.Syntax;

public class SyntaxMapper
{
    public static SyntaxTreeModel Map(SyntaxTree tree)
    {
        return new SyntaxTreeModel
        {
            Tree = tree,
            Root = Map(tree.GetRoot())
        };
    }

    public static object Map(SyntaxNodeOrToken nodeOrToken)
    {
        return nodeOrToken.IsNode ? (object)Map(nodeOrToken.AsNode()) : Map(nodeOrToken.AsToken());
    }

    public static SyntaxTokenModel Map(SyntaxToken t)
    {
        return new SyntaxTokenModel
        {
            Kind = t.Kind().ToString(),
            Text = t.Text,
            IsMissing = t.IsMissing
        };
    }

    public static SyntaxNodeModel Map(SyntaxNode node)
    {
        return node == null ? null : DoMap((dynamic) node);
    }

    private static SyntaxNodeModel DoMap(SyntaxNode node)
    {
        var model = new SyntaxNodeModel
        {
            LeadingTrivia = node.GetLeadingTrivia().Select(t => new SyntaxTriviaModel(t)).ToList(),
            Type = node.GetType().Name,
            Kind = node.Kind().ToString(),
            Text = node.GetText().ToString(),
            TrailingTrivia = node.GetTrailingTrivia().Select(t => new SyntaxTriviaModel(t)).ToList(),
            IsMissing = node.IsMissing,
            ChildNodes = node.ChildNodes().Select(Map).ToList(),
            ChildTokens = node.ChildTokens().Select(Map).ToList(),
        };
        return model;
    }
}