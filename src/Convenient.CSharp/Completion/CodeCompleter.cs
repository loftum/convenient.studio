using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Convenient.CSharp.Completion
{
    public class CodeCompleter
    {
        private readonly IEnumerable<ISymbol> _symbols;
        private readonly string _prefix;

        public CodeCompleter(SyntaxTree tree, Compilation compilation, int location)
        {
            var semantics = compilation.GetSemanticModel(tree);
            var nodes = GetCompletionSyntax(tree.GetRoot().GetMostSpecificNodeOrTokenAt(location > 0 ? location - 1 : 0));
            _prefix = nodes.Prefix?.GetText().ToString() ?? "";
            _symbols = semantics.GetCompletionSymbols(location, nodes);
        }

        public IEnumerable<CompletionThingy> GetCompletions()
        {
            return _symbols
                .Where(s => s.Name.StartsWith(_prefix, StringComparison.InvariantCultureIgnoreCase))
                .Select(s => new CompletionThingy(s.Name.Substring(0, _prefix.Length),
                    s.ToDisplayString(DisplayFormats.CompletionFormat).Substring(_prefix.Length),
                    s.ToDisplayString(DisplayFormats.ContentFormat),
                    s.ToDisplayString(DisplayFormats.DescriptionFormat))
                );
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
                    return new CompletionSyntax(null, prefix);
                }
                dot = nodeOrToken;
            }

            var previous = dot.GetPreviousSibling();
            return previous.IsNode ? new CompletionSyntax(previous.AsNode(), prefix) : new CompletionSyntax(null, prefix);
        }
    }
}