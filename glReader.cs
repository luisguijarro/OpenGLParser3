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
            ReadEnums(xdoc, verbose);
        }
    }
}