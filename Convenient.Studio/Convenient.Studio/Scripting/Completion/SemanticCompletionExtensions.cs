using Microsoft.CodeAnalysis;

namespace Convenient.Studio.Scripting.Completion
{
    public static class SemanticCompletionExtensions
    {
        public static IEnumerable<ISymbol> GetCompletionSymbols(this SemanticModel semantics, int location, CompletionSyntax syntax)
        {
            if (syntax.Container == null)
            {
                return semantics.LookupSymbols(location);
            }

            var symbolInfo = semantics.GetSymbolInfo(syntax.Container);
            if (symbolInfo.Symbol is INamespaceOrTypeSymbol namespaceOrType)
            {
                return semantics.LookupSymbols(location, namespaceOrType).Where(s => namespaceOrType.IsNamespace || s.IsStatic);
            }
            var typeInfo = semantics.GetTypeInfo(syntax.Container);
            var type = typeInfo.ConvertedType ?? typeInfo.Type;
            return type == null
                ? semantics.LookupSymbols(location)
                : semantics.LookupSymbols(location, type, includeReducedExtensionMethods : true).Where(s => !s.IsStatic);
        }
    }
}