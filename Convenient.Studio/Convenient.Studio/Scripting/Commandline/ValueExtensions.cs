namespace Convenient.Studio.Scripting.Commandline;

internal static class ValueExtensions
{
    public static bool In<T>(this T value, params T[] values)
    {
        return values.Contains(value);
    }
    public static bool In<T>(this T value, IEnumerable<T> values)
    {
        return values.Contains(value);
    }
}