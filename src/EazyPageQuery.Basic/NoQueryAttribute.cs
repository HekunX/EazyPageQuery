using System;
using System.Collections.Generic;
using System.Text;

namespace EazyPageQuery.Basic
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class NoQueryAttribute : Attribute
    {

    }
}
