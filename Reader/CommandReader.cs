using System;
using System.Xml;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glReader
    {
        public static Dictionary<string, glCommand> d_Commandos; // <NombreMetodo, Metodo>
        public static void ReadCommands(XmlDocument xdoc, bool verbose)
        {
            d_Commandos = new Dictionary<string, glCommand>();
            if (verbose) { Console.WriteLine(); Console.WriteLine("Parsing OpenGL Commands."); }

            XmlNodeList commandlist = xdoc.SelectNodes("registry/commands[@namespace='GL']/command"); //Obtenemos lista de commandos
            if (commandlist.Count > 0) //Comprobamos que se obtienen resultados.
            {
                for (int i = 0; i < commandlist.Count; i++) //Recorremos la lista
                {
                    string commandName = commandlist[i].SelectSingleNode("proto/name").InnerText; //Obtenemos el nombre del metodo.
                    string s_paramType = ""; //Declaramos tipo.

                    glCommand commandTemp = new glCommand(); //Declaracion de comando temporal.
                    
                    #region Obtener Valor de Retorno

                    if (commandlist[i].SelectSingleNode("proto/ptype") != null) //Si existe <ptype> se coge su valor
                    {
                        commandTemp.ReturnedType = d_TiposValores[commandlist[i].SelectSingleNode("proto/ptype").InnerXml]; //Obtenemos el tipo de valor que retorna el método del diccionario.
                    }
                    else
                    {
                        XmlNode nodetemp = commandlist[i].SelectSingleNode("proto").Clone(); //Obtenemos el nodo <proto> sin hijos.
                        //commandTemp.ReturnedTipe = TiposValores[nodetemp.InnerText].Replace(" ", "");
                        if (nodetemp.ChildNodes[0].NodeType == XmlNodeType.Text )
                        {
                            commandTemp.ReturnedType = d_TiposValores[nodetemp.ChildNodes[0].InnerText]; //Obtenemos tipo de valor retornado
                        }
                    }

                    if (commandlist[i].SelectSingleNode("proto").InnerText.Contains("*")) //Obtenemos si retorna puntero.
                    {
                            commandTemp.ReturnedTypePointer = true; 
                            commandTemp.EsInseguro = true;  // Si el valor retornado es puntero el metodo es inseguro.
                    }

                    #endregion

                    #region Obtener Parametrosl Metodo.
                    
                    XmlNodeList paramList = commandlist[i].SelectNodes("param"); //Obtenemos lista de parametros del metodo.
                    for (int p=0;p<paramList.Count;p++)
                    {
                        glParam paramtemp = new glParam();
                        string s_ParamName = paramList[p].SelectSingleNode("name").InnerText; //Obtenemos nombre del parametro.
                        if (paramList[p].SelectSingleNode("ptype") == null) //Si no tiene <type/> leemos texto;
                        {
                            if (paramList[p].InnerText.Contains("void *"))
                            {
                                s_paramType = "IntPtr";
                            }
                        }
                        else
                        {
                            if (paramList[p].InnerText.Contains("*")) //Si tiene asterisco es un puntero.)
                            {
                                paramtemp.esPuntero = paramList[p].InnerText.Split('*').Length-1;
                                if (paramList[p].InnerText.Contains("const")) // Si es Constante es In.
                                { 
                                    paramtemp.Acces = AccesParam.In;
                                } 
                                else
                                {
                                    if (s_ParamName.Contains("get")) // Si es un método de obtención, damos por hecho que es OUT si no tiene const.
                                    {
                                        paramtemp.Acces = AccesParam.Out;
                                    }
                                    else
                                    {
                                        // De momento la ultima excepcion la dejamos como indeterminado ante el desconocimiento.
                                    }
                                }
                            }
                            commandTemp.EsInseguro = (paramtemp.esPuntero>0) ? true : commandTemp.EsInseguro; //Indicamos si el método es inseguro o se queda como estaba.
                            s_paramType = paramList[p].SelectSingleNode("ptype").InnerText; //Obtenemos tipo del parametro.
                            s_paramType = d_TiposValores[s_paramType]; //Obtenemos el tipo

                            if (paramList[p].Attributes["group"] != null) // Si hay grupo...
                            {
                                string s_group = paramList[p].Attributes["group"].Value; //Obtenemos nombre del enumerador.
                                if (d_Enumerators.ContainsKey(s_group)) // Si el enumerador existe...
                                {
                                    s_paramType = s_group; // Tipo de valor es el enumerador.
                                }
                                else // Si no existe enumerador
                                {
                                    if (s_paramType == "GLenum") //... y el tipo es GLenum ...
                                    {
                                        s_paramType = "uint"; // ... El valor es uint.
                                    }
                                }
                            }
                        }                        
                        
                        paramtemp.tipo = s_paramType;

                        paramtemp.esArray = paramList[p].Attributes["len"] != null; //Si tiene len es un Array.

                        commandTemp.Parametros.Add(Tools.FixedParamName(s_ParamName), paramtemp); //Añadimos parametro con Corrección de nombre.
                    }

                    #endregion

                    d_Commandos.Add(commandName, commandTemp); //Añadir commanods al diccionario.
                }
            }
        

            if (verbose) 
            {                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Parsed ");
                Console.ResetColor(); 
                Console.WriteLine(d_Commandos.Count + " OpenGL Commands."); 
            }
        }
    }
}