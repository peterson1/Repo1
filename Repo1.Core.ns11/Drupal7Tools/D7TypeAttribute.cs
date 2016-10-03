using System;
using System.Linq;
using System.Reflection;

namespace Repo1.Core.ns11.Drupal7Tools
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class D7TypeAttribute : Attribute
    {
        public string Key { get; set; }
    }


    public static class D7Type
    {
        public static string GetKey<T>()
        {
            var att = typeof(T).GetTypeInfo()
                .GetCustomAttributes<D7TypeAttribute>(true).SingleOrDefault();

            if (att == null)
                throw new MissingMemberException("Missing attribute from class: « D7Type »");

            return att.Key;
        }
    }
}
