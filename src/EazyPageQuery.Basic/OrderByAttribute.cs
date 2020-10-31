using System;
using System.Collections.Generic;
using System.Text;

namespace EazyPageQuery.Basic
{
    public enum OrderType:int
    {
        /// <summary>
        /// 升序
        /// </summary>
        Ascending,
        /// <summary>
        /// 降序
        /// </summary>
        Descending
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class OrderByAttribute:Attribute
    {
        public int Order { get; set; } = 0;
        public OrderType OrderType { get; set; } = OrderType.Ascending;
    }
}
