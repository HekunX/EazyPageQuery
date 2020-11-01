using System;
using System.Collections.Generic;
using System.Text;

namespace EazyPageQuery
{
    public static class ExceptionHelper
    {
        public static void ThrowPropertyNotFound(string propName) => throw new PropertyNotFoundException(propName);
        public static void ThrowPropertyTypeError(string msg) => throw new PropertyTypeErrorException(msg);
        public static void ThrowPropertyNameError(string msg) => throw new PropertyTypeNameErrorException(msg);
    }
}
