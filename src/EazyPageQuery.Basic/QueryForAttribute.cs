using System;
using System.Collections.Generic;
using System.Text;

namespace EazyPageQuery.Basic
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class QueryForAttribute : Attribute
    {
        public string Name { get; set; }
        public QueryForAttribute(string name)
        {
            Name = name;
        }
    }
}
