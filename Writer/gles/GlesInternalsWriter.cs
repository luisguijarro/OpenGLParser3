using System;
using System.IO;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glWriter
    {
        private static void WriteGLesInternals(string NameSpace, string outpath, bool verbose)
        {
            if (verbose) //Si Verbose mode mostramos inicio del proceso.
            {
                Console.WriteLine(); Console.WriteLine("Generating File: GLesInternals.cs");
            }
            if (!Directory.Exists(outpath)) // Si la ruta no existe la creamos
            {
                Directory.CreateDirectory(outpath);
            }
            if (File.Exists(outpath + "GLesInternals.cs")) //Si existe algun archivo previo lo eliminamos.
            {
                File.Delete(outpath + "GLesInternals.cs");
            }
            StreamWriter file = File.CreateText(outpath + "GLesInternals.cs"); //Generamos Contenido del archivo.
            file.WriteLine("// OpenGL|ES Internals.");
            file.WriteLine("// File Created with OpenGL Parser 3.");
            file.WriteLine("// Developed by Luis Guijarro Pérez.");
            file.WriteLine();

            file.WriteLine("using System;");
            file.WriteLine();
            file.WriteLine("namespace " + NameSpace + ".OpenGL");
            file.WriteLine("{");

            string tab = "\t"; //Definimos tabulación.

            file.WriteLine(tab+"internal static class internalGLES"); //Declaramos Clase Estatica contenedora de los métodos.
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

                file.WriteLine(tab + tab + "internal static delegatesGLES." + CommandsKeysList[key] + " " + CommandsKeysList[key] + ";");
                file.WriteLine();
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
                Console.WriteLine(": GLesInternals.cs");
            }
        }
    }
}

