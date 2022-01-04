using System;
using System.Xml;

namespace OpenGLParser
{
    public static class glParser
    {
        private static XmlDocument xdoc;
        public static void Parse(string rutaxml, string @namespace, string destination, bool verbose, bool gitRefPages, bool ogles)
        {
            xdoc = new XmlDocument();
            xdoc.Load(rutaxml);

            //Procesar Parseo fase a fase.
            glReader.Parse(xdoc, verbose, ogles);

            if (gitRefPages)
            {
                DocuParser.CloneFromGit();
                if (DocuParser.Parse21()) // Si el parseo es correcto....
                {
                    
                }    
                if (DocuParser.Parse4()) // Si el parseo es correcto....
                {
                    
                }
                DocuParser.CompleteEnums();            
            }

            //Escribir archivos .cs
            glWriter.Write(@namespace, destination, verbose);
        }
    }
}