using Convenient.Studio.Scripting.Cancellation;
using Convenient.Studio.Scripting.Commandline;

namespace Convenient.Studio.Scripting;

public class ConsoleApp
{
    public event EventHandler OnSave;
    private readonly FileManager _fileManager = new FileManager("console");
    private readonly CommandReader _commandReader;
    private readonly CSharpEvaluator _evaluator;
    private readonly InputHistory _history;
    private readonly Func<object, string> _output;

    public ConsoleApp(CSharpEvaluator evaluator) : this(evaluator, new ConsoleSettings(), ScriptStateExtensions.Pretty)
    {
    }
        
    public ConsoleApp(CSharpEvaluator evaluator, ConsoleSettings settings) : this(evaluator, settings, ScriptStateExtensions.Pretty)
    {
    }
        
    public ConsoleApp(CSharpEvaluator evaluator, ConsoleSettings settings, Func<object, string> output)
    {
        var history = LoadHistory();
        _evaluator = evaluator;
        _commandReader = new CommandReader(evaluator, history, settings);
        _history = history;
        _output = output ?? ScriptStateExtensions.Pretty;
    }

    public async Task Run()
    {
        using var source = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            Save();
            Console.WriteLine("Bye!");
            Environment.Exit(0);
        };
        try
        {
            await Run(source.Token);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e);
            Console.ResetColor();
        }
    }

    private async Task Run(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _history.Reset();
                    var input = await _commandReader.ReadCommand();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        continue;
                    }
                    if (input.In("ctrl+s"))
                    {
                        Save();
                        continue;
                    }
                    if (input.In("quit", "exit", "ctrl+q", "alt+f4"))
                    {
                        Save();
                        Console.WriteLine("Bye!");
                        break;
                    }
                    _history.Add(input);
                    var result = await _evaluator.EvaluateAsync(input, cancellationToken, CancellationInjection.ByComment);
                    Console.WriteLine(_output(result));
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ResetColor();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void Save()
    {
        Console.WriteLine("Saving history");
        _fileManager.SaveJson(_history.Commands.ToList(), "history.json");
        OnSave?.Invoke(this, EventArgs.Empty);
    }

    private InputHistory LoadHistory()
    {
        var commands = _fileManager.LoadJson<List<string>>("history.json") ?? new List<string>();
        return new InputHistory(commands, 50);
    }
}