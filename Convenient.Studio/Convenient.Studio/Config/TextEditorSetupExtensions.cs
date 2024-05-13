using Avalonia.Input;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.Search;
using Convenient.Studio.Controls;
using Convenient.Studio.Scripting;
using Convenient.Studio.Syntax;
using Convenient.Studio.ViewModels;

namespace Convenient.Studio.Config;

public static class TextEditorSetupExtensions
{
    public static TextEditor SetDefaultOptions(this TextEditor editor)
    {
        editor.Options = new TextEditorOptions
        {
            ConvertTabsToSpaces = true,
            IndentationSize = 2,
            EnableRectangularSelection = true,
            HighlightCurrentLine = true,
            EnableHyperlinks = false,
            EnableEmailHyperlinks = false,
            RequireControlModifierForHyperlinkClick = true
        };

        editor.TextArea.AddPlatformSpecificKeyBindings();
        return editor;
    }

    private static void AddPlatformSpecificKeyBindings(this TextArea textArea)
    {
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.MacOSX:
                textArea.KeyBindings.Clear();
                textArea.KeyBindings.AddRange(KeyBindings.ForMac);
                break;
        }
    }

    public static TextEditor AddSearch(this TextEditor editor, Action<SearchPanel> configuration = null)
    {
        var panel = SearchPanel.Install(editor);
        configuration?.Invoke(panel);
        return editor;
    }

    public static void PrepareForJson(this TextEditor editor)
    {
        editor.SyntaxHighlighting = SyntaxHighlighting.Json;
    }
        
    public static void PrepareForCSharp(this TextEditor editor, ICompletionProvider completionProvider)
    {
        editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(editor.Options);
        editor.TextArea.TextView.BackgroundRenderers.Add(new CurrentStatementRenderer(editor));
        editor.KeyDown += KeyDown(editor, completionProvider);
        editor.PointerHoverStopped += PointerHoverStopped(editor, completionProvider);
        editor.TextArea.TextView.LineTransformers.Add(new SemanticColorizer(editor, completionProvider));
    }

    private static EventHandler<PointerEventArgs> PointerHoverStopped(TextEditor editor, ICompletionProvider completionProvider)
    {
        return ShowInfo;
        async void ShowInfo(object _, PointerEventArgs e)
        {
            var position = editor.GetPositionFromPoint(e.GetPosition(editor));
            if (position == null)
            {
                return;
            }
            var offset = editor.TextArea.Document.GetOffset(position.Value.Location);
            var span = editor.GetCodeSpanAt(offset);

            var completer = completionProvider.GetCodeCompleter(editor.Text.Substring(span.Start, span.End));
        }
    }

    private static EventHandler<KeyEventArgs> KeyDown(TextEditor editor, ICompletionProvider completionProvider)
    {
        return Down;

        async void Down(object _, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space when (e.KeyModifiers & KeyModifiers.Control) == KeyModifiers.Control:
                {
                    var currentStatement = editor.GetCompletionStatement();
                    var completions = await completionProvider.GetCompletionsAsync(currentStatement, currentStatement.Length);
                    if (!completions.Any())
                    {
                        return;
                    }

                    var completionWindow = new CompletionWindow(editor.TextArea) {Width = editor.TextArea.Width <= 300 ? 300 : editor.TextArea.Width * .5,};
                    completionWindow.CompletionList.CompletionData.AddRange(completions.Select(c => new CompletionData(c.Prefix, c.Completion, c.Description)));
                    completionWindow.Show();
                    completionWindow.Closed += (_, _) => completionWindow = null;
                    e.Handled = true;
                    break;
                }
                case Key.P when (e.KeyModifiers & Modifiers.CtrlOrMeta) == Modifiers.CtrlOrMeta:
                {
                    Console.WriteLine("ctrl+P!");
                    var insightWindow = new InsightWindow(editor.TextArea)
                    {
                        
                        Width = editor.TextArea.Width <= 300 ? 300 : editor.TextArea.Width * .5,
                        Height = 200
                    };
                    

                    
                    insightWindow.Show();
                    insightWindow.Closed += (_, _) => insightWindow = null;
                    e.Handled = true;
                    break;
                }
            }
        }
    }
}