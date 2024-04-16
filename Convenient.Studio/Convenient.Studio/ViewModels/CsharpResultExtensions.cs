using System.Text.Json;
using System.Text.Json.Serialization;
using Convenient.Studio.Scripting.Cancellation;
using Convenient.Studio.Scripting.Commandline;

namespace Convenient.Studio.ViewModels;

public static class CsharpResultExtensions
{
    private static readonly JsonSerializerOptions Pretty = new()
    {
        Converters =
        {
            new JsonStringEnumConverter()
        },
        WriteIndented = true
    };
    
    public static string ToResultString(this object item)
    {
        switch (item)
        {
            case null:
                return "null";
            case CancelExecutionException cex:
                return "cancelled";
            case Exception e:
                return e.ToString();
        }

        return item.GetType().LooksSimple()
            ? item.ToString()
            : JsonSerializer.Serialize(item, Pretty);
    }

    private static bool LooksSimple(this Type type)
    {
        return type.In(typeof(byte),
                   typeof(short),
                   typeof(int),
                   typeof(long),
                   typeof(float),
                   typeof(double),
                   typeof(decimal),
                   typeof(bool),
                   typeof(Guid),
                   typeof(DateTime),
                   typeof(DateTimeOffset),
                   typeof(string)) ||
               type.IsValueType && !type.GetProperties().Any() && !type.GetFields().Any();
    }
}