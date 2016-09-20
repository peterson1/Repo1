using System;
using System.Reflection;

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
}
