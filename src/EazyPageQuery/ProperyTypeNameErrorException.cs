using System;
using System.Collections.Generic;
using System.Text;

namespace EazyPageQuery
{
    public class PropertyTypeNameErrorException:Exception
    {
        public PropertyTypeNameErrorException(string msg):base(msg)
        {

        }
    }
}
