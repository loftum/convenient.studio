using AvaloniaEdit;
using Microsoft.CodeAnalysis.Text;

namespace Convenient.Studio.Controls;

public static class TextEditorExtensions
{
    public static CodeSpan GetCodeSpan(this TextEditor editor)
    {
        var caret = editor.TextArea.Caret;
        return editor.GetCodeSpanAt(caret.Offset);
    }

    public static CodeSpan GetCodeSpanAt(this TextEditor editor, int offset)
    {
        if (offset <= 0)
        {
            return new CodeSpan();
        }
        var startCandidate = offset - 1;
        var start = startCandidate;
        var lineFeeds = 0;
        while (startCandidate >= 0)
        {
            var c = editor.Text[startCandidate];
            if (!char.IsWhiteSpace(c))
            {
                start = startCandidate;
            }
            if (c == '\n')
            {
                if (lineFeeds > 0)
                {
                    break;
                }
                lineFeeds++;
            }
            else if (!char.IsWhiteSpace(c))
            {
                lineFeeds = 0;
            }
            startCandidate--;
        }
        
        var endCandidate = offset;
        var end = endCandidate;
        lineFeeds = 0;
        while (endCandidate < editor.Text.Length)
        {
            var c = editor.Text[endCandidate];
            if (!char.IsWhiteSpace(c))
            {
                end = endCandidate;
            }
            if (c == '\n')
            {
                if (lineFeeds > 0)
                {
                    break;
                }
                lineFeeds++;
            }
            else if (!char.IsWhiteSpace(c))
            {
                lineFeeds = 0;
            }
            endCandidate++;
        }
        
        
        return CodeSpan.FromBounds(start, end, offset);
    }
    
    public static TextSpan GetCompletionSpan(this TextEditor editor)
    {
        var caret = editor.TextArea.Caret;
        return editor.GetCompletionSpanAt(caret.Offset);
    }

    public static TextSpan GetCompletionSpanAt(this TextEditor editor, int offset)
    {
        var caret = editor.TextArea.Caret;
        if (offset <= 0)
        {
            return new TextSpan();
        }
        var startCandidate = offset - 1;
        var start = startCandidate;
        var lineFeeds = 0;
        while (startCandidate >= 0)
        {
            var c = editor.Text[startCandidate];
            if (!char.IsWhiteSpace(c))
            {
                start = startCandidate;
            }
            if (c == '\n')
            {
                if (lineFeeds > 0)
                {
                    break;
                }
                lineFeeds++;
            }
            else if (!char.IsWhiteSpace(c))
            {
                lineFeeds = 0;
            }
            startCandidate--;
        }
        return TextSpan.FromBounds(start, caret.Offset);
    }
    
    public static string GetCompletionStatement(this TextEditor editor)
    {
        var span = editor.GetCodeSpan();
        return span.IsEmpty ? "" : editor.Text.Substring(span.Start, span.OffsetLength);
    }
}