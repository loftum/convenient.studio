using System.Windows.Input;
using Avalonia.Input;
using AvaloniaEdit;

namespace Convenient.Visualizer.Views;

public static class KeyBindings
{
    public static readonly KeyBinding[] ForMac =
    [
        Bind(EditingCommands.MoveToLineStart, "cmd+left"),
        Bind(EditingCommands.MoveToLineEnd, "cmd+right"),
        Bind(EditingCommands.MoveToDocumentStart, "cmd+up"),
        Bind(EditingCommands.MoveToDocumentEnd, "cmd+down"),
        Bind(EditingCommands.MoveLeftByWord, "alt+left"),
        Bind(EditingCommands.MoveRightByWord, "alt+right"),
        Bind(EditingCommands.SelectLeftByWord, "shift+alt+left"),
        Bind(EditingCommands.SelectRightByWord, "shift+alt+right"),
        Bind(EditingCommands.SelectToLineStart, "shift+cmd+left"),
        Bind(EditingCommands.SelectToLineEnd, "shift+cmd+right")
    ];
            
    private static KeyBinding Bind(ICommand command, string gesture) => new()
    {
        Command = command,
        Gesture = KeyGesture.Parse(gesture)
    };
}