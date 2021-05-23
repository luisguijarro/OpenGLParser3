using System;
using System.Xml;

namespace OpenGLParser
{
    public static partial class glWriter
    {
        public static void Write(string NameSpace, string outpath, bool verbose)
        {
            WriteEnums(NameSpace, outpath, verbose);
            WriteInternals(NameSpace, outpath, verbose);
            WriteDelegates(NameSpace, outpath, verbose);
            WriteInternalTools(NameSpace, outpath, verbose);
            WriteDelegateInitializer(NameSpace, outpath, verbose);
            WriteDelegateInitializerEXT(NameSpace, outpath, verbose);
            WriteCommands(NameSpace, outpath, verbose);
            WriteEXTCommands(NameSpace, outpath, verbose);
        }
    }
}