using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EazyPageQuery
{
    internal static class Utility
    {
        /// <summary>
        /// 获得目标属性名
        /// </summary>
        /// <param name="propertyInfos">要搜索的数组</param>
        /// <param name="name">全名</param>
        /// <param name="suffix">后缀</param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfoTrimEnd(PropertyInfo[] propertyInfos,string name,string suffix)
        {
            return propertyInfos.FirstOrDefault(x => x.Name == name.TrimEnd(suffix.ToArray()));
        }

        public static PropertyInfo GetPropertyInfo(PropertyInfo[] propertyInfos, string name) => GetPropertyInfoTrimEnd(propertyInfos, name, "");

        
    }
}
