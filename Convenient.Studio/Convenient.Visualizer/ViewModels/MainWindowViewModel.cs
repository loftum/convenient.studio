using Avalonia.Threading;
using Convenient.Visualizer.Models;
using Convenient.Visualizer.Models.Syntax;
using ReactiveUI;

namespace Convenient.Visualizer.ViewModels;

public class MainWindowViewModel : ReactiveObject, IDisposable
{
    private readonly Task _parser;
    private string _inputText;
    private string _metaText;
    private int _fontSize = 12;
    private string _compilationText;
    private string _syntaxTreeText;

    public int FontSize
    {
        get => _fontSize;
        set => this.RaiseAndSetIfChanged(ref _fontSize, value);
    }

    public string InputText
    {
        get => _inputText;
        set
        {
            _inputText = value;
            ShouldParse = true;
            ParseAt = DateTimeOffset.UtcNow.AddMilliseconds(100);
        }
    }
    
    public DateTimeOffset ParseAt { get; set; }

    public string MetaText
    {
        get => _metaText;
        set => this.RaiseAndSetIfChanged(ref _metaText, value);
    }

    public string CompilationText
    {
        get => _compilationText;
        set => this.RaiseAndSetIfChanged(ref _compilationText, value);
    }

    public string SyntaxTreeText
    {
        get => _syntaxTreeText;
        set
        {
            Console.WriteLine("Setting syntaxtreetext");
            this.RaiseAndSetIfChanged(ref _syntaxTreeText, value);
        }
    }

    public bool ShouldParse { get; set; }
    
    public VisualizerModel Model { get; set; }

    private readonly CancellationTokenSource _cts = new();
    
    public MainWindowViewModel()
    {
        _parser = ParseInLoop(_cts.Token);
    }

    private async Task ParseInLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!ShouldParse)
                {
                    await Task.Delay(100, cancellationToken);
                    continue;
                }
                ShouldParse = false;
                while (DateTimeOffset.UtcNow < ParseAt)
                {
                    await Task.Delay(100, cancellationToken);
                }
                DoParse();
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Cancelled");
                return;
            }
        }
    }

    private void DoParse()
    {
        Console.WriteLine("PAAARSE!");
        var input = InputText ?? "";
        Model = VisualizerModel.Parse(input);
        var tree = SyntaxMapper.Map(Model.SyntaxTree);
        var compilation = CompilationMapper.Map(Model.Compilation);

        try
        {
            SyntaxTreeText = tree.ToPrettyJson();
            CompilationText = compilation.ToPrettyJson();
        
            Dispatcher.UIThread .Invoke(() =>
            {
                SyntaxTreeText = tree.ToPrettyJson();
                CompilationText = compilation.ToPrettyJson();
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _parser?.Dispose();
    }
}