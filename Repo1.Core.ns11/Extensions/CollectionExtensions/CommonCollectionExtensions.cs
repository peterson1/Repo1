using System.Collections.Generic;

namespace Repo1.Core.ns11.Extensions.CollectionExtensions
{
    public static class CommonCollectionExtensions
    {
        public static void Swap<T>(this IList<T> colxn, IEnumerable<T> newItems)
        {
            if (colxn == null) return;
            colxn.Clear();
            foreach (var item in newItems)
                colxn.Add(item);
        }
    }
}
