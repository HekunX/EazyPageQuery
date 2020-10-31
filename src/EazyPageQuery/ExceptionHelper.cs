using System;
using System.Collections.Generic;
using System.Text;

namespace EazyPageQuery
{
    public static class ExceptionHelper
    {
        public static void ThrowPropertyNotFound(string propName) => throw new PropertyNotFoundException(propName);
        
    }
}
