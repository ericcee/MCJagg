using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg
{
    public static class ExtensionMethods
    {
        public static void AddIfNotNull<T>(this List<T> list, T item)
        {
            if (item != null) list.Add(item);
        }
        public static void AddIfNotNull<T>(this List<T> list, T item, int times)
        {
            if (item != null)
                for (int i=0; i<times; i++)
                    list.Add(item);
        }
    }
}
