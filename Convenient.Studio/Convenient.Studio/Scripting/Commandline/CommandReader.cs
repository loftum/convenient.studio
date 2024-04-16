using System.Text;

namespace Convenient.Studio.Scripting.Commandline
{
    public class ConsoleSettings
    {
        public ConsoleColor PromptColor { get; set; } = ConsoleColor.Green;
        public Func<string> GetPrompt { get; set; } = () => "> ";
    }

    public class CommandReader
    {
        private readonly ConsoleSettings _settings;
        private string Prompt => _settings.GetPrompt();

        private void PrintPrompt(bool newline = true)
        {
            if (newline)
            {
                Console.WriteLine();
            }
            else
            {
                Console.CursorLeft = 0;
            }
            Console.ForegroundColor = _settings.PromptColor;
            Console.Write(Prompt);
            Console.ResetColor();
        }

        private readonly CSharpEvaluator _evaluator;
        private readonly InputHistory _history;

        public CommandReader(CSharpEvaluator evaluator, InputHistory history) : this(evaluator, history, new ConsoleSettings())
        {
        }
        
        public CommandReader(CSharpEvaluator evaluator, InputHistory history, ConsoleSettings settings)
        {
            _evaluator = evaluator;
            _history = history;
            _settings = settings;
        }

        protected int Cursor
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        protected int Index => Cursor - Prompt.Length;

        public async Task<string> ReadCommand()
        {
            ConsoleKey? lastKey = null;
            var autocomplete = false;
            PrintPrompt();
            while (true)
            {
                var read = Console.ReadKey(true);
                if (read.Key == ConsoleKey.Delete && (read.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift)
                {
                    ClearLine();
                    _history.DeleteCurrent();
                    PrintPrompt(false);
                    Console.Write(_history.Current);
                }

                if (read.Key == ConsoleKey.S && (read.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                {
                    return "ctrl+S";
                }
                
                switch (read.Key)
                {
                    case ConsoleKey.Enter:
                        var input = _history.Current.ToString();
                        Console.WriteLine();
                        return input;
                    case ConsoleKey.UpArrow:
                        if (_history.HasPrevious())
                        {
                            ClearLine();
                            _history.MovePrevious();
                            PrintPrompt(false);
                            Console.Write(_history.Current);
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (_history.HasNext())
                        {
                            ClearLine();
                            _history.MoveNext();
                            PrintPrompt(false);
                            Console.Write(_history.Current);
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (Console.CursorLeft > Prompt.Length)
                        {
                            Console.CursorLeft--;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (Console.CursorLeft < Prompt.Length + _history.Current.Length)
                        {
                            Console.CursorLeft++;
                        }
                        break;
                    case ConsoleKey.Home:
                        Console.CursorLeft = Prompt.Length;
                        break;
                    case ConsoleKey.End:
                        Console.CursorLeft = Prompt.Length + _history.Current.Length;
                        break;
                    case ConsoleKey.Tab:
                        var completions = await _evaluator.GetCompletions(_history.Current.ToString(), Index);
                        if (lastKey == ConsoleKey.Tab && autocomplete)
                        {
                            lastKey = null;
                            completions = completions.Take(1).ToList();
                        }
                        autocomplete = lastKey != ConsoleKey.Tab;
                        switch (completions.Count)
                        {
                            case 0:
                                break;
                            case 1:
                                var single = completions.Single();
                                Cursor -= single.Prefix.Length;
                                Replace(single.Prefix);
                                Insert(single.Completion);
                                break;
                            default:
                                var cursor = Cursor;
                                Console.WriteLine();
                                Console.WriteLine(string.Join(", ", completions));
                                PrintPrompt();
                                Console.Write(_history.Current.ToString());
                                Cursor = cursor;
                                break;
                        }
                        break;
                    case ConsoleKey.Backspace:
                        if (Console.CursorLeft > Prompt.Length)
                        {
                            Cursor--;
                            DeleteCurrentChar();
                        }
                        break;
                    case ConsoleKey.Delete:
                        if (Cursor < _history.Current.Length + Prompt.Length)
                        {
                            DeleteCurrentChar();
                        }
                        break;
                    default:
                        if (ShouldPrint(read))
                        {
                            Insert(read.KeyChar.ToString());
                        }
                        break;
                }
                lastKey = read.Key;
            }
        }

        private void ClearLine()
        {
            Cursor = Prompt.Length;
            Console.Write(Whitespace(_history.Current.Length));
            Cursor = Prompt.Length;
        }

        private void Insert(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            _history.Current.Insert(Index, text);
            var cursor = Cursor;
            Console.Write(_history.Current.From(Index));
            Cursor = cursor + text.Length;
        }

        private void Replace(string replacement)
        {
            if (string.IsNullOrEmpty(replacement))
            {
                return;
            }
            _history.Current.Remove(Index, replacement.Length);
            _history.Current.Insert(Index, replacement);
            Console.Write(replacement);
        }

        private void DeleteCurrentChar()
        {
            var cursor = Cursor;
            var whitespaces = Whitespace(_history.Current.Length - Index);
            var theRest = _history.Current.From(Index + 1);
            _history.Current.Remove(Index, 1);
            Console.Write(whitespaces);
            Cursor = cursor;
            Console.Write(theRest);
            Cursor = cursor;
        }

        private static char[] Whitespace(int number)
        {
            return Enumerable.Range(0, number).Select(i => ' ').ToArray();
        }

        private static bool ShouldPrint(ConsoleKeyInfo read)
        {
            return !char.IsControl(read.KeyChar) && (char.IsLetterOrDigit(read.KeyChar) ||
                                                     char.IsPunctuation(read.KeyChar) ||
                                                     char.IsSeparator(read.KeyChar) ||
                                                     char.IsSymbol(read.KeyChar) ||
                                                     char.IsWhiteSpace(read.KeyChar));
        }
    }

    public static class StringbuilderExtensions
    {
        public static char[] From(this StringBuilder builder, int index)
        {
            return EnumerateFrom(builder, index).ToArray();
        }

        private static IEnumerable<char> EnumerateFrom(this StringBuilder builder, int index)
        {
            for (var ii = index; ii < builder.Length; ii++)
            {
                yield return builder[ii];
            }
        }
    }
}