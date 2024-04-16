using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using Convenient.Studio.Scripting;
using Convenient.Studio.UI;

namespace Convenient.Studio.Controls;

public class SemanticColorizer : DocumentColorizingTransformer
{
    private readonly TextEditor _editor;
    private readonly ICompletionProvider _completionProvider;

    public SemanticColorizer(TextEditor editor, ICompletionProvider completionProvider)
    {
        _editor = editor;
        _completionProvider = completionProvider;
    }

    protected override void ColorizeLine(DocumentLine line)
    {
        var text = _editor.Text.Substring(line.Offset, line.Length);
        var script = _completionProvider.ScriptState.Script.ContinueWith(text);
        script.Compile();
        var compilation = script.GetCompilation();
        var tree = compilation.SyntaxTrees.Single();
        var semantics = compilation.GetSemanticModel(compilation.SyntaxTrees.Single());
        foreach (var node in tree.GetRoot().DescendantNodesAndTokens())
        {
            var symbolType = semantics.GetSyntaxHighlightType(node);

            var start = line.Offset + node.SpanStart;
            var end = line.Offset + node.Span.End;
            switch (symbolType)
            {
                case SyntaxHighlightType.Event:
                    ChangeLinePart(start, end, MakeColor(Brushes.Magenta));
                    break;
                case SyntaxHighlightType.Field:
                case SyntaxHighlightType.Property:
                    ChangeLinePart(start, end, MakeColor(Brushes.YellowGreen));
                    break;
                case SyntaxHighlightType.Method:
                    ChangeLinePart(start, end, MakeColor(Brushes.WhiteSmoke, FontWeight.Bold));
                    break;
                case SyntaxHighlightType.ExtensionMethod:
                    ChangeLinePart(start, end, MakeColor(Brushes.WhiteSmoke, FontWeight.Bold, FontStyle.Italic));
                    break;
                case SyntaxHighlightType.Class:
                    ChangeLinePart(start, end, MakeColor(CustomBrush.ClassOrange));
                    break;
                case SyntaxHighlightType.StaticClass:
                    ChangeLinePart(start, end, MakeColor(CustomBrush.StaticClassMagenta));
                    break;
                case SyntaxHighlightType.Interface:
                    ChangeLinePart(start, end, MakeColor(Brushes.Cyan));
                    break;
                case SyntaxHighlightType.Enum:
                    ChangeLinePart(start, end, MakeColor(Brushes.SandyBrown));
                    break;
                case SyntaxHighlightType.Struct:
                    ChangeLinePart(start, end, MakeColor(CustomBrush.StructBrown));
                    break;
                case SyntaxHighlightType.Delegate:
                    ChangeLinePart(start, end, MakeColor(Brushes.PaleGreen));
                    break;
                case SyntaxHighlightType.Alias:
                    ChangeLinePart(start, end, MakeColor(Brushes.CornflowerBlue));
                    break;
                case SyntaxHighlightType.Namespace:
                    ChangeLinePart(start, end, MakeColor(Brushes.DeepPink));
                    break;
                case SyntaxHighlightType.Constant:
                    ChangeLinePart(start, end, MakeColor(Brushes.Yellow));
                    break;
                case SyntaxHighlightType.Error:
                    ChangeLinePart(start, end, MakeColor(Brushes.Red));
                    break;
            }
        }
    }

    private static Action<VisualLineElement> MakeColor(IBrush brush, FontWeight? weight = null, FontStyle? fontstyle = null)
    {
        return e =>
        {
            e.TextRunProperties.SetForegroundBrush(brush);
            e.TextRunProperties.SetTypeface(new Typeface(
                e.TextRunProperties.Typeface.FontFamily,
                fontstyle.GetValueOrDefault(e.TextRunProperties.Typeface.Style),
                weight.GetValueOrDefault(e.TextRunProperties.Typeface.Weight))
            );
        };
    }
}