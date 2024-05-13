using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Convenient.Visualizer.Models.Syntax;

public struct SyntaxTriviaModel
{
    public SyntaxKind Kind { get; set; }
    public string Text { get; set; }

    public SyntaxTriviaModel(SyntaxTrivia trivia)
    {
        Kind = trivia.Kind();
        Text = trivia.ToString();
    }
}