using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Repo1.Core.ns11.Drupal7Tools.UndFields;
using Repo1.Core.ns11.Extensions.StringExtensions;

namespace Repo1.Core.ns11.Drupal7Tools
{
    public class D7TaxonomyMapper
    {
        public static void Map<T>(Dictionary<string, object> dict, T obj)
        {
            foreach (var prop in typeof(T).GetRuntimeProperties())
            {
                var attr = prop.CustomAttributes.SingleOrDefault(x
                    => x.AttributeType == typeof(TermAttribute));
                if (attr == null) continue;

                var key = attr.NamedArguments.SingleOrDefault(x
                        => x.MemberName == nameof(TermAttribute.Key))
                            .TypedValue.Value.ToString();
                if (key.IsBlank()) continue;

                dict.Add(key, und.TermID(prop, obj));
            }
        }
    }
}
