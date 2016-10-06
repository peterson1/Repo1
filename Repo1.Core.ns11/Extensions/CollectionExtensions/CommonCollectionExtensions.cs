using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Repo1.Core.ns11.Extensions.CollectionExtensions
{
    public static class CommonCollectionExtensions
    {
        public static void Swap<T>(this Collection<T> colxn, IEnumerable<T> newItems)
        {
            colxn.Clear();
            foreach (var item in newItems)
                colxn.Add(item);
        }
    }
}
