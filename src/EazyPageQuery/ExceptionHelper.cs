using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EazyPageQuery
{
    public static class ExceptionHelper
    {
        public static void ThrowPropertyNotFound(string propName) => throw new PropertyNotFoundException(propName);
        public static void ThrowPropertyTypeError(string msg) => throw new PropertyTypeErrorException(msg);
        public static void ThrowPropertyNameError(string msg) => throw new PropertyTypeNameErrorException(msg);

        public static void ThrowNotSupportedError(string msg) => throw new NotSupportedException(msg);

        public static void ThrowArgNullExceptionIfNull(string name,object value)
        {
            if(value is null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
