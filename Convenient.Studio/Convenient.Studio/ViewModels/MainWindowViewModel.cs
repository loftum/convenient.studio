using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Convenient.Studio.Scripting;
using Convenient.Studio.Scripting.Cancellation;
using ReactiveUI;

namespace Convenient.Studio.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly Interactive _interactive;
    private ICompletionProvider _completionProvider;
    private IBrush _environmentBorderColor;
    private bool _isBusy;
    private string _executionTimer = "...";

    public int FontSize
    {
        get => _fontSize;
        set => this.RaiseAndSetIfChanged(ref _fontSize, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            Editor.InputEnabled = !value;
            this.RaiseAndSetIfChanged(ref _isBusy, value);
        }
    }

    public string ExecutionTimer
    {
        get => _executionTimer;
        set => this.RaiseAndSetIfChanged(ref _executionTimer, value);
    }
    
    public IBrush EnvironmentBorderColor
    {
        get => _environmentBorderColor;
        private set => this.RaiseAndSetIfChanged(ref _environmentBorderColor, value);
    }

    public IEnvironmentVm EnvironmentVm { get; }

    public ICommand Cancel { get; }

    public ICompletionProvider CompletionProvider
    {
        get => _completionProvider;
        private set
        {
            _completionProvider = value;
            this.RaisePropertyChanged();
        }
    }
    
    public EditorViewModel Editor { get; }

    public MainWindowViewModel()
    {
        Editor = new EditorViewModel();
        var context = new StudioContext();
        _interactive = new Interactive(context);
        context.PropertyChanged += ChangeEnvironmentColor;
        CompletionProvider = _interactive.Evaluator;

        EnvironmentVm = context;
        
        Cancel = ReactiveCommand.Create(DoCancel);
        IsBusy = false;
        UpdateEnvironmentColor();
        FontSize = 12;
    }
    
    private void ChangeEnvironmentColor(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EnvironmentVm.Environment))
        {
            UpdateEnvironmentColor();
        }
    }

    private void UpdateEnvironmentColor()
    {
        var color = GetEnvironmentColor(EnvironmentVm.Environment);
        EnvironmentBorderColor = new SolidColorBrush(color);
    }

    private static Color GetEnvironmentColor(string environment)
    {
        switch (environment)
        {
            case null: return Colors.Transparent;
            case var prod when prod.StartsWith("prod", StringComparison.OrdinalIgnoreCase): return Colors.Red;
            default: return Colors.Transparent;
        }
    }

    private void DoCancel()
    {
        Console.WriteLine("Attempting to cancel...");
        _interactive.CancellationTokenSource.Cancel();
        IsBusy = false;
    }

    public async Task DoExecute(string statement, CancellationInjection cancellationInjection)
    {
        if (string.IsNullOrWhiteSpace(statement))
        {
            return;
        }

        var started = Utc.Now;

        try
        {
            ExecutionTimer = "pølse";

            Editor.Output = null;
            IsBusy = true;
            StartTimer();
            var result =
                await Task.Run(() =>
                    _interactive.EvaluateAsync(statement,
                        cancellationInjection)); // Silly way to make it run on threadpool thread.
            Editor.Output = result.ToResultString();
            if (result is CancelExecutionException)
            {
                Console.WriteLine("Cancelled.");
            }
        }
        catch (Exception ex)
        {
            Editor.Output = ex.ToString();
        }
        finally
        {
            IsBusy = false;
            StopTimer();
            ExecutionTimer = $"{(Utc.Now - started):g} ({(_interactive.CancellationTokenSource.IsCancellationRequested?"cancelled":"done")})";
        }
    }

    private DateTimeOffset _executionStartTime;
    private DispatcherTimer _timer;
    private int _fontSize;

    private void StartTimer()
    {
        ExecutionTimer = "...";
        _executionStartTime = Utc.Now;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMicroseconds(100) };
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void StopTimer()
    {
        _timer.Stop();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (IsBusy)
        {
            var soFar = Utc.Now - _executionStartTime;
            ExecutionTimer = $"{soFar:g}";
        }
    }
}