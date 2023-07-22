namespace SabTool.Utils.Extensions;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> TopologicalSort<T>(this IEnumerable<T> nodes, Func<T, IEnumerable<T>> connected, T nullValue)
    {
        Dictionary<T, HashSet<T>> elems = nodes.ToDictionary(node => node, node => new HashSet<T>(connected(node)));

        while (elems.Count > 0)
        {
            KeyValuePair<T, HashSet<T>> elem = elems.FirstOrDefault(x => x.Value.Count == 0);
            if (elem.Key.Equals(nullValue))
            {
                throw new ArgumentException("Cyclic connections are not allowed");
            }
            _ = elems.Remove(elem.Key);
            foreach (KeyValuePair<T, HashSet<T>> selem in elems)
            {
                _ = selem.Value.Remove(elem.Key);
            }
            yield return elem.Key;
        }
    }
}
