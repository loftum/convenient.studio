using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Convenient.Studio.Scripting.Completion;

public static class SyntaxNodeExtensions
{
    public static SyntaxNodeOrToken GetMostSpecificNodeOrTokenAt(this SyntaxNode root, int position)
    {
        SyntaxNodeOrToken specific = root;
        var descendants = root.DescendantNodesAndTokens();
        foreach (var node in descendants)
        {
            if (node.FullSpan.Covers(position) && node.FullSpan.IsMoreSpecificThan(specific.FullSpan))
            {
                specific = node;
            }
        }
        return specific;
    }

    public static SyntaxNode GetMostSpecificNodeAt(this SyntaxNode root, int position)
    {
        var specific = root;
        foreach (var node in root.DescendantNodes())
        {
            if (node.FullSpan.Covers(position) && node.FullSpan.IsMoreSpecificThan(specific.FullSpan))
            {
                specific = node;
            }
        }
        return specific;
    }

    public static bool Covers(this TextSpan span, int position)
    {
        return span.Start <= position && span.End > position;
    }

    public static bool IsMoreSpecificThan(this TextSpan span, TextSpan other)
    {
        return span.Start > other.Start && span.End <= other.End ||
               span.Start >= other.Start && span.End < other.End;
    }
}