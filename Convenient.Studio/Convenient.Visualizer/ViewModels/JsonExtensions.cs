using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Convenient.Studio.ViewModels;

namespace Convenient.Visualizer.ViewModels;

public static class JsonExtensions
{
    public static string ToPrettyJson(this object item)
    {
        return JsonSerializer.Serialize(item, Pretty);
    }

    private static JsonSerializerOptions Pretty { get; } = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(),
            new EncodingJsonConverter(),
            new TypeJsonConverter()
        }
    };
}

public class TypeJsonConverter : JsonConverter<Type>
{
    public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return null;
    }

    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.GetFriendlyName());
    }
}

public class EncodingJsonConverter : JsonConverter<Encoding>
{
    public override Encoding Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var name = reader.GetString();
        return name == null
            ? null
            : Encoding.GetEncoding(name);
    }

    public override void Write(Utf8JsonWriter writer, Encoding value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.EncodingName);
    }
}