using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using Convenient.Studio.Config;
using Convenient.Visualizer.ViewModels;
using Convenient.Studio.Scripting.Commandline;

namespace Convenient.Visualizer.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel _vm;
    private readonly TextEditor _inputBox;
    private readonly TextEditor _syntaxTreeBox;
    private readonly FileManager _fileManager = new("visualizer");

    public MainWindow()
    {
        InitializeComponent();
        _inputBox = this.FindControl<TextEditor>("InputBox");
        _inputBox.SetDefaultOptions()
            .AddSearch();
        _syntaxTreeBox = this.FindControl<TextEditor>("SyntaxTreeBox");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        var settings = _fileManager.LoadJsonOrDefault<WindowData>() ?? new WindowData();
        Position = new PixelPoint(settings.X, settings.Y);
        Height = settings.Height;
        Width = settings.Width;
        
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        _vm = DataContext as MainWindowViewModel;
        if (_vm == null)
        {
            return;
        }
        _vm.PropertyChanged += OnVmPropertyChanged;
    }

    private void OnVmPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is not MainWindowViewModel vm)
        {
            return;
        }
        Console.WriteLine($"PropertyChanged: {e.PropertyName}");
        switch (e.PropertyName)
        {
            case nameof(vm.SyntaxTreeText):
                
                _syntaxTreeBox.Text = vm.SyntaxTreeText;
                break;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.S && (e.KeyModifiers & Modifiers.CtrlOrMeta) == Modifiers.CtrlOrMeta)
        {
            Save();
            e.Handled = true;
            return;
        }
        base.OnKeyDown(e);
    }

    private void Save()
    {
        var settings = new WindowData
        {
            X = Position.X,
            Y = Position.Y,
            Height = Height,
            Width = Width
        };
            
        _fileManager.SaveJson(settings);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        Save();
        base.OnClosing(e);
    }

    private void InputTextChanged(object sender, EventArgs e)
    {
        var editor = (TextEditor) sender;
        _vm.InputText = editor.Text;
    }
}