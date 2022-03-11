using System;
using System.Xml;

namespace OpenGLParser
{
    public static partial class glWriter
    {
        public static void Write(string NameSpace, string outpath, bool verbose, bool ogles)
        {
            WriteEnums(NameSpace, outpath, verbose);
            WriteInternals(NameSpace, outpath, verbose);
            WriteDelegates(NameSpace, outpath, verbose);
            WriteInternalTools(NameSpace, outpath, verbose);
            WriteDelegateInitializer(NameSpace, outpath, verbose);
            WriteDelegateInitializerEXT(NameSpace, outpath, verbose);
            WriteCommands(NameSpace, outpath, verbose);
            WriteEXTCommands(NameSpace, outpath, verbose);

            if (ogles)
            {
                if (!System.IO.Directory.Exists(outpath+"/gles"))
                {
                    System.IO.Directory.CreateDirectory(outpath+"/gles");
                }
                WriteGLesInternals(NameSpace, outpath+"/gles/", verbose);
                WriteGlesDelegates(NameSpace, outpath+"/gles/", verbose);
                WriteInternalGLesTools(NameSpace, outpath+"/gles/", verbose);
                WriteDelegateInitializerGles(NameSpace, outpath+"/gles/", verbose);
                WriteDelegateInitializerGLesEXT(NameSpace, outpath+"/gles/", verbose);
                WriteGlesCommands(NameSpace, outpath+"/gles/", verbose);
                WriteGlesEXTCommands(NameSpace, outpath+"/gles/", verbose);
            }
        }
    }
}