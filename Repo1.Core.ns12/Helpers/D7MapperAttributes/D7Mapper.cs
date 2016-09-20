using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Repo1.Core.ns12.Helpers.D7MapperAttributes.UndFields;

namespace Repo1.Core.ns12.Helpers.D7MapperAttributes
{
    public class D7Mapper
    {
        public static Dictionary<string, object> ToObjectDictionary<T>(T origObj)
        {
            var dict  = new Dictionary<string, object>();

            AddBaseNodeFields     (dict, origObj);
            AddUndValueFields     (dict, origObj);
            AddUndTargetIdFields  (dict, origObj);

            return dict;
        }


        private static void AddUndTargetIdFields<T>(Dictionary<string, object> dict, T origObj)
        {
            foreach (var prop in typeof(T).GetRuntimeProperties())
            {
                var attr = GetAttrib<NodeAttribute>(prop);
                if (attr == null) continue;
                var key = GetKey(attr, nameof(NodeAttribute.Key));
                dict.Add(key, und.TargetID(prop, origObj));
            }
        }


        private static void AddUndValueFields<T>(Dictionary<string, object> dict, T origObj)
        {
            foreach (var prop in typeof(T).GetRuntimeProperties())
            {
                var attr = GetAttrib<ValueAttribute>(prop);
                if (attr == null) continue;
                var key = GetKey(attr, nameof(ValueAttribute.Key));
                dict.Add(key, und.Value(prop, origObj));
            }
        }


        private static string GetKey(CustomAttributeData attr, string keyPropertyName)
            => attr.NamedArguments.SingleOrDefault(x
                => x.MemberName == keyPropertyName).TypedValue.Value.ToString();


        private static CustomAttributeData GetAttrib<T>(PropertyInfo prop)
            => prop.CustomAttributes.SingleOrDefault(x
                    => x.AttributeType == typeof(T));


        private static void AddBaseNodeFields<T>(Dictionary<string, object> dict, T origObj)
        {
            var typ   = typeof(T);
            var props = typ.GetRuntimeProperties();

            var att = typ.GetTypeInfo().CustomAttributes.SingleOrDefault(x
                    => x.AttributeType == typeof(D7TypeAttribute));

            if (att == null)
                throw new MissingMemberException("Missing attribute from class: « D7Type »");

            dict.Add("type", att.NamedArguments.SingleOrDefault(x
                => x.MemberName == nameof(D7TypeAttribute.Key)).TypedValue.Value);

            var prop = props.SingleOrDefault(x => x.Name == "nid");
            if (prop != null) dict.Add("nid", prop.GetValue(origObj));

            prop = props.SingleOrDefault(x => x.Name == "uid");
            if (prop == null)
                dict.Add("uid", 0);
            else
                dict.Add("uid", prop.GetValue(origObj));

            prop = props.SingleOrDefault(x => x.CustomAttributes.Any(y
                 => y.AttributeType == typeof(NodeTitleAttribute)));
            if (prop != null) dict.Add("title", prop.GetValue(origObj));

            dict.Add("status", 1);
        }
    }
}
