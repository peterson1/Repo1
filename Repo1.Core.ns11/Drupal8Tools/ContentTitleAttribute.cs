using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Repo1.Core.ns11.Drupal8Tools
{
    public class ContentTitleAttribute : Attribute
    {
        private static object GetPropertyValue<T>(T sourceObj)
        {
            var prop = typeof(T).GetRuntimeProperties()
                      .SingleOrDefault(x => x.CustomAttributes
                      .Any(y => y.AttributeType == typeof(ContentTitleAttribute)));

            return prop?.GetValue(sourceObj);
        }

        internal static List<Dictionary<string, object>> ToD8Field<T>(T sourceObj) where T : D8NodeBase
            => D8HALJson.ValueField(GetPropertyValue(sourceObj)?.ToString());
    }
}
