using Avalonia.Media;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;

namespace Convenient.Studio.ViewModels;

public class CompletionData : ICompletionData
{
    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
        var prefixLength = Prefix.Length;
        if (prefixLength > 0)
        {
            // Corrects casing on prefix
            // Entered prefix: sso
            // Prefix: Sso
            // Completion: Session
            // Result: SsoSession instead of ssoSession
            var offset = completionSegment.Offset - prefixLength;
            textArea.Document.Replace(offset, completionSegment.Length + prefixLength, Text);
            return;
        }
        textArea.Document.Replace(completionSegment, Completion);
    }
        
    public CompletionData(string prefix, string completion, object description)
    {
        Prefix = prefix;
        Completion = completion;
        Description = description;
    }
        
    public string Prefix { get; }
    public string Completion { get; }

    public IImage Image { get; set; }
    public string Text => $"{Prefix}{Completion}";
    public object Content => Text;
    public object Description { get; }
    public double Priority { get; set; }
}