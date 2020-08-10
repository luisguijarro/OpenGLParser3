using System;
using System.IO;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glWriter
    {
        private static void WriteEnums(string NameSpace, string outpath, bool verbose)
        {
            if (verbose) //Si Verbose mode mostramos inicio del proceso.
            {
                Console.WriteLine(); Console.WriteLine("Generating File: Enums.cs");
            }
            if (!Directory.Exists(outpath)) // Si la ruta no existe la creamos
            {
                Directory.CreateDirectory(outpath);
            }
            if (File.Exists(outpath + "Enums.cs")) //Si existe algun archivo previo lo eliminamos.
            {
                File.Delete(outpath + "Enums.cs");
            }
            StreamWriter file = File.CreateText(outpath + "Enums.cs"); //Generamos Contenido del archivo.
            file.WriteLine("// OpenGL Enumerators.");
            file.WriteLine("// File Created with OpenGL Parser 3.");
            file.WriteLine("// Developed by Luis Guijarro Pérez.");
            file.WriteLine();

            file.WriteLine("using System;");
            file.WriteLine();
            file.WriteLine("namespace " + NameSpace + ".OpenGL");
            file.WriteLine("{");

            string tab = "\t"; //Definimos tabulación.

            List<string> EnumKeysList = new List<string>(glReader.d_Enumerators.Keys); //Creamos lista de nombres de enumeradores para ordenar.
            EnumKeysList.Sort(); //Ordenamos lista alfabeticamente.
            for (int key = 0;key<EnumKeysList.Count;key++) //Recorremos la lista de Enumeradores
            {
                DataObjects.glEnum glenumerator = glReader.d_Enumerators[EnumKeysList[key]]; //Recogemos el Enumerador.
                file.WriteLine(tab+"enum " + EnumKeysList[key] + " : " + glenumerator.Tipo.ToString().Replace("System.", ""));  //Enumerador.
                file.WriteLine(tab+"{"); // Apertura de enumerador.

                List<string> keyList = new List<string>(glenumerator.EnumValues.Keys); //Obtenemos lista de keys del diccionario para ordenarlas alfabeticamente.
                keyList.Sort(); // Ordenamos alfabeticamente.

                for (int i=0;i<keyList.Count;i++) //Repasamos los valores y los escribimos.
                {
                    string numvalueLine = tab+tab+keyList[i]+ " = " + (glenumerator.EnumValues[keyList[i]].Value).ToString() + ","; //Confeccionamos la línea.
                    if (i==keyList.Count-1) { numvalueLine = numvalueLine.Substring(0, numvalueLine.Length-1); } //Elimina la coma del utlimo valor del Enum.
                    file.WriteLine(numvalueLine); // Escribimos la línea.
                }

                file.WriteLine(tab+"}"); // Cierre de Enumerador.
                file.WriteLine();
            }

            file.WriteLine("}");
            file.WriteLine();
            file.Close();

            if (verbose) //Si Verbose mode mostramos la finalización del proceso.
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Generated File");
                Console.ResetColor(); 
                Console.WriteLine(": Enums.cs");
            }
        }
    }
}