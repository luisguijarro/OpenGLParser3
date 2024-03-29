using System;
using System.IO;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glWriter
    {
        private static void WriteGlesDelegates(string NameSpace, string outpath, bool verbose)
        {
            if (verbose) //Si Verbose mode mostramos inicio del proceso.
            {
                Console.WriteLine(); Console.WriteLine("Generating File: GLesDelegates.cs");
            }
            if (!Directory.Exists(outpath)) // Si la ruta no existe la creamos
            {
                Directory.CreateDirectory(outpath);
            }
            if (File.Exists(outpath + "GLesDelegates.cs")) //Si existe algun archivo previo lo eliminamos.
            {
                File.Delete(outpath + "GLesDelegates.cs");
            }
            StreamWriter file = File.CreateText(outpath + "GLesDelegates.cs"); //Generamos Contenido del archivo.
            file.WriteLine("// OpenGL|ES Delegates.");
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

            file.WriteLine(tab+"internal static class delegatesGLES"); //Declaramos Clase Estatica contenedora de los métodos.
            file.WriteLine(tab+"{"); //Abrimos clase 



            List<string> CommandsKeysList = new List<string>(glReader.d_Commandos.Keys); //Creamos lista de nombres de comandos para ordenar.
            CommandsKeysList.Sort(); //Ordenamos lista alfabeticamente.


            char LastFirstLetter = ' '; // Creamos variable para recoger la ultima primera letra de metodo empleada.
            for (int key = 0;key<CommandsKeysList.Count;key++) //Recorremos la lista de Comandos
            {
                bool IsGles = false;
                foreach (glVersion vers in glReader.d_gles_versiones.Values)
                {
                    if (vers.Metodos.Contains(CommandsKeysList[key]))
                    {
                        IsGles = true;
                    }
                }
                foreach (glExtension ext in glReader.d_Gles_Extensions.Values)
                {
                    if (ext.Metodos.Contains(CommandsKeysList[key]))
                    {
                        IsGles = true;
                    }
                }

                if (!IsGles) { continue; } // Si no es de OpenGL|ES nos lo saltamos.
                
                //Definir Regiones Alfabeticas.
                DataObjects.glCommand commandTemp = glReader.d_Commandos[CommandsKeysList[key]]; //Recuperamos el comando.
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

                //Crear Delegados.
                string s_delegate = tab + tab + "internal" + (commandTemp.EsInseguro? " unsafe " : " ") + "delegate ";
                s_delegate += commandTemp.ReturnedType + (commandTemp.ReturnedTypePointer? "* " : " ") + CommandsKeysList[key] + "(";
                foreach(string keyParam in commandTemp.Parametros.Keys) //Recorremos lista deparametros para añadirlos uno a uno.
                {                    
                    glParam param = commandTemp.Parametros[keyParam]; //Obtenemos el parametro.
                    //s_delegate += param.tipo + (param.esArray? "[] ": " ") + keyParam + ", "; //Añadimos tipo, si es array y el nombre del parametro.
                    string s_ptn = "";
                    string s_tipo = param.tipo;
                    if (param.esPuntero>0) // Es punter?
                    {
                        if (s_tipo == "char")
                        {
                            if (param.Acces == AccesParam.In)
                            {
                                //s_tipo = "[MarshalAs(UnmanagedType.LPStr)] string" + ((param.esPuntero>1) ? "[]" : "");
                                s_tipo = "string" + ((param.esPuntero>1) ? "[]" : "");
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
                    s_delegate += s_tipo + s_ptn + " " + keyParam + ", "; //Añadimos tipo, si es puntero y numero de asteriscos y el nombre del parametro.

                    // s_delegate += param.tipo + ((param.esPuntero>0)? "* ": " ") + keyParam + ", "; //Añadimos tipo, si es array y el nombre del parametro.
                }
                if (commandTemp.Parametros.Count>0) {s_delegate = s_delegate.Substring(0,s_delegate.Length-2);} //Quitamos última coma y espacio si se han escrito parametros.
                s_delegate += ");"; //Cerramos enunciado de método.
                file.WriteLine(s_delegate); //Escribimos enunciado de método en archivo.
            }

            file.WriteLine(tab+tab+"#endregion"); //Escribimos el último endregion.
            file.WriteLine();

            file.WriteLine(tab+"}"); //Cerramos Clase
            file.WriteLine("}"); //Cerramos Espacio de Nombres
            file.WriteLine();
            file.Close(); //Cerramos Archivo.

            if (verbose) //Si Verbose mode mostramos la finalización del proceso.
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Generated File");
                Console.ResetColor(); 
                Console.WriteLine(": GLesDelegates.cs");
            }
        }
    }
}
