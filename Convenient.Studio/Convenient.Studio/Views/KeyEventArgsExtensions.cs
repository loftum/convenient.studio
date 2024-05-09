using Avalonia.Input;

namespace Convenient.Studio.Views;

internal static class KeyEventArgsExtensions
{
    public static bool Has(this KeyEventArgs e, Key key, params KeyModifiers[] modifiers)
    {
        var m = modifiers.Aggregate((p, n) => p | n);
        return e.Key == key && (e.KeyModifiers & m) == m;
    }
}