using System.Collections.Generic;

namespace Convenient.Studio.Controls
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        public static void AddRange<T>(this IList<T> list, params T[] items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }
    }
}