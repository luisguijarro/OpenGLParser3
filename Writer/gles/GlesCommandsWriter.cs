using System;
using System.IO;
using System.Collections.Generic;


using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glWriter
    {
        private static void WriteGlesCommands(string NameSpace, string outpath, bool verbose)
        {
            if (verbose) //Si Verbose mode mostramos inicio del proceso.
            {
                Console.WriteLine(); Console.WriteLine("Generating File: GLesMethods.cs");
            }
            if (!Directory.Exists(outpath)) // Si la ruta no existe la creamos
            {
                Directory.CreateDirectory(outpath);
            }
            if (File.Exists(outpath + "GLesMethods.cs")) //Si existe algun archivo previo lo eliminamos.
            {
                File.Delete(outpath + "GLesMethods.cs");
            }
            StreamWriter file = File.CreateText(outpath + "GLesMethods.cs"); //Generamos Contenido del archivo.
            file.WriteLine("// OpenGL|ES Methods.");
            file.WriteLine("// File Created with OpenGL Parser 3.");
            file.WriteLine("// Developed by Luis Guijarro Pérez.");
            file.WriteLine();

            file.WriteLine("using System;");
            file.WriteLine("using System.Text;");
            file.WriteLine("using System.Runtime.InteropServices;");
            file.WriteLine();
            file.WriteLine("namespace " + NameSpace + ".OpenGL");
            file.WriteLine("{");

            string tab = "\t"; //Definimos tabulación.

            file.WriteLine(tab+"public static partial class GLES"); //Declaramos Clase Estatica contenedora de los métodos.
            file.WriteLine(tab+"{"); //Abrimos clase 

            List<string> CommandsKeysList = new List<string>(glReader.d_Commandos.Keys); //Creamos lista de nombres de comandos para ordenar.
            CommandsKeysList.Sort(); //Ordenamos lista alfabeticamente.

            char LastFirstLetter = ' '; // Creamos variable para recoger la ultima primera letra de metodo empleada.

            for (int key = 0;key<CommandsKeysList.Count;key++) //Recorremos la lista con los nombres de los Comandos
            {
                bool IsGles = false;
                foreach (glVersion vers in glReader.d_gles_versiones.Values)
                {
                    if (vers.Metodos.Contains(CommandsKeysList[key]))
                    {
                        IsGles = true;
                    }
                }

                if (!IsGles) { continue; } // Si no es de OpenGL|ES nos lo saltamos.

                //Definir Regiones Alfabeticas.
                DataObjects.glCommand commandTemp = glReader.d_Commandos[CommandsKeysList[key]]; //Recuperamos el comando.

                if (commandTemp.FromGlesVersion.Length > 0) //Solo escribimos los que Estan en alguna versión de Gles.
                {
                    char ActualLetter = CommandsKeysList[key].Replace("gl", "").Substring(0,1).ToCharArray()[0];

                    if (ActualLetter != LastFirstLetter) //Si la nueva letra no es la ultima
                    {
                        if (LastFirstLetter != ' ') //Comprovamos que no es la primera
                        {
                            file.WriteLine(tab+tab+"#endregion"); //Cerramos región
                            file.WriteLine();
                        }
                        LastFirstLetter = ActualLetter; //Establecemos nueva letra
                        file.WriteLine(tab+tab+"#region "+LastFirstLetter.ToString().ToUpper()+":"); //Abrimos región
                        file.WriteLine();
                    }

                    //Escribir info de Versión.
                    string s_comentario = tab+tab+"///<sumary> ";
                    if (commandTemp.FromGlesVersion.Length>0)
                    {
                        s_comentario += "Available from OpenGL|ES version " + commandTemp.FromGlesVersion;
                        
                    }
                    s_comentario += commandTemp.DeprecatedGlesVersion.Length > 0 ? " | Deprecated in OpenGL|ES version " + commandTemp.DeprecatedGlesVersion : "";
                    s_comentario += "</sumary>";

                    //Ahora Escribir el Método.
                    string s_metodo = tab+tab+"public static "; //Iniciamos escritura del método.
                    s_metodo += commandTemp.EsInseguro ? "unsafe " : "";
                    s_metodo += commandTemp.ReturnedType + (commandTemp.ReturnedTypePointer? "* " : " ") + CommandsKeysList[key]+"(";
                    foreach(string keyParam in commandTemp.Parametros.Keys) //Recorremos lista deparametros para añadirlos uno a uno.
                    {                    
                        glParam param = commandTemp.Parametros[keyParam]; //Obtenemos el parametro.
                        //s_metodo += param.tipo + (param.esArray? "[] ": " ") + keyParam + ", "; //Añadimos tipo, si es array y el nombre del parametro.
                        
                        string s_tipo = param.tipo;
                        string s_ptn = "";
                        if (param.esPuntero>0)
                        {              
                            if (s_tipo == "char")
                            {
                                if (param.Acces == AccesParam.In)
                                {
                                    s_tipo = "[MarshalAs(UnmanagedType.LPStr)] string" + ((param.esPuntero>1) ? "[]" : "");
                                }
                                else
                                {
                                    s_tipo = "StringBuilder" + ((param.esPuntero>1) ? "[]" : "");
                                }
                            }
                            else
                            {
                                for (int ptn=0;ptn<param.esPuntero;ptn++)
                                {
                                    s_ptn += "*";
                                }
                            }                  
                        }
                        s_metodo += s_tipo + s_ptn + " " + keyParam + ", "; //Añadimos tipo, si es puntero y numero de asteriscos y el nombre del parametro.
                        //s_metodo += param.tipo + ((param.esPuntero>0) ? "* ": " ") + keyParam + ", "; //Añadimos tipo, si es puntero y el nombre del parametro.
                        
                    }
                    if (commandTemp.Parametros.Count>0) {s_metodo = s_metodo.Substring(0,s_metodo.Length-2);} //Quitamos última coma y espacio si se han escrito parametros.
                    s_metodo += ")"; //Cerramos enunciado de método.
                    file.WriteLine(s_comentario); //Escribimos comentario de versión.
                    file.WriteLine(s_metodo); //Escribimos enunciado de método en archivo.
                    file.WriteLine(tab+tab+"{"); //Abrimos metodo

                    //Ahora a escribir llamada.
                    string s_llamada = ""+tab+tab+tab + ((commandTemp.ReturnedType != "void") ? "return " : ""); //Definimos si retorna valor.
                    s_llamada += "internalGLES." + CommandsKeysList[key]+"("; //Iniciamos escritura de la llamada a metodo interno delegado.
                    foreach(string keyParam in commandTemp.Parametros.Keys) //Recorremos lista deparametros para añadirlos uno a uno.
                    {
                        s_llamada += keyParam + ", ";
                    }
                    if (commandTemp.Parametros.Count>0) {s_llamada = s_llamada.Substring(0,s_llamada.Length-2);} //Quitamos última coma y espacio si se han escrito parametros.
                    s_llamada += ");";
                    file.WriteLine(s_llamada); //Escribimos la llamada la metod interno delegado 
                    file.WriteLine(tab+tab+"}"); //Cerramos Método
                    file.WriteLine();
                }
            }

            file.WriteLine(tab+tab+"#endregion"); //Escribimos el último endregion.
            file.WriteLine();

            file.WriteLine(tab+"}"); //Cerramos clase 
            file.WriteLine("}"); //Cerramos namespace 
            file.WriteLine();
            file.Close(); //Cerramos Archivo.

            if (verbose) //Si Verbose mode mostramos la finalización del proceso.
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Generated File");
                Console.ResetColor(); 
                Console.WriteLine(": GLesMethods.cs");
            }
        }
    }
}