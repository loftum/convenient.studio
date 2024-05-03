using System.Reflection;
using System.Text;
using Convenient.Studio.Scripting.Commandline;

namespace Convenient.Studio.ViewModels;

public static class TypeExtensions
{
    private static readonly Dictionary<Type, string> ValueTypeAliases = new()
    {
        [typeof(byte)] = "byte",
        [typeof(short)] = "short",
        [typeof(ushort)] = "ushort",
        [typeof(int)] = "int",
        [typeof(uint)] = "uint",
        [typeof(long)] = "long",
        [typeof(ulong)] = "ulong",
        [typeof(float)] = "float",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
        [typeof(bool)] = "bool",
        [typeof(string)] = "string",
        [typeof(void)] = "void"
    };
    
    public static string GetFriendlyName(this Type type)
    {
        if (type.IsGenericType)
        {
            var name = type.Name.Split('`')[0];
            return $"{name}<{string.Join(", ", type.GetGenericArguments().Select(a => a.GetFriendlyName()))}>";
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return $"{type.GenericTypeArguments[0].GetFriendlyName()}?";
        }

        if (ValueTypeAliases.TryGetValue(type, out var alias))
        {
            return alias;
        }

        return type.Name;
    }

    public static string ToFriendlyString(this PropertyInfo property)
    {
        var builder = new StringBuilder($"{property.PropertyType.GetFriendlyName()} ");
        if (property.DeclaringType != null)
        {
            builder.Append($"{property.DeclaringType.GetFriendlyName()}.");
        }

        builder.Append(property.Name);
        return builder.ToString();
    }
    
    public static string ToFriendlyString(this MethodInfo method)
    {
        var builder = new StringBuilder($"{method.ReturnType.GetFriendlyName()} ");
        if (method.DeclaringType != null)
        {
            builder.Append($"{method.DeclaringType.GetFriendlyName()}.");
        }

        builder.Append($"{method.Name}(")
            .Append(string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.GetFriendlyName()} {p.Name}")))
            .Append(")");
        return builder.ToString();
    }
    
    public static bool LooksSimple(this Type type)
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