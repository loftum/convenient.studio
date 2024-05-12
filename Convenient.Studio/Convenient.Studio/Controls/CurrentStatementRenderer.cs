using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Rendering;

namespace Convenient.Studio.Controls;

public class CurrentStatementRenderer : IBackgroundRenderer
{
    public Pen BorderPen { get; set; } = null;// new Pen(Brushes.DarkGray, 1);
    public ISolidColorBrush BackgroundBrush { get; set; } = new SolidColorBrush(Colors.DarkGray, .2);

    public KnownLayer Layer => KnownLayer.Caret;

    private readonly TextEditor _editor;

    public CurrentStatementRenderer(TextEditor editor)
    {
        _editor = editor;
    }

    public void Draw(TextView textView, DrawingContext drawingContext)
    {
        var span = _editor.GetCodeSpan();
        if (span.IsEmpty)
        {
            return;
        }

        var builder = new BackgroundGeometryBuilder();

        builder.AddSegment(textView, new HighlightedSection {Offset = span.Start, Length = span.Length});

        var geometry = builder.CreateGeometry();
        if (geometry != null)
        {
            drawingContext.DrawGeometry(BackgroundBrush, BorderPen, geometry);
        }
    }
}