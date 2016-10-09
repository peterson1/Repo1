using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repo1.Core.ns11.Drupal7Tools
{
    public class D7TaxonomyMapper
    {
        public static void Map<T>(Dictionary<string, object> dict, T obj)
        {
            //foreach (var prop in typeof(T).GetRuntimeProperties())
            //{
            //    var attr = prop.CustomAttributes.SingleOrDefault(x
            //        => x.AttributeType == typeof(TermAttribute));
            //    if (attr == null) continue;

            //    var key = attr.NamedArguments.SingleOrDefault(x
            //            => x.MemberName == nameof(TermAttribute.Key))
            //                .TypedValue.Value.ToString();

            //    dict.Add(key, und.TargetID(prop, origObj));
            //}
        }
    }
}
