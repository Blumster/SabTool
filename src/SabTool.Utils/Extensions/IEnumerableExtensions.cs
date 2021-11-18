using System;
using System.Collections.Generic;
using System.Linq;

namespace SabTool.Utils.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> TopologicalSort<T>(this IEnumerable<T> nodes, Func<T, IEnumerable<T>> connected, T nullValue)
        {
            var elems = nodes.ToDictionary(node => node, node => new HashSet<T>(connected(node)));

            while (elems.Count > 0)
            {
                var elem = elems.FirstOrDefault(x => x.Value.Count == 0);
                if (elem.Key.Equals(nullValue))
                {
                    throw new ArgumentException("Cyclic connections are not allowed");
                }
                elems.Remove(elem.Key);
                foreach (var selem in elems)
                {
                    selem.Value.Remove(elem.Key);
                }
                yield return elem.Key;
            }
        }
    }
}
