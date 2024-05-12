using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Convenient.Studio.Config;
using Convenient.Studio.ViewModels;
using Convenient.Studio.Views;
using ReactiveUI;

namespace Convenient.Studio;

public static class KeyBinder
{
    public static void AddKeyBindingsAndMenu(MainWindowViewModel vm, MainWindow window)
    {
        var ctrl = Modifiers.CtrlOrMeta;

        var newFile = Bind(vm.Editor.NewFile, Key.N, ctrl);
        var save = Bind(vm.Editor.Save, Key.S, ctrl);
        var closeCurrentFile = Bind(vm.Editor.CloseCurrentFile, Key.W, ctrl);
        var closeAllButThis = Bind(vm.Editor.CloseAllButThis, Key.W, ctrl | KeyModifiers.Shift);
        var execute = Bind(ReactiveCommand.CreateFromTask(window.ExecuteCancelByCommentAsync), Key.F5);
        var executeCancelEveryStatement = Bind(ReactiveCommand.CreateFromTask(window.ExecuteCancelEveryStatementAsync), Key.F5, ctrl);
        var goToPreviousTab = Bind(ReactiveCommand.Create(window.GoToPreviousTab), Key.Left, ctrl | KeyModifiers.Alt);
        var goToNextTab = Bind(ReactiveCommand.Create(window.GoToNextTab), Key.Right, ctrl | KeyModifiers.Alt);
            
        if (Platform.IsMac)
        {
            var menu = new NativeMenu
            {
                new NativeMenuItem("File")
                {
                    Menu = new NativeMenu
                    {
                        Items =
                        {
                            new NativeMenuItem("New").WithBinding(newFile),
                            new NativeMenuItem("Save").WithBinding(save),
                            new NativeMenuItem("Close").WithBinding(closeCurrentFile),
                            new NativeMenuItem("Close all but this").WithBinding(closeAllButThis)
                        }
                    }
                },
                new NativeMenuItem("Navigate")
                {
                    Menu = new NativeMenu
                    {
                        Items =
                        {
                            new NativeMenuItem("Previous tab").WithBinding(goToPreviousTab),
                            new NativeMenuItem("Next tab").WithBinding(goToNextTab)
                        }
                    }
                },
                new NativeMenuItem("Run")
                {
                    Menu = new NativeMenu
                    {
                        Items =
                        {
                            new NativeMenuItem("Execute").WithBinding(execute),
                            new NativeMenuItem("Execute (every statement cancellable)").WithBinding(executeCancelEveryStatement)
                        }
                    }
                }
            };
            NativeMenu.SetMenu(window, menu);
        }
    }

    private static KeyBinding Bind(ICommand command, Key key, KeyModifiers modifiers = KeyModifiers.None)
    {
        return new()
        {
            Command = command,
            Gesture = new KeyGesture(key, modifiers)
        };
            
    }
}

public static class WindowExtensions
{
    public static NativeMenuItem WithBinding(this NativeMenuItem item, KeyBinding binding)
    {
        item.Command = binding.Command;
        item.Gesture = binding.Gesture;
        return item;
    }
}