using System;
using System.Collections.Generic;
using System.Reflection;

namespace Repo1.Core.ns12.Helpers.D7MapperAttributes.UndFields
{
    public class UndContainer<T>
    {
        public List<T>  und  { get; set; } = new List<T>();
    }


    public class und
    {
        public static UndContainer<ValueField> Value(PropertyInfo propertyInf, object sourceObj)
        {
            var objVal     = propertyInf.GetValue(sourceObj);
            var valueField = new ValueField { value = objVal };
            var container  = new UndContainer<ValueField>();
            container.und.Add(valueField);
            return container;
        }


        public static UndContainer<TargetIdField> TargetID (PropertyInfo propertyInf, object sourceObj)
        {
            var nodeObj   = propertyInf.GetValue(sourceObj);
            var nidProp   = nodeObj.GetType().GetRuntimeProperty("nid");
            if (nidProp == null)
                throw new MissingMemberException($"No “nid” property found in {propertyInf.Name}.");

            var nid       = int.Parse(nidProp.GetValue(nodeObj).ToString());
            var targF     = new TargetIdField { target_id = nid };
            var container = new UndContainer<TargetIdField>();
            container.und.Add(targF);
            return container;
        }
    }
}
