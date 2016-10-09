using System;
using System.Reflection;

namespace Repo1.Core.ns11.Drupal7Tools.UndFields
{
    public class TermIdField
    {
        public int tid { get; set; }


        internal static TermIdField Wrap (PropertyInfo property, object sourceObj)
        {
            if (sourceObj == null) return null;
            var val = property.GetValue(sourceObj);
            if (val == null) return null;
            var tid = Convert.ToInt32(val);

            return new TermIdField { tid = tid };
        }
    }
}
