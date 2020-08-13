using System;
using System.Xml;

namespace OpenGLParser
{
    public static partial class glReader
    {
        //private XmlDocument xdoc;
        public static void Parse(XmlDocument xdoc, bool verbose)
        {
            //this.xdoc = xmlDocument;
            ReadTypes(xdoc, verbose);
            ReadEnums(xdoc, verbose);
            ReadCommands(xdoc, verbose);
            ReadVersions(xdoc, verbose);
            ReadExtensions(xdoc, verbose);
        }
    }
}