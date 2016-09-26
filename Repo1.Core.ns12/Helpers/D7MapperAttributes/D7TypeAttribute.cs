using System;
using System.Linq;
using System.Reflection;

namespace Repo1.Core.ns12.Helpers.D7MapperAttributes
{
    public class D7TypeAttribute : Attribute
    {
        public string Key { get; set; }
    }


    public static class D7Type
    {
        public static string GetKey<T>()
        {
            var att = typeof(T).GetTypeInfo().CustomAttributes.SingleOrDefault(x
                    => x.AttributeType == typeof(D7TypeAttribute));

            if (att == null)
                throw new MissingMemberException("Missing attribute from class: « D7Type »");

            return att.NamedArguments.SingleOrDefault(x
                => x.MemberName == nameof(D7TypeAttribute.Key)).TypedValue.Value.ToString();
        }
    }
}
