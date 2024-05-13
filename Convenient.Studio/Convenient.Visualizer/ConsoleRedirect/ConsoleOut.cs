namespace Convenient.Visualizer.ConsoleRedirect;

public static class ConsoleOut
{
    public static readonly DelegateTextWriter Writer = new();

    static ConsoleOut()
    {
        Console.SetOut(Writer);
    }
}