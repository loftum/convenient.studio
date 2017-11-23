using System;
using Newtonsoft.Json;

namespace Convenient.Studio.ViewModels
{
    public static class CsharpResultExtensions
    {
        public static string ToResultString(this object item)
        {
            if (item == null)
            {
                return "null";
            }
            if (item is string s)
            {
                return s;
            }
            if (item is Exception e)
            {
                return e.ToString();
            }
            if (item.GetType().IsValueType)
            {
                return item.ToString();
            }
            return JsonConvert.SerializeObject(item, Formatting.Indented);
        }
    }
}