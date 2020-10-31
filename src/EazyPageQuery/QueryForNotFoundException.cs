using System;
using System.Collections.Generic;
using System.Text;

namespace EazyPageQuery
{
    public sealed class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException(string mapForName) : base($"Can not find the property called {mapForName},Please Check if it is a property type or the property is missing") { }
    }
}
