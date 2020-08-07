using System;
using System.Xml;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glReader
    {
        public static Dictionary<string, glEnum> d_Enumerators; //Nombre del enumerador, Enumerador.
        private static Dictionary<string,string> d_Valores; //Almacena todos los valores. <nombre,valor>
        private static void ReadEnums(XmlDocument xdoc, bool verbose)
        {
            d_Enumerators = new Dictionary<string, glEnum>();
            d_Valores = new Dictionary<string, string>();

            //Lo primero es leer todos los Enumeradores y sus valores.
            if (verbose) { Console.WriteLine(); Console.WriteLine("Parsing OpenGL Enumerators."); }

            XmlNodeList enumlist = xdoc.SelectNodes("registry/enums[@namespace='GL']"); //Obtenemos listas de enumeradores
            if (enumlist.Count > 0) //Comprobamos que se obtienen resultados.
            {
                for (int i = 0; i < enumlist.Count; i++) //Recorremos las listas
                {
                    XmlNodeList enumvalues = enumlist[i].SelectNodes("enum"); //Obtenemos los valores de cada lista de valores de enum
                    if (enumvalues.Count > 0) //Confirmamos que hay enums en la lista.
                    {
                        for (int a = 0; a < enumvalues.Count; a++) //Recorremos los valores del enumerador
                        {
                            string s_val = enumvalues[a].Attributes["value"].Value; //Obtenemos el Valor
                            string s_valname = enumvalues[a].Attributes["name"].Value; //Obtenemos el nombre del Valor

                            if (!d_Valores.ContainsKey(s_valname)) //Comprobamos que el diccionario no tenga ya el valor
                            {
                                d_Valores.Add(s_valname, s_val); //Añadimos el valor al dicionario
                            }
                        }
                    }
                }
            }

            //Leemos los grupos, les adjuntamos los valores y así definimos los Enumeradores.
            int ctop = Console.CursorTop;

            XmlNodeList grouplist = xdoc.SelectNodes("registry/groups/group"); //Obtenemos la lista de grupos
            if (grouplist.Count > 0) // Comprobamos que se obtengan resultados.
            {
                for (int i = 0; i < grouplist.Count; i++) //Recorremos los grupos
                {
                    string s_groupName = grouplist[i].Attributes["name"].Value; //Obtenemos el nombre del grupo. ¡¡¡ManOwaR!!!

                    if (!d_Enumerators.ContainsKey(s_groupName)) //Comprobamos que no exista ya el Enumerador.
                    {
                        XmlNodeList groupvalues = grouplist[i].SelectNodes("enum"); //Obtenemos el listado de los nombres de los valores contenidos.
                        if (groupvalues.Count > 0) //Comprobamos que tenga alguno. No siempre es así y no tiene sentido un grupo vacio.
                        {
                            glEnum tempgroup = new glEnum(); //Creamos el enumerador correspondiente al grupo.
                            for (int a = 0; a < groupvalues.Count; a++) //Recorremos la lista de nombres de valores.
                            {
                                string s_valname = groupvalues[a].Attributes["name"].Value; // Obtenemos el nombre del valor.
                                if (d_Valores.ContainsKey(s_valname)) //Comprobamos que existe valor para este nombre de valor.
                                {
                                    if (!tempgroup.EnumValues.ContainsKey(s_valname)) //Comprovamos si ya existe en el enumerador
                                    {
                                        tempgroup.EnumValues.Add(s_valname, new glEnumValue(s_valname,d_Valores[s_valname])); //Si no existe lo añadimos.
                                        tempgroup.Tipo = Tools.GetPrevailingType(tempgroup.Tipo, d_Valores[s_valname]);
                                    }
                                }
                                else
                                {
                                    if (verbose) //Mostramos el error de Parseo.
                                    {
                                        Console.SetCursorPosition(0,ctop);
                                        Console.Write(new String(' ', Console.BufferWidth)); //Limpiamos linea a sobreescribir.
                                        Console.SetCursorPosition(0,ctop);
                                        Console.WriteLine("    - Enum Parse Error: Value to " + s_valname + "not finded.");
                                    }
                                }
                            }
                            d_Enumerators.Add(s_groupName, tempgroup); // Añadimos Enumerador con valores al Diccionario.
                            if (verbose) //Mostramos el Enumerador Parseado.
                            {
                                Console.SetCursorPosition(0,ctop);
                                Console.Write(new String(' ', Console.BufferWidth)); //Limpiamos linea a sobreescribir.
                                Console.SetCursorPosition(0,ctop);
                                Console.Write("    - Enums Parsed "+d_Enumerators.Count.ToString("D3")+": "+s_groupName);
                            }
                        }
                    }
                }
            }
            if (verbose) //Mostrar Recuento final.
            {
                Console.SetCursorPosition(0,ctop);
                Console.Write(new String(' ', Console.BufferWidth)); //Limpiamos linea a sobreescribir.
                Console.SetCursorPosition(0,ctop);
                Console.WriteLine("Parsed " + d_Enumerators.Count + " OpenGL Enumerators.");
                Console.WriteLine();
            }
        }
    }
}