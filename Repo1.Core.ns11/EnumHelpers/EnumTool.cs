using System;
using System.Collections.Generic;

namespace Repo1.Core.ns11.EnumHelpers
{
    public class EnumTool
    {
        public static List<T> List<T>()
        {
            var list = new List<T>();

            foreach (T item in Enum.GetValues(typeof(T)))
                list.Add(item);

            return list;
        }
    }
}
