using System;

namespace OpenGLParser
{
    public static class Tools
    {
        private static Type GetTypeFromStringValue(string s_value)
        {
            Type ret = typeof(uint);
            uint uiresult = 0;
            if (uint.TryParse(s_value,out uiresult))
            {
                return typeof(uint);
            }
            else
            {
                int iresult = 0;
                if (int.TryParse(s_value,out iresult))
                {
                    return typeof(int);
                }
                else
                {
                    ulong ulresult = 0;
                    if (ulong.TryParse(s_value,out ulresult))
                    {
                        return typeof(ulong);
                    }
                    else
                    {
                        long lresult = 0;
                        if (long.TryParse(s_value,out lresult))
                        {
                            return typeof(long);
                        }
                    }
                }
            }
            return ret;
        }
    
        private static Type GetPrevailingType(Type t1, Type t2)
        {
            Type t_uint = typeof(int);
            Type t_int = typeof(int);
            Type t_ulong = typeof(int);
            Type t_long = typeof(int);

            if (t1 == t_uint)
            {
                if (t2 == t_int)
                {
                    return t1;
                }
                if (t2 == t_ulong)
                {
                    return t2;
                }
                if (t2 == t_long)
                {
                    return t2;
                }
                return t1;
            }

            if (t1 == t_int)
            {
                if (t2 == t_uint)
                {
                    return t2;
                }
                if (t2 == t_ulong)
                {
                    return t2;
                }
                if (t2 == t_long)
                {
                    return t2;
                }
                return t1;
            }


            if (t1 == t_long)
            {
                if (t2 == t_int)
                {
                    return t1;
                }
                if (t2 == t_uint)
                {
                    return t1;
                }
                if (t2 == t_ulong)
                {
                    return t2;
                }
                return t1;
            }

            if (t1 == t_ulong)
            {
                return t1;
            }
            return t1; //Para que no falle la compilacion.
        }
    
        public static Type GetPrevailingType(Type t1, string s_value)
        {
            return GetPrevailingType(t1, GetTypeFromStringValue(s_value));
        }
    }
}