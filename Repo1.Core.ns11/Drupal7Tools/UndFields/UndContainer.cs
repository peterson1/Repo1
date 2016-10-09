using System;
using System.Collections.Generic;
using System.Reflection;

namespace Repo1.Core.ns11.Drupal7Tools.UndFields
{
    public class UndContainer<T>
    {
        public UndContainer(T item)
        {
            und.Add(item);
        }


        public List<T> und { get; set; } = new List<T>();
    }


    public class und
    {
        public static UndContainer<ValueField> Value(PropertyInfo propertyInf, object sourceObj)
            => new UndContainer<ValueField>(ValueField.Wrap(propertyInf, sourceObj));


        public static UndContainer<TargetIdField> TargetID(PropertyInfo propertyInf, object sourceObj)
        {
            var nodeObj = propertyInf.GetValue(sourceObj);
            var nidProp = nodeObj.GetType().GetRuntimeProperty("nid");
            if (nidProp == null)
                throw new MissingMemberException($"No “nid” property found in {propertyInf.Name}.");

            var nid       = int.Parse(nidProp.GetValue(nodeObj).ToString());
            var targF     = new TargetIdField { target_id = nid };
            var container = new UndContainer<TargetIdField>(targF);
            return container;
        }


        public static UndContainer<TermIdField> TermID(PropertyInfo propertyInf, object sourceObj)
            => new UndContainer<TermIdField>(TermIdField.Wrap(propertyInf, sourceObj));
    }
}
