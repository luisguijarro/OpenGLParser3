using System;
using System.Xml;

namespace OpenGLParser
{
    public static partial class glReader
    {
        //private XmlDocument xdoc;
        public static void Parse(XmlDocument xdoc, bool verbose, bool ogles)
        {
            //this.xdoc = xmlDocument;
            ReadTypes(xdoc, verbose);
            ReadEnums(xdoc, verbose);
            ReadCommands(xdoc, verbose);
            ReadVersions(xdoc, verbose);
            ReadExtensions(xdoc, verbose);

            if (ogles) { ReadGlesVersions(xdoc, verbose); }
            if (ogles) { ReadGlesExtensions(xdoc, verbose); }
        }
    }
}