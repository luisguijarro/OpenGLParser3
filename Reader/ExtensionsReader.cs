using System;
using System.Xml;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glReader
    {
        public static Dictionary<string, glExtension> d_Extensions; //Listado de Extensiones y sus metodos.
        public static Dictionary<string, glExtension> d_Gles_Extensions; //Listado de Extensiones y sus metodos.
        private static void ReadExtensions(XmlDocument xdoc, bool verbose)
        {
            d_Extensions = new Dictionary<string, glExtension>();
            if (verbose) { Console.WriteLine(); Console.WriteLine("Parsing OpenGL Extensions."); }

            XmlNodeList extensionlist = xdoc.SelectNodes("registry/extensions/extension[@supported='gl']"); //Obtenemos lista de Extensiones
            if (extensionlist.Count > 0) //Comprobamos que se obtienen resultados.
            {
                for (int i = 0; i < extensionlist.Count; i++) //Recorremos las Extensiones
                {
                    string s_extension = extensionlist[i].Attributes["name"].Value; //Obtenemos el nombre e la extensión.
                    string s_gr = s_extension.Split('_')[1]; // Recuperamos el nombre de definicion del grupo ej: AMD, NV, ARB, EXT....
                    int i_try = 0;
                    if (int.TryParse(s_gr[0].ToString(), out i_try)) //Comprobamos si empieza por número. El nombre de una clase no puede empezar por número.
                    {
                        s_gr = "_"+s_gr; //Añadimos guión bajo delante. El nombre de una clase no puede empezar por un número.
                    }

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

            if (verbose) //Mostrar Recuento final.
            {
                /*Console.SetCursorPosition(0,ctop);
                Console.Write(new String(' ', Console.BufferWidth)); //Limpiamos linea a sobreescribir.
                Console.SetCursorPosition(0,ctop);*/
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Parsed ");
                Console.ResetColor(); 
                Console.WriteLine(d_Extensions.Count + " OpenGL Extensions.");
            }
        }
    
        private static void ReadGlesExtensions(XmlDocument xdoc, bool verbose)
        {
            d_Gles_Extensions = new Dictionary<string, glExtension>();
            if (verbose) { Console.WriteLine(); Console.WriteLine("Parsing OpenGL|ES Extensions."); }

            XmlNodeList extensionlist = xdoc.SelectNodes("registry/extensions/extension[contains(@supported,'gles')]"); //Obtenemos lista de Extensiones de OpenGL|ES
            if (extensionlist.Count > 0) //Comprobamos que se obtienen resultados.
            {
                for (int i = 0; i < extensionlist.Count; i++) //Recorremos las Extensiones
                {
                    string s_extension = extensionlist[i].Attributes["name"].Value; //Obtenemos el nombre e la extensión.
                    string s_gr = s_extension.Split('_')[1]; // Recuperamos el nombre de definicion del grupo ej: AMD, NV, ARB, EXT....
                    int i_try = 0;
                    if (int.TryParse(s_gr[0].ToString(), out i_try)) //Comprobamos si empieza por número. El nombre de una clase no puede empezar por número.
                    {
                        s_gr = "_"+s_gr; //Añadimos guión bajo delante. El nombre de una clase no puede empezar por un número.
                    }

                    XmlNodeList extCommands = extensionlist[i].SelectNodes("require/command");

                    if (extCommands.Count > 0) //Comprobar si hay comandos en la extensión.
                    {
                        if (!d_Gles_Extensions.ContainsKey(s_gr))
                        {
                            d_Gles_Extensions.Add(s_gr, new glExtension());
                        }
                        for (int c=0;c<extCommands.Count;c++) //Recorremos los comandos.
                        {
                            string s_metodo = extCommands[c].Attributes["name"].Value; // Obtenemos nombre del método.
                            if (!d_Gles_Extensions[s_gr].Metodos.Contains(s_metodo)) // Si no está en la lista...
                            {
                                d_Gles_Extensions[s_gr].Metodos.Add(s_metodo); // ...añadimos el Método.
                            }
                        }                        
                    }
                }
            }

            if (verbose) //Mostrar Recuento final.
            {
                /*Console.SetCursorPosition(0,ctop);
                Console.Write(new String(' ', Console.BufferWidth)); //Limpiamos linea a sobreescribir.
                Console.SetCursorPosition(0,ctop);*/
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Parsed ");
                Console.ResetColor(); 
                Console.WriteLine(d_Gles_Extensions.Count + " OpenGL|ES Extensions.");
            }
        }
    }
}