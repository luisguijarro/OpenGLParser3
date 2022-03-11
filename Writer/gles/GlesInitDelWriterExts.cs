using System;
using System.IO;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glWriter
    {
        private static void WriteDelegateInitializerGLesEXT(string NameSpace, string outpath, bool verbose)
        {
            if (verbose) //Si Verbose mode mostramos inicio del proceso.
            {
                Console.WriteLine(); Console.WriteLine("Generating File: OpenGLesInitDelsExts.cs");
            }
            if (!Directory.Exists(outpath)) // Si la ruta no existe la creamos
            {
                Directory.CreateDirectory(outpath);
            }
            if (File.Exists(outpath + "OpenGLesInitDelsExts.cs")) //Si existe algun archivo previo lo eliminamos.
            {
                File.Delete(outpath + "OpenGLesInitDelsExts.cs");
            }
            StreamWriter file = File.CreateText(outpath + "OpenGLesInitDelsExts.cs"); //Generamos Contenido del archivo.
            file.WriteLine("// OpenGL|ES Delegates Initializer for Extensions.");
            file.WriteLine("// File Created with OpenGL Parser 3.");
            file.WriteLine("// Developed by Luis Guijarro Pérez.");
            file.WriteLine();

            file.WriteLine("using System;");
            file.WriteLine("using System.Collections.Generic;");
            file.WriteLine("using System.Runtime.InteropServices;");
            file.WriteLine();
            file.WriteLine("namespace " + NameSpace + ".OpenGL");
            file.WriteLine("{");

            string tab = "\t"; //Definimos tabulación.

            file.WriteLine(tab+"internal static partial class DelegastesInitGLes"); //Declaramos Clase Estatica contenedora de los métodos.
            file.WriteLine(tab+"{"); //Abrimos clase 
            file.WriteLine(tab+tab+"private static List<string> SuportedExt;"); // Lista que contendrá los métodos de extensiones soportados.
            file.WriteLine();
            file.WriteLine(tab+tab+"internal static void InitdelegatesGLESExts()"); //Declaramos Metodo Estatico Iniciador de Delegados..
            file.WriteLine(tab+tab+"{"); //Abrimos Metodo 
            //file.WriteLine(tab+tab+tab+"InternalGLesTool.GetOS();"); //Lammamos a herramienta de definición del SO actual.

            #region Get Ext Supported

            file .WriteLine(tab+tab+tab+"SuportedExt = new List<string>();");
            file.WriteLine(tab+tab+tab+"int oglVer = int.Parse(oglGetString("+NameSpace+".OpenGL.StringName.GL_VERSION).Split('.')[0]);"); // Se comprueba Version de OpenGL.
            file.WriteLine(tab+tab+tab+"if (oglVer<3)"); // Si es menor a la versión 3.0
            file.WriteLine(tab+tab+tab+"{");
            file.WriteLine(tab+tab+tab+tab+"string[] oglExts = oglGetString("+NameSpace+".OpenGL.StringName.GL_EXTENSIONS).Split(' ');"); // Obtenemos Array de Extensiones.
            file.WriteLine(tab+tab+tab+tab+"SuportedExt.AddRange(oglExts);"); // Add to List
            file.WriteLine(tab+tab+tab+"}");
            file.WriteLine(tab+tab+tab+"else"); // En caso contrario es mayor o igual a 3.0
            file.WriteLine(tab+tab+tab+"{");
            file.WriteLine(tab+tab+tab+tab+"glGetIntegerv = (dglGetIntegerv)"+NameSpace+".OpenGL.InternalGLesTool.GetGLesMethodAdress(\"glGetIntegerv\", typeof(dglGetIntegerv));");
            file.WriteLine(tab+tab+tab+tab+"glGetStringi = (dglGetStringi)"+NameSpace+".OpenGL.InternalGLesTool.GetGLesMethodAdress(\"glGetStringi\", typeof(dglGetStringi));");
            file.WriteLine(tab+tab+tab+tab+"int Extnum = oglGetIntegerv("+NameSpace+".OpenGL.GetPName.GL_NUM_EXTENSIONS);"); // Obtenemos el número de Extensiones.
            file.WriteLine(tab+tab+tab+tab+"for (int i=0;i<Extnum;i++)");
            file.WriteLine(tab+tab+tab+tab+"{");
            file.WriteLine(tab+tab+tab+tab+tab+"string oglExts = oglGetStringi("+NameSpace+".OpenGL.StringName.GL_EXTENSIONS, i);"); // Obtenemos el número de Extensione.
            file.WriteLine(tab+tab+tab+tab+tab+"SuportedExt.Add(oglExts);"); // Add to List
            file.WriteLine(tab+tab+tab+tab+"}");
            file.WriteLine(tab+tab+tab+"}");

            #endregion

            List<string> CommandsKeysList = new List<string>(glReader.d_Commandos.Keys); //Creamos lista de nombres de comandos para ordenar.
            CommandsKeysList.Sort(); //Ordenamos lista alfabeticamente.


            char LastFirstLetter = ' '; // Creamos variable para recoger la ultima primera letra de metodo empleada.
            for (int key = 0;key<CommandsKeysList.Count;key++) //Recorremos la lista de Comandos
            {
                bool IsGles = false;
                foreach (glExtension vers in glReader.d_Gles_Extensions.Values)
                {
                    if (vers.Metodos.Contains(CommandsKeysList[key]))
                    {
                        IsGles = true;
                    }
                }

                if (!IsGles) { continue; } // Si no es de OpenGL|ES nos lo saltamos.

                //Definir Regiones Alfabeticas.
                DataObjects.glCommand commandTemp = glReader.d_Commandos[CommandsKeysList[key]]; //Recuperamos el comando.

                if (commandTemp.FromVersion.Length <= 0) //Solo escribimos los que NO estan en el Core de OpenGL.
                {
                    char ActualLetter = CommandsKeysList[key].Replace("gl", "").Substring(0,1).ToCharArray()[0];

                    if (ActualLetter != LastFirstLetter) //Si la nueva letra no es la ultima
                    {
                        if (LastFirstLetter != ' ') //Comprovamos que no es la primera
                        {
                            file.WriteLine(tab+tab+tab+"#endregion"); //Cerramos región
                            file.WriteLine();
                        }
                        LastFirstLetter = ActualLetter; //Establecemos nueva letra
                        file.WriteLine(tab+tab+tab+"#region "+LastFirstLetter.ToString().ToUpper()+":"); //Abrimos región
                        file.WriteLine();
                    }

                    file.WriteLine(tab+tab+tab+"if (SuportedExt.Contains(\"" + CommandsKeysList[key] + "\"))"); // Comprobar si está soportado.
                    file.WriteLine(tab+tab+tab+"{");

                    string s_initDel = tab + tab + tab + tab + NameSpace + ".OpenGL.internalGLES." + CommandsKeysList[key] + " = ";
                    s_initDel += "(" + NameSpace + ".OpenGL.delegatesGLES." + CommandsKeysList[key] + ") ";
                    s_initDel += "InternalGLesTool.GetGLesMethodAdress(\""+ CommandsKeysList[key] + "\", typeof("+ NameSpace + ".OpenGL.delegatesGLES." + CommandsKeysList[key] + "));";
                    file.WriteLine(s_initDel); //Escribimos iniciación de Metodo de OpenGL
                    //file.WriteLine();
                    file.WriteLine(tab+tab+tab+"}");
                }
            }

            file.WriteLine(tab+tab+tab+"#endregion"); //Escribimos el último endregion.
            file.WriteLine();

            file.WriteLine(tab+tab+"}"); //Cerramos Metodo 

            file.WriteLine(tab+tab+"[DllImport(\"opengl32.dll\")]");
            file.WriteLine(tab+tab+"private static extern IntPtr glGetString(StringName name);");
            file.WriteLine();
            file.WriteLine(tab+tab+"private static string oglGetString("+NameSpace+".OpenGL.StringName name)");
            file.WriteLine(tab+tab+"{");
            file.WriteLine(tab+tab+tab+"return Marshal.PtrToStringAnsi(glGetString(name));");
            file.WriteLine(tab+tab+"}");
            file.WriteLine();

            //file.WriteLine(tab+tab+"[DllImport(\"opengl32.dll\")]");
            file.WriteLine(tab+tab+"private unsafe delegate void dglGetIntegerv("+NameSpace+".OpenGL.GetPName pname, out int* @params);");
            file.WriteLine(tab+tab+"private static dglGetIntegerv glGetIntegerv;");
            file.WriteLine();
            file.WriteLine(tab+tab+"private static unsafe int oglGetIntegerv("+NameSpace+".OpenGL.GetPName pname)");
            file.WriteLine(tab+tab+"{");
            file.WriteLine(tab+tab+tab+"int* pdata;");
            file.WriteLine(tab+tab+tab+"glGetIntegerv(pname, out pdata);");
            file.WriteLine(tab+tab+tab+"return (int)pdata;");
            file.WriteLine(tab+tab+"}");
            file.WriteLine();

            file.WriteLine(tab+tab+"private delegate IntPtr dglGetStringi("+NameSpace+".OpenGL.StringName name, int num);");
            file.WriteLine(tab+tab+"private static dglGetStringi glGetStringi;");
            file.WriteLine();
            file.WriteLine(tab+tab+"private static string oglGetStringi("+NameSpace+".OpenGL.StringName name, int num)");
            file.WriteLine(tab+tab+"{");
            file.WriteLine(tab+tab+tab+"return Marshal.PtrToStringAnsi(glGetStringi(name, num));");
            file.WriteLine(tab+tab+"}");
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
                Console.WriteLine(": OpenGLesInitDelsExts.cs");
            }
        }
    }
}

