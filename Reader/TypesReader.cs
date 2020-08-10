using System;
using System.Xml;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glReader
    {
        public static Dictionary<string, string> TiposValores; // <nombre dle tipo, tipoequivalente>
        public static void ReadTypes(XmlDocument xdoc, bool verbose)
        {
            TiposValores = new Dictionary<string, string>();
            TiposValores.Add("void ", "void"); //Añadimos Tipo Base void por defecto.
            TiposValores.Add("void *", "IntPtr"); //Añadimos Tipo Base void * por defecto.

            if (verbose) { Console.WriteLine(); Console.WriteLine("Parsing OpenGL Types."); }

            XmlNodeList typeslist = xdoc.SelectNodes("registry/types/type"); //Obtenemos lista de tipos
            if (typeslist.Count > 0) //Comprobamos que se obtienen resultados.
            {
                for (int i = 0; i < typeslist.Count; i++) //Recorremos la lista
                {
                    if (typeslist[i].ChildNodes.Count > 0) //Nos aseguramos de que hay Hijos.
                    {
                        string v_name = ""; //Declaramos el nombre del tipo de variable
                        string v_type = ""; //Declaramos el tipo equivalente.

                        if (typeslist[i].Attributes["name"] != null) //Comporbamos si no es definición normal.
                        {
                            v_name = typeslist[i].Attributes["name"].Value; // Obtenemos el nombre del tipo
                            if (typeslist[i].InnerText.Contains("unsigned int")) //Solo cogemos lo que no es required
                            {
                                v_type = "uint";
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else //Si la declaración es normal...
                        {
                            v_name = typeslist[i].SelectSingleNode("name").InnerText; //Obtenemos el nombre del tipo GL
                            if (typeslist[i].SelectSingleNode("apientry") != null) //Si tiene <apientry/> es un delegado
                            {
                                v_type = "delegate";
                            }
                            else
                            {
                                if (v_name.Contains("struct ")) //Si el nombre tiene Strcut es struct
                                {
                                    //v_name = (v_name.Contains("struct")) ? v_name.Replace("struct ", "") : v_name; //Se elimina del nombre struct
                                    v_type = "IntPtr"; //Se asigna tipo de valor a IntPtr
                                }
                                else
                                {
                                    if (v_name == "GLboolean")
                                    {
                                        v_type = "bool";
                                    }
                                    else
                                    {
                                        v_type = Tools.GetTypeFromGLType(typeslist[i].InnerText); //Obtenemos el tipo de valor.
                                    }
                                }
                            }
                        }

                        TiposValores.Add(v_name, v_type); // Añadimos el valor y si equivalencia;
                    }
                }
            }

            if (verbose) 
            { 
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Parsed ");
                Console.ResetColor(); 
                Console.WriteLine(TiposValores.Count + " OpenGL Types."); 
            }
        }
    }
}