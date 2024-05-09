namespace Convenient.Studio.ConsoleRedirect;

public static class ConsoleOut
{
    public static readonly DelegateTextWriter Writer = new DelegateTextWriter();

    static ConsoleOut()
    {
        Console.SetOut(Writer);
    }
}