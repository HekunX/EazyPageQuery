using System;
using System.Collections.Generic;
using System.Text;

namespace EazyPageQuery
{
    public class PropertyTypeErrorException:Exception
    {
        public PropertyTypeErrorException(string msg):base(msg)
        {

        }
    }
}
