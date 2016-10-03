using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Repo1.Core.ns11.Drupal7Tools.UndFields;

namespace Repo1.Core.ns11.Drupal7Tools
{
    public static class D7Mapper
    {
        public static Dictionary<string, object> ToObjectDictionary<T>(T origObj)
        {
            var dict = new Dictionary<string, object>();

            AddBaseNodeFields(dict, origObj);
            AddUndValueFields(dict, origObj);
            AddUndTargetIdFields(dict, origObj);

            return dict;
        }


        public static void SetNodeIDs<T>(this Dictionary<string, object> dict, T nodeObj)
        {
            CopyIntPropertyToNode("nid", dict, nodeObj);
            CopyIntPropertyToNode("uid", dict, nodeObj);
            CopyIntPropertyToNode("vid", dict, nodeObj);
        }


        private static void CopyIntPropertyToNode<T>(string propertyName, Dictionary<string, object> dict, T node)
        {
            object val = null;
            int id = 0;
            if (!dict.TryGetValue(propertyName, out val)) return;
            if (val == null) return;
            if (!int.TryParse(val.ToString(), out id)) return;
            if (id == 0) return;

            var prop = typeof(T).GetRuntimeProperty(propertyName);
            prop?.SetValue(node, id);
        }


        private static void CopyPropertyFromNode<T>(string propertyName, T node, Dictionary<string, object> dict)
        {
            if (node == null) return;
            var prop = typeof(T).GetRuntimeProperty(propertyName);
            if (prop == null) return;
            dict.Add(propertyName, prop.GetValue(node));
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


        public static Dictionary<string, object> CopyNodeIDs<T>(T node)
        {
            var dict = new Dictionary<string, object>();
            CopyPropertyFromNode("nid", node, dict);
            CopyPropertyFromNode("uid", node, dict);
            CopyPropertyFromNode("vid", node, dict);
            return dict;
        }


        private static void AddBaseNodeFields<T>(Dictionary<string, object> dict, T origObj)
        {
            //var typ   = typeof(T);
            var props = typeof(T).GetRuntimeProperties();

            //var att = typ.GetTypeInfo().CustomAttributes.SingleOrDefault(x
            //        => x.AttributeType == typeof(D7TypeAttribute));

            //if (att == null)
            //    throw new MissingMemberException("Missing attribute from class: « D7Type »");

            //dict.Add("type", att.NamedArguments.SingleOrDefault(x
            //    => x.MemberName == nameof(D7TypeAttribute.Key)).TypedValue.Value);

            dict.Add("type", D7Type.GetKey<T>());

            var prop = props.SingleOrDefault(x => x.Name == "nid");
            if (prop != null) dict.Add("nid", prop.GetValue(origObj));

            prop = props.SingleOrDefault(x => x.Name == "uid");
            if (prop == null)
                dict.Add("uid", 0);
            else
                dict.Add("uid", prop.GetValue(origObj));


            prop = props.SingleOrDefault(x => x.Name == "vid");
            if (prop != null)
            {
                var val = int.Parse(prop.GetValue(origObj).ToString());
                if (val > 0) dict.Add("vid", val);
            }


            prop = props.SingleOrDefault(x => x.CustomAttributes.Any(y
                 => y.AttributeType == typeof(NodeTitleAttribute)));
            if (prop != null) dict.Add("title", prop.GetValue(origObj));

            dict.Add("status", 1);
            //dict.Add("language", "und");
        }
    }
}
