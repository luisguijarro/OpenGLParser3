using System;
using System.Xml;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glReader
    {
        public static Dictionary<string, glExtension> d_Extensions; //Listado de Extensiones y sus metodos.
        private static void ReadExtensions(XmlDocument xdoc, bool verbose)
        {
            //d_versiones = new Dictionary<string, glVersion>();
            if (verbose) { Console.WriteLine(); Console.WriteLine("Parsing OpenGL Extensions."); }

            XmlNodeList extensionlist = xdoc.SelectNodes("registry/extensions/extension[@supported='gl']"); //Obtenemos lista de Extensiones
            if (extensionlist.Count > 0) //Comprobamos que se obtienen resultados.
            {
                for (int i = 0; i < extensionlist.Count; i++) //Recorremos las Extensiones
                {
                    string s_extension = extensionlist[i].Attributes["name"].Value; //Obtenemos el nombre e la extensión.
                    string s_gr = s_extension.Split('_')[1]; // Recuperamos el nombre de definicion del grupo ej: AMD, NV, ARB, EXT....

                    XmlNodeList extCommands = extensionlist[i].SelectNodes("require/command");

                    if (extCommands.Count > 0) //Comprobar si hay comandos en la extensión.
                    {
                        if (!d_Extensions.ContainsKey(s_gr))
                        {
                            d_Extensions.Add(s_gr, new glExtension());
                        }
                        for (int c=0;c<extCommands.Count;c++) //Recorremos los comandos.
                        {
                            string s_metodo = extCommands[c].Attributes["name"].Value; // Obtenemos nombre del método.
                            if (!d_Extensions[s_gr].Metodos.Contains(s_metodo)) // Si no está en la lista...
                            {
                                d_Extensions[s_gr].Metodos.Add(s_metodo); // ...añadimos el Método.
                            }
                        }                        
                    }
                }
            }
        }
    }
}