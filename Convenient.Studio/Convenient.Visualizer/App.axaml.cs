using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Convenient.Visualizer.ViewModels;
using Convenient.Visualizer.Views;

namespace Convenient.Visualizer;

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
            
            var window = new MainWindow
            {
                DataContext = vm
            };
            
            desktop.MainWindow = window;
        }
        else
        {
            Console.WriteLine("OMG NOT Classic!");
        }

        base.OnFrameworkInitializationCompleted();
    }
}