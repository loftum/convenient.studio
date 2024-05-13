using Avalonia;
using Avalonia.ReactiveUI;

namespace Convenient.Visualizer;

class Program
{
    public static int Main(string[] args)
    {
        try
        {
            return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 1;
        }
    }
    
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}