using System;
using System.Collections.Generic;
using System.Reflection;
using Repo1.Core.ns12.Helpers.StringExtensions;

namespace Repo1.Core.ns12.Helpers.D7MapperAttributes.UndFields
{
    public class ValueField
    {
        public object value { get; set; }


        internal static ValueField Wrap (PropertyInfo property, object sourceObj)
            => new ValueField { value = Cast(property, sourceObj) };


        private static object Cast(PropertyInfo property, object sourceObj)
        {
            var val = property.GetValue(sourceObj);


            if (property.PropertyType == typeof(bool))
                return (bool)val ? 1 : 0;


            if (property.PropertyType == typeof(bool?))
            {
                var nulB = (bool?)val;
                if (!nulB.HasValue) return null;
                return nulB.Value ? 1 : 0;
            }


            if (property.PropertyType == typeof(DateTime))
                return Fmt((DateTime)val);


            if (property.PropertyType == typeof(DateTime?))
                return Fmt((DateTime?)val);


            return val;
        }

        public static string Fmt(DateTime? date)
            => date?.ToString("yyyy-MM-dd HH:mm:ss");
    }


    public static class ValueFieldExtensions
    {
        public static string FieldValue(this Dictionary<string, object> dict, string fieldNme)
        {
            if (!dict.ContainsKey(fieldNme))
                throw new MissingMemberException($"Not in returned dict: “{fieldNme}”");

            var jsonStr = dict[fieldNme].ToString();

            return jsonStr.Between("{und:[{value:", "}]}");
        }
    }
}
