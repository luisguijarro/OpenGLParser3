using System;
using System.Xml;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glReader
    {
        public static Dictionary<string, glCommand> Commandos; // <NombreMetodo, Metodo>
        public static void ReadCommands(XmlDocument xdoc, bool verbose)
        {
            Commandos = new Dictionary<string, glCommand>();
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
                        commandTemp.ReturnedTipe = d_TiposValores[commandlist[i].SelectSingleNode("proto/ptype").InnerXml]; //Obtenemos el tipo de valor que retorna el método del diccionario.
                    }
                    else
                    {
                        XmlNode nodetemp = commandlist[i].SelectSingleNode("proto").Clone(); //Obtenemos el nodo <proto> sin hijos.
                        //commandTemp.ReturnedTipe = TiposValores[nodetemp.InnerText].Replace(" ", "");
                        if (nodetemp.ChildNodes[0].NodeType == XmlNodeType.Text )
                        {
                            commandTemp.ReturnedTipe = d_TiposValores[nodetemp.ChildNodes[0].InnerText];
                        }
                    }
                    #endregion

                    XmlNodeList paramList = commandlist[i].SelectNodes("param"); //Obtenemos lista d eparametros del metodo.
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
                            paramtemp.esPuntero = paramList[p].InnerText.Contains("*"); //Si tiene asterisco es un puntero.
                            commandTemp.EsInseguro = paramtemp.esPuntero ? true : commandTemp.EsInseguro; //Indicamos si el método es inseguro o se queda como estaba.
                            s_paramType = paramList[p].SelectSingleNode("ptype").InnerText; //Obtenemos tipo del parametro.
                            if (s_paramType == "GLenum") //Si es un enumerador 
                            {
                                if (paramList[p].Attributes["group"] != null)
                                {
                                    s_paramType = paramList[p].Attributes["group"].Value; //Obtenemos nombre dle enumerador.
                                }
                                else
                                {
                                    s_paramType = "uint";
                                }
                            }
                            else
                            {
                                s_paramType = d_TiposValores[s_paramType];
                            }
                        }                        
                        
                        paramtemp.tipo = s_paramType;

                        paramtemp.esArray = paramList[p].Attributes["len"] != null; //Si tiene len es un Array.

                        commandTemp.Parametros.Add(Tools.FixedParamName(s_ParamName), paramtemp); //Añadimos parametro con Corrección de nombre.
                    }


                    Commandos.Add(commandName, commandTemp); //Añadir commanods al diccionario.
                }
            }
        

            if (verbose) 
            {                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Parsed ");
                Console.ResetColor(); 
                Console.WriteLine(Commandos.Count + " OpenGL Commands."); 
            }
        }
    }
}