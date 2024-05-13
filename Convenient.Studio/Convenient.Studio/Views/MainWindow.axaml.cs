using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaEdit;
using Convenient.Studio.Config;
using Convenient.Studio.ConsoleRedirect;
using Convenient.Studio.Controls;
using Convenient.Studio.Scripting.Cancellation;
using Convenient.Studio.Scripting.Commandline;
using Convenient.Studio.Settings;
using Convenient.Studio.Syntax;
using Convenient.Studio.ViewModels;

namespace Convenient.Studio.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel _vm;
    private readonly TextEditor _inputBox;
    private readonly TextEditor _outputBox;
    private readonly TextEditor _consoleBox;
    private readonly TabStrip _fileTabs;
    private readonly FileManager _fileManager = new("studio");
        
    static MainWindow()
    {
    }
        
    public MainWindow()
    {
        SyntaxHighlighting.Load();
        InitializeComponent();
        _fileTabs = this.FindControl<TabStrip>("FileTabs");
        _inputBox = this.FindControl<TextEditor>("InputBox");
        _outputBox = this.FindControl<TextEditor>("OutputBox");
        _consoleBox = this.FindControl<TextEditor>("ConsoleBox");
        _consoleBox.TextArea.TextView.LinkTextForegroundBrush = Brushes.HotPink;
        ConsoleOut.Writer.Add(new DocumentTextWriter(_consoleBox.Document), Dispatcher.UIThread);
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
        var old = _vm;
        if (old != null)
        {
            old.PropertyChanged -= VmPropertyChanged;
            old.PropertyChanging -= VmPropertyChanging;
        }
            
        _vm = (MainWindowViewModel) DataContext;
        _vm.Editor.PropertyChanged += VmPropertyChanged;
        _vm.Editor.PropertyChanging += VmPropertyChanging;
        _inputBox.SetDefaultOptions()
            .AddSearch()
            .PrepareForCSharp(_vm.CompletionProvider);

        _outputBox.SetDefaultOptions().AddSearch();
        _consoleBox.SetDefaultOptions().AddSearch();

        _inputBox.Text = _vm.Editor.Input;
        _outputBox.Text = _vm.Editor.Output;
    }

    private void VmPropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        if (!(sender is EditorViewModel editor))
        {
            return;
        }
        switch (e.PropertyName)
        {
            case nameof(editor.CurrentFile):
                editor.CurrentFile.Input = _inputBox.Text;
                break;
        }
    }

    private void VmPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is not EditorViewModel editor)
        {
            return;
        }
            
        switch (e.PropertyName)
        {
            case nameof(editor.Input):
                _inputBox.Text = editor.Input;
                break;
            case nameof(editor.Output):
                _outputBox.Text = editor.Output;
                break;
        }
    }

    public Task ExecuteCancelByCommentAsync() => ExecuteAsync(CancellationInjection.ByComment);
    public Task ExecuteCancelEveryStatementAsync() => ExecuteAsync(CancellationInjection.EveryStatement);
        
    private async Task ExecuteAsync(CancellationInjection cancellationInjection)
    {
        var text = string.IsNullOrEmpty(_inputBox.SelectedText) ? _inputBox.Text : _inputBox.SelectedText;
        await _vm.DoExecute(text, cancellationInjection);
        _inputBox.Focus();
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
        _vm.Editor.Input = _inputBox.Text;
        _vm.Editor.SaveFiles();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        Save();
        base.OnClosing(e);
    }
        
    private void Console_TextChanged(object sender, EventArgs e)
    {
        var textBox = sender as TextEditor;
        textBox?.ScrollToEnd();
    }

    public void GoToPreviousTab()
    {
        _fileTabs.SelectPreviousTab();
    }

    public void GoToNextTab()
    {
        _fileTabs.SelectNextTab();
    }

    private void FileChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_vm == null)
        {
            return;
        }
        _vm.Editor.Input = _inputBox.Text;
    }
}