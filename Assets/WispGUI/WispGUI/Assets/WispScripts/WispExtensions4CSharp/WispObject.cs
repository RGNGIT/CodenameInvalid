using System;

namespace WispExtensions
{
    public static class WispObject
    {
        // From : https://stackoverflow.com/questions/972636/casting-a-variable-using-a-type-variable
        public static T CastObject<T>(this object ParamMe)
        {
            return (T)ParamMe;
        }

        // From : https://stackoverflow.com/questions/972636/casting-a-variable-using-a-type-variable
        public static T ConvertObject<T>(this object ParamMe)
        {
            return (T)Convert.ChangeType(ParamMe, typeof(T));
        }

        public static object To(this object ParamMe, Type ParamType) // Untested
        {
            return Convert.ChangeType(ParamMe, ParamType);
        }
    }
}