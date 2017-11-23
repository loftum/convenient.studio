using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Convenient.Studio.Controls
{
    public class CompletionData : ICompletionData
    {
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            if (string.IsNullOrEmpty(Prefix))
            {
                textArea.Document.Replace(completionSegment, Completion);
                return;
            }
            var segment = new TextSegment
            {
                StartOffset = completionSegment.Offset - Prefix.Length,
                EndOffset = completionSegment.EndOffset,
                Length = completionSegment.Length + Prefix.Length
            };
            textArea.Document.Replace(segment, Text);
        }

        public CompletionData(string prefix, string completion, object description)
        {
            Prefix = prefix;
            Completion = completion;
            Description = description;
        }

        public string Prefix { get; }
        public string Completion { get; }

        public ImageSource Image => null;
        public string Text => $"{Prefix}{Completion}";
        public object Content => Text;
        public object Description { get; }
        public double Priority { get; set; }
    }
}