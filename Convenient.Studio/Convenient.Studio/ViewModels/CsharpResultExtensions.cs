using System.Linq.Expressions;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Convenient.Studio.Nodes;
using Convenient.Studio.Scripting.Cancellation;

namespace Convenient.Studio.ViewModels;

public class JsonTypeConverter : JsonConverter<Type>
{
    public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return null;
    }

    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }
        writer.WriteStringValue(value.GetFriendlyName());
    }
}

public class JsonMethodInfoConverter : JsonConverter<MethodInfo>
{
    public override MethodInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return null;
    }

    public override void Write(Utf8JsonWriter writer, MethodInfo value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToFriendlyString());
    }
}

public class JsonParameterInfoConverter : JsonConverter<ParameterInfo>
{
    public override ParameterInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return null;
    }

    public override void Write(Utf8JsonWriter writer, ParameterInfo value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToFriendlyString());
    }
}

public class JsonMemberInfoConverter : JsonConverter<MemberInfo>
{
    public override MemberInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return null;
    }

    public override void Write(Utf8JsonWriter writer, MemberInfo value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToFriendlyString());
    }
}

public static class CsharpResultExtensions
{
    private static readonly JsonSerializerOptions Pretty = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(),
            new JsonTypeConverter(),
            new JsonMethodInfoConverter(),
            new JsonParameterInfoConverter(),
            new JsonMemberInfoConverter()
        },
        
        WriteIndented = true,
        IncludeFields = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    
    public static string ToResultString(this object item)
    {
        switch (item)
        {
            case null:
                return "null";
            case CancelExecutionException:
                return "cancelled";
            case Exception e:
                return e.ToString();
            case Type t:
                return t.Name;
            case Expression e:
                return JsonSerializer.Serialize(ObjectNode.For(e), Pretty);
        }

        return item.GetType().LooksSimple()
            ? item.ToString()
            : JsonSerializer.Serialize(item, Pretty);
    }
}

