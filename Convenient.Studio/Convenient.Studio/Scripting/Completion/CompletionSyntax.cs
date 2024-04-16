using Microsoft.CodeAnalysis;

namespace Convenient.Studio.Scripting.Completion
{
    public struct CompletionSyntax
    {
        public SyntaxNode Container { get; }
        public SyntaxNode Prefix { get; }

        public CompletionSyntax(SyntaxNode container, SyntaxNode prefix)
        {
            Container = container;
            Prefix = prefix;
        }
    }
}