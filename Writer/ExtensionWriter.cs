using System;
using System.IO;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glWriter
    {
        private static void WriteEXTCommands(string NameSpace, string outpath, bool verbose)
        {
            if (verbose) //Si Verbose mode mostramos inicio del proceso.
            {
                Console.WriteLine(); Console.WriteLine("Generating File: ExtMethods.cs");
            }
            if (!Directory.Exists(outpath)) // Si la ruta no existe la creamos
            {
                Directory.CreateDirectory(outpath);
            }
            if (File.Exists(outpath + "ExtMethods.cs")) //Si existe algun archivo previo lo eliminamos.
            {
                File.Delete(outpath + "ExtMethods.cs");
            }
            StreamWriter file = File.CreateText(outpath + "ExtMethods.cs"); //Generamos Contenido del archivo.
            file.WriteLine("// OpenGL Extension's Methods.");
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

            file.WriteLine(tab+"public static partial class GL"); //Declaramos Clase Estatica contenedora de los métodos.
            file.WriteLine(tab+"{"); //Abrimos clase 

            List<string> ExtensionsKeysList = new List<string>(glReader.d_Extensions.Keys); //Creamos lista de nombres de las Extensiones para ordenar.
            ExtensionsKeysList.Sort();
            for (int ext=0;ext<ExtensionsKeysList.Count;ext++)
            {
                file.WriteLine(tab+tab+"public static class " + ExtensionsKeysList[ext]); //Declaramos Clase Estatica contenedora de los métodos.
                file.WriteLine(tab+tab+"{"); //Abrimos clase EXT

                glReader.d_Extensions[ExtensionsKeysList[ext]].Metodos.Sort(); //Ordenamos lista alfabeticamente.

                for (int i=0;i<glReader.d_Extensions[ExtensionsKeysList[ext]].Metodos.Count;i++)
                {
                    string key = glReader.d_Extensions[ExtensionsKeysList[ext]].Metodos[i];

                    if (glReader.d_Commandos.ContainsKey(key))
                    {
                        DataObjects.glCommand commandTemp = glReader.d_Commandos[key]; //Recuperamos el comando.

                        //Ahora Escribir Método.
                        string s_metodo = tab+tab+tab+"public static "; //Iniciamos escritura del método.
                        s_metodo += commandTemp.EsInseguro ? "unsafe " : "";
                        s_metodo += commandTemp.ReturnedType + (commandTemp.ReturnedTypePointer? "* " : " ") + key + "(";
                        foreach(string keyParam in commandTemp.Parametros.Keys) //Recorremos lista deparametros para añadirlos uno a uno.
                        {                    
                            glParam param = commandTemp.Parametros[keyParam]; //Obtenemos el parametro.
                            //s_metodo += param.tipo + (param.esArray? "[] ": " ") + keyParam + ", "; //Añadimos tipo, si es array y el nombre del parametro.
                            string s_ptn = "";
                            string s_tipo = param.tipo;
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
                            s_metodo += s_tipo + s_ptn + " " + keyParam + ", ";
                            
                            //s_metodo += param.tipo + (param.esPuntero? "* ": " ") + keyParam + ", "; //Añadimos tipo, si es puntero y el nombre del parametro.
                        }
                        
                        if (commandTemp.Parametros.Count>0) {s_metodo = s_metodo.Substring(0,s_metodo.Length-2);} //Quitamos última coma y espacio si se han escrito parametros.
                        s_metodo += ")"; //Cerramos enunciado de método.
                        
                        file.WriteLine(s_metodo); //Escribimos enunciado de método en archivo.
                        file.WriteLine(tab+tab+tab+"{"); //Abrimos metodo

                        //Ahora a escribir llamada.
                        string s_llamada = ""+tab+tab+tab+tab + ((commandTemp.ReturnedType != "void") ? "return " : ""); //Definimos si retorna valor.
                        s_llamada += "internalGL."+key+"("; //Iniciamos escritura de la llamada a metodo interno delegado.
                        foreach(string keyParam in commandTemp.Parametros.Keys) //Recorremos lista deparametros para añadirlos uno a uno.
                        {
                            s_llamada += keyParam + ", ";
                        }
                        if (commandTemp.Parametros.Count>0) {s_llamada = s_llamada.Substring(0,s_llamada.Length-2);} //Quitamos última coma y espacio si se han escrito parametros.
                        s_llamada += ");";
                        file.WriteLine(s_llamada); //Escribimos la llamada la metod interno delegado 
                        file.WriteLine(tab+tab+tab+"}"); //Cerramos Método
                        if (i<glReader.d_Extensions[ExtensionsKeysList[ext]].Metodos.Count-1) {file.WriteLine(); }
                    }
                }

                file.WriteLine(tab+tab+"}"); //Cerramos Clase EXT
                if (ext<ExtensionsKeysList.Count-1) { file.WriteLine(); }
            }            

            file.WriteLine(tab+"}"); //Cerramos Clase GL
            file.WriteLine("}"); //Cerramos Espacio de Nombres
            file.WriteLine();
            file.Close(); //Cerramos Archivo.

            if (verbose) //Si Verbose mode mostramos la finalización del proceso.
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Generated File");
                Console.ResetColor(); 
                Console.WriteLine(": ExtMethods.cs");
            }
        }
    }
}
