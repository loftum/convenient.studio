using System.Linq;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using ICSharpCode.AvalonEdit.Search;

namespace Convenient.Studio.Controls
{
    public class InteractiveEditor : TextEditor
    {
        public IInteractiveVm Vm { get; set; }

        public InteractiveEditor()
        {
            Options = new TextEditorOptions
            {
                ConvertTabsToSpaces = true,
                IndentationSize = 2,
                EnableRectangularSelection = true,
                HighlightCurrentLine = true,
                EnableHyperlinks = true,
                RequireControlModifierForHyperlinkClick = true
            };
            TextArea.IndentationStrategy = new CSharpIndentationStrategy(Options);
            
            SearchPanel.Install(this);
            KeyDown += Input_KeyDown;
        }

        private async void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Vm != null)
            {
                var completions = await Vm.GetCompletions(GetCurrentStatement());
                var completionWindow = new CompletionWindow(TextArea);
                completionWindow.CompletionList.CompletionData.AddRange(completions.Select(c => new CompletionData(c.Prefix, c.Completion, c.Description)));
                completionWindow.Show();
                completionWindow.Closed += (o, ea) => completionWindow = null;
                e.Handled = true;
            }
        }

        public string GetSelectedOrAllText()
        {
            var selectedText = SelectedText;
            return string.IsNullOrEmpty(selectedText) ? Text : selectedText;
        }

        public string GetCurrentStatement()
        {
            var caret = TextArea.Caret;
            if (caret.Offset <= 0)
            {
                return "";
            }
            var start = caret.Offset <= 0 ? 0 : caret.Offset - 1;
            var lineFeeds = 0;
            while (start > 0)
            {
                var c = Text[start];
                if (c == '\n')
                {
                    if (lineFeeds > 0)
                    {
                        break;
                    }
                    lineFeeds++;
                }
                else if (c != '\r')
                {
                    lineFeeds = 0;
                }
                start--;
            }
            var currentLine = Text.Substring(start, caret.Offset - start).Trim();
            return currentLine;
        }
    }
}