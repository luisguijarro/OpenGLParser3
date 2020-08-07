using System;
using System.Xml;

namespace OpenGLParser
{
    public static class glParser
    {
        private static XmlDocument xdoc;
        public static void Parse(string rutaxml, string @namespace, string destination, bool verbose)
        {
            xdoc = new XmlDocument();
            xdoc.Load(rutaxml);

            //Procesar Parseo fase a fase.
            glReader.Parse(xdoc, verbose);

            glWriter.Write(@namespace, destination, verbose);
        }
    }
}