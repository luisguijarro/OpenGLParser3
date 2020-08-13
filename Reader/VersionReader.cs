using System;
using System.Xml;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glReader
    {
        public static Dictionary<string,glVersion> d_versiones; //<version, glversion>
        private static void ReadVersions(XmlDocument xdoc, bool verbose)
        {
            d_versiones = new Dictionary<string, glVersion>();
            if (verbose) { Console.WriteLine(); Console.WriteLine("Parsing OpenGL Versions."); }

            XmlNodeList versionlist = xdoc.SelectNodes("registry/feature[@api='gl']"); //Obtenemos lista de Versiones
            if (versionlist.Count > 0) //Comprobamos que se obtienen resultados.
            {
                for (int i = 0; i < versionlist.Count; i++) //Recorremos las Versiones
                {
                    string s_versionNumber = versionlist[i].Attributes["number"].Value; //Obtenemos el número de versión.
                    glVersion verTemp = new glVersion();

                    XmlNodeList MetodosEnVersion = versionlist[i].SelectNodes("require/command"); //Obtenemos lista de metodos soportados
                    for (int m=0;m<MetodosEnVersion.Count;m++) //Recorremos lista de metodos soportados.
                    {
                        verTemp.Metodos.Add(MetodosEnVersion[m].Attributes["name"].Value); //Añadimos metodo a la lista de soportados.
                        if (d_Commandos.ContainsKey(MetodosEnVersion[m].Attributes["name"].Value)) //Comprobanmos que el método existe.
                        {
                            if (d_Commandos[MetodosEnVersion[m].Attributes["name"].Value].FromVersion == "") //Comprovamos si ha existido antes.
                            {
                                d_Commandos[MetodosEnVersion[m].Attributes["name"].Value].FromVersion = s_versionNumber; //Establecemos esta versión de OpenGL como la primera
                            }
                        }
                    }

                    XmlNodeList MetodosEliminados = versionlist[i].SelectNodes("remove/command"); //Obtenemos lista de metodos obsoletos.
                    for (int r=0;r<MetodosEliminados.Count;r++) //Recorremos lista de metodos obsoletos.
                    {
                        verTemp.Obsoletos.Add(MetodosEliminados[r].Attributes["name"].Value); //Añadimos metodo a la lista de obsoletos.
                        if (d_Commandos.ContainsKey(MetodosEliminados[r].Attributes["name"].Value)) //Comprobanmos que el método existe.
                        {
                            if (d_Commandos[MetodosEliminados[r].Attributes["name"].Value].DeprecatedVersion == "") //Comprovamos si se ha depreciado anteriormente
                            {
                                d_Commandos[MetodosEliminados[r].Attributes["name"].Value].DeprecatedVersion = s_versionNumber; //Se marca como obsoleta a partir de esta versión.
                            }
                        }
                    }
                    d_versiones.Add(s_versionNumber, verTemp); //Añadimos Version a la lista de versiones.
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
                Console.WriteLine(d_versiones.Count + " OpenGL Versions.");
            }
        }
    }
}