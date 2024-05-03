using System.Collections;
using System.Reflection;
using Convenient.Studio.ViewModels;

namespace Convenient.Studio.Nodes;

public class ObjectNode
{
    private readonly HashSet<object> _visited = new();

    private readonly object _ignore = new();
    
    public object Visit(object o)
    {
        switch (o)
        {
            case null:
                return null;
            case var _ when o.GetType().LooksSimple():
            {
                return o;
            }
            case PropertyInfo property:
                return property.ToFriendlyString();
            case MethodInfo method:
                return method.ToFriendlyString();
            case Module:
            case Assembly:
                return _ignore;
            
        }
        
        if (!_visited.Add(o))
        {
            return _ignore;
        }

        switch (o)
        {
            case ICollection collection:
            {
                var objects = new List<object>();
                foreach (var item in collection)
                {
                    var visited = Visit(item);
                    if (!ReferenceEquals(visited, _ignore))
                    {
                        objects.Add(visited);    
                    }
                }

                return objects;
            }
            default:
            {
                var dictionary = new Dictionary<string, object>();
                foreach (var property in o.GetType().GetProperties().Where(p => p.GetGetMethod() != null))
                {
                    try
                    {
                        var value = property.GetValue(o);
                        var visited = Visit(value);
                        if (!ReferenceEquals(visited, _ignore))
                        {
                            dictionary[property.Name] = visited;    
                        }
                        
                    }
                    catch (Exception e)
                    {
                        return e.GetBaseException().GetType().GetFriendlyName();
                        throw new ObjectNodeException($"Could not get {property.PropertyType.GetFriendlyName()} {o.GetType().GetFriendlyName()}.{property.Name}", e);
                    }
                    
                }

                return dictionary;
            }
        }        
    }
    
    public static object For(object o)
    {
        return new ObjectNode().Visit(o);
    }
}

public class ObjectNodeException : Exception
{
    public ObjectNodeException(string message, Exception inner) : base(message, inner)
    {
        
    }
}
