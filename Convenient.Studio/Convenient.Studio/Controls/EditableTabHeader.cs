using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace Convenient.Studio.Controls;

public class EditableTabHeader : StackPanel
{
    public static readonly DirectProperty<EditableTabHeader, ICommand> CloseCommandProperty =
        AvaloniaProperty.RegisterDirect<EditableTabHeader, ICommand>(nameof(CloseCommand),
            e => e.CloseCommand, (e, command) => e.CloseCommand = command, enableDataValidation: true);

    public static readonly DirectProperty<EditableTabHeader, string> TextProperty =
        AvaloniaProperty.RegisterDirect<EditableTabHeader, string>(nameof(TextProperty), h => h.Text, (h,v) => h.Text = v);

    private string _text;
    public string Text
    {
        get => _text;
        set
        {
            _editor.Text = value;
            _label.Text = value;
            SetAndRaise(TextProperty, ref _text, value);
        }
    }

    private readonly TextBox _editor;
    private readonly TextBlock _label;
    private readonly Button _closeButton;

    public ICommand CloseCommand
    {
        get => _closeButton.Command;
        set => _closeButton.Command = value;
    }

    private bool _isEditing;
    public bool IsEditing
    {
        get => _isEditing;
        private set
        {
            _isEditing = value;
            _editor.IsVisible = value;
            _label.IsVisible = !value;
        }
    }

    public EditableTabHeader()
    {
        Background = Brushes.Transparent;
        Orientation = Orientation.Horizontal;
        _editor = new TextBox
        {
            VerticalAlignment = VerticalAlignment.Center,
            Background = Brushes.Transparent
        };
        _editor.LostFocus += Editor_LostFocus;
        _editor.KeyDown += Editor_KeyDown;
        _label = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center,
            Background = Brushes.Transparent
        };
        _label.DoubleTapped += Label_MouseDown;
        _closeButton = new Button
        {
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0, 0, 0),
            Content = "X",
            Cursor = Cursors.Hand
        };
        Children.Add(_editor);
        Children.Add(_label);
        Children.Add(_closeButton);
        IsEditing = false;
    }

    private void Editor_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                IsEditing = false;
                var newText = _editor.Text;
                Text = newText;
                break;
            case Key.Escape:
                CancelEditing();
                break;
            default:
                base.OnKeyDown(e);
                break;
        }
    }

    private void StartEditing()
    {
        IsEditing = true;
    }

    private void CancelEditing()
    {
        IsEditing = false;
        Text = _editor.Text = Text;
    }

    private void Editor_LostFocus(object sender, RoutedEventArgs e)
    {
        CancelEditing();
    }

    private void Label_MouseDown(object sender, RoutedEventArgs e)
    {
        StartEditing();
    }
}