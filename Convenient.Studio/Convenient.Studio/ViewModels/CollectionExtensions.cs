namespace Convenient.Studio.ViewModels;

public static class CollectionExtensions
{
    public static int GetIndexAfterRemovalOf<T>(this IList<T> collection, T item)
    {
        var index = collection.IndexOf(item);
        if (index == 0)
        {
            return 0;
        }

        if (index == collection.Count - 1)
        {
            return index - 1;
        }

        return index;
    }

    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}