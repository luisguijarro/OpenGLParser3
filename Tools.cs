using System;
using System.Globalization;

namespace OpenGLParser
{
    public static class Tools
    {
        private static Type GetTypeFromStringValue(string value)
        {
            string s_value = value.Replace("0x", "");
            Type ret = typeof(uint);
            uint uiresult = 0;
            if (uint.TryParse(s_value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uiresult))
            {
                return typeof(uint);
            }
            else
            {
                int iresult = 0;
                if (int.TryParse(s_value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out iresult))
                {
                    return typeof(int);
                }
                else
                {
                    ulong ulresult = 0;
                    if (ulong.TryParse(s_value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ulresult))
                    {
                        return typeof(ulong);
                    }
                    else
                    {
                        long lresult = 0;
                        if (long.TryParse(s_value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out lresult))
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

        public static string GetTypeFromGLType(string tipo)
        {
            string ret = "";
            bool b_unsigned = tipo.Contains("unsigned");
            
            if (tipo.Contains(" int"))
            {
                return b_unsigned ? "uint": "int";
            }
            if (tipo.Contains(" char"))
            {
                return b_unsigned ? "byte": "sbyte";
            }
            
            if (tipo.Contains("_int8"))
            {
                return "sbyte";
            }
            if (tipo.Contains("_uint8"))
            {
                return "byte";
            }

            if (tipo.Contains("_int16"))
            {
                return "short";
            }
            if (tipo.Contains("_uint16"))
            {
                return "ushort";
            }

            if (tipo.Contains("_int32"))
            {
                return "int";
            }
            if (tipo.Contains("_uint32"))
            {
                return "uint";
            }
            
            if (tipo.Contains("_int64"))
            {
                return "long";
            }
            if (tipo.Contains("_uint64"))
            {
                return "ulong";
            }
            
            if (tipo.Contains("_ssize"))
            {
                return "int";
            }
            
            if (tipo.Contains("_float"))
            {
                return "float";
            }

            if (tipo.Contains("_double"))
            {
                return "double";
            }

            if (tipo.Contains("void *"))
            {
                return "IntPtr";
            }

            if (tipo.Contains("_intptr"))
            {
                return "IntPtr";
            }

            if (tipo.Contains("void"))
            {
                return "void";
            }

            if (tipo.Contains(" struct "))
            {
                return "IntPtr";
            }

            return ret;
        }
    }
}