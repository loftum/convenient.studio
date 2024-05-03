using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.Scripting;

namespace Convenient.Studio.Scripting;

public static class ScriptStateExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter()
        },
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
        
    public static object GetResult(this ScriptState state)
    {
        return state.ReturnValue ?? state.Exception;
    }

    public static string Pretty(this object o)
    {
        switch (o)
        {
            case null: return null;
            case string s: return s;
            case Task t: return "";
            case WebException we: return Format(we);
            case Exception e: return e.ToString();
            case Expression e: return e.ToString();
            default: return JsonSerializer.Serialize(o, JsonOptions);
        }
    }

    private static string Format(WebException we)
    {
        var builder = new StringBuilder().Append(we).AppendLine();

        if (we.Response is HttpWebResponse {ContentLength: > 0} response)
        {
            using var reader = new StreamReader(response.GetResponseStream());
            var txt = reader.ReadToEnd();
            builder.AppendLine("Response:").AppendLine(txt);
        }
        return builder.ToString();
    }
}