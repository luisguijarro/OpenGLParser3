using System;
using System.Xml;

namespace OpenGLParser
{
    public static partial class glWriter
    {
        public static void Write(string NameSpace, string outpath, bool verbose)
        {
            WriteEnums(NameSpace, outpath, verbose);
        }
    }
}