using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Repo1.Core.ns12.Helpers.D7ValueFields;

namespace Repo1.Core.ns12.Helpers.D7MapperAttributes
{
    public class D7Mapper
    {
        public static Dictionary<string, object> ToObjectDictionary<T>(T origObj)
        {
            var dict  = new Dictionary<string, object>();

            AddBaseNodeFields    (dict, origObj);
            AddScalarValueFields (dict, origObj);

            return dict;
        }


        private static void AddScalarValueFields<T>(Dictionary<string, object> dict, T origObj)
        {
            foreach (var prop in typeof(T).GetRuntimeProperties())
            {
                var attr = prop.CustomAttributes.SingleOrDefault(x 
                    => x.AttributeType == typeof(ValueAttribute));
                if (attr != null)
                {
                    var key = attr.NamedArguments.SingleOrDefault(x 
                        => x.MemberName == nameof(ValueAttribute.Key)).TypedValue.Value;
                    var val = new ScalarValueField(prop, origObj);
                    dict.Add(key.ToString(), val);
                }
            }
        }


        private static void AddBaseNodeFields<T>(Dictionary<string, object> dict, T origObj)
        {
            var typ   = typeof(T);
            var props = typ.GetRuntimeProperties();

            var att = typ.GetTypeInfo().CustomAttributes.SingleOrDefault(x
                    => x.AttributeType == typeof(D7TypeAttribute));
            if (att != null) dict.Add("type", att.NamedArguments.SingleOrDefault(x
                => x.MemberName == nameof(D7TypeAttribute.Key)).TypedValue.Value);

            var prop = props.SingleOrDefault(x => x.Name == "nid");
            if (prop != null) dict.Add("nid", prop.GetValue(origObj));

            prop = props.SingleOrDefault(x => x.CustomAttributes.Any(y
                 => y.AttributeType == typeof(NodeTitleAttribute)));
            if (prop != null) dict.Add("title", prop.GetValue(origObj));

            dict.Add("status", 1);
            dict.Add("uid", 0);
        }
    }
}
