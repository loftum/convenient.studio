using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Convenient.Studio.ViewModels;
using Convenient.Studio.Views;

namespace Convenient.Studio;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = new MainWindowViewModel();
            vm.Editor.Load();
            var window = new MainWindow
            {
                DataContext = vm
            };
            KeyBinder.AddKeyBindingsAndMenu(vm, window);
            desktop.MainWindow = window;
        }
        else
        {
            Console.WriteLine("OMG NOT Classic!");
        }

        base.OnFrameworkInitializationCompleted();
    }
}