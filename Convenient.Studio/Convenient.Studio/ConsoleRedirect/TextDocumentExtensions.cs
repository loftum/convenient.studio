using AvaloniaEdit.Document;

namespace Convenient.Studio.ConsoleRedirect;

internal static class TextDocumentExtensions
{
    public static void Append(this TextDocument document, string text)
    {
        document.Insert(document.TextLength, text);
    }
        
    public static void AppendLine(this TextDocument document, string text)
    {
        document.Insert(document.TextLength, $"{text}{Environment.NewLine}");
    }
        
    public static void AppendLine(this TextDocument document)
    {
        document.Insert(document.TextLength, Environment.NewLine);
    }

    public static Task AppendAsync(this TextDocument document, string text)
    {
        document.Append(text);
        return Task.CompletedTask;
    }
        
    public static Task AppendLineAsync(this TextDocument document)
    {
        document.Insert(document.TextLength, Environment.NewLine);
        return Task.CompletedTask;
    }
        
    public static Task AppendLineAsync(this TextDocument document, string text)
    {
        document.Insert(document.TextLength, $"{text}{Environment.NewLine}");
        return Task.CompletedTask;
    }
}