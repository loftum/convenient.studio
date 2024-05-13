using Convenient.Studio.Scripting.Cancellation;

namespace Convenient.Studio.Scripting.Commandline;

public class CSharpRepl
{
    public event EventHandler OnSave;
    private readonly FileManager _fileManager = new FileManager("repl");
    private readonly CSharpEvaluator _evaluator;
    private readonly InputHistory _history;

    public CSharpRepl(CSharpEvaluator evaluator)
    {
        _evaluator = evaluator;
        _history = LoadHistory();
    }

    public async Task Run()
    {
        using var source = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            source.Cancel();
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
                    var reader = new CommandReader(_evaluator, _history);
                    var input = await reader.ReadCommand();

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

                    if (input.StartsWith("run"))
                    {
                        if (input.Length <= 4)
                        {
                            Console.WriteLine($"Script file must be provided. E.g 'run /path/to/script.cs'");
                            continue;
                        }
                        var scriptFile = input.Substring("run ".Length).Trim();
                        if (!File.Exists(scriptFile))
                        {
                            Console.WriteLine($"Unknown script file {scriptFile}");
                            continue;
                        }

                        var script = await File.ReadAllTextAsync(scriptFile, cancellationToken);
                        _history.Add(input);
                        var r = await _evaluator.EvaluateAsync(script, cancellationToken, CancellationInjection.ByComment);
                        Console.WriteLine(r.Pretty());
                        continue;
                    }
                        
                    _history.Add(input);
                    var result = await _evaluator.EvaluateAsync(input, cancellationToken, CancellationInjection.ByComment);
                    Console.WriteLine(result.Pretty());
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