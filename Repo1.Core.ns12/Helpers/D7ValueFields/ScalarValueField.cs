using System.Collections.Generic;
using System.Reflection;

namespace Repo1.Core.ns12.Helpers.D7ValueFields
{
    public class ScalarValueField
    {
        public ScalarValueField(PropertyInfo prop, object origObj)
        {
            und.Add(new ValueField
            {
                value = prop.GetValue(origObj)
            });
        }

        public List<ValueField> und { get; set; } = new List<ValueField>();
    }
}
