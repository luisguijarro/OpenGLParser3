using System;
using System.IO;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glWriter
    {
        private static void WriteInternalGLesTools(string NameSpace, string outpath, bool verbose)
        {
            if (verbose) //Si Verbose mode mostramos inicio del proceso.
            {
                Console.WriteLine(); Console.WriteLine("Generating File: GLInternalGLesTool.cs");
            }
            if (!Directory.Exists(outpath)) // Si la ruta no existe la creamos
            {
                Directory.CreateDirectory(outpath);
            }
            if (File.Exists(outpath + "GLInternalGLesTool.cs")) //Si existe algun archivo previo lo eliminamos.
            {
                File.Delete(outpath + "GLInternalGLesTool.cs");
            }
            StreamWriter file = File.CreateText(outpath + "GLInternalGLesTool.cs"); //Generamos Contenido del archivo.
            file.WriteLine("// OpenGL|ES Internal Tool.");
            file.WriteLine("// File Created with OpenGL Parser 3.");
            file.WriteLine("// Developed by Luis Guijarro Pérez.");
            file.WriteLine();

            file.WriteLine("using System;");
            file.WriteLine("using System.Runtime.InteropServices;");
            file.WriteLine();
            file.WriteLine("namespace " + NameSpace + ".OpenGL");
            file.WriteLine("{");

            string tab = "\t"; //Definimos tabulación.

            file.WriteLine(tab + "internal static class InternalGLesTool"); //Declaramos Clase Estatica contenedora de los métodos.
            file.WriteLine(tab + "{"); //Abrimos clase 

            file.WriteLine(tab + tab + "internal static IntPtr lib;");
            file.WriteLine(tab + tab + "internal static Delegate GetGLesMethodAdress(String MethodName, Type type_origen)");
            file.WriteLine(tab + tab + "{"); //Abrimos Método 

            file.WriteLine(tab + tab + tab + "IntPtr p_ret = IntPtr.Zero;");
            file.WriteLine(tab + tab + tab + "p_ret = eglGetProcAddress(MethodName);");
            file.WriteLine(tab + tab + tab + "if (p_ret != IntPtr.Zero)");
            file.WriteLine(tab + tab + tab + "{");
            file.WriteLine(tab + tab + tab + tab + "try");
            file.WriteLine(tab + tab + tab + tab + "{");
            file.WriteLine(tab + tab + tab + tab + tab + "return Marshal.GetDelegateForFunctionPointer(p_ret, type_origen);");
            file.WriteLine(tab + tab + tab + tab + "}");
            file.WriteLine(tab + tab + tab + tab + "catch");
            file.WriteLine(tab + tab + tab + tab + "{");
            file.WriteLine(tab + tab + tab + tab + tab + "#if DEBUG");
            file.WriteLine(tab + tab + tab + tab + tab + "Console.WriteLine(type_origen.ToString());");
            file.WriteLine(tab + tab + tab + tab + tab + "#endif");
            file.WriteLine(tab + tab + tab + tab + tab + "return null;");
            file.WriteLine(tab + tab + tab + tab + "}");
            file.WriteLine(tab + tab + tab + "}");
            file.WriteLine(tab + tab + tab + "else");
            file.WriteLine(tab + tab + tab + "{");
            file.WriteLine(tab + tab + tab + tab + "return null;");
            file.WriteLine(tab + tab + tab + "}");

            file.WriteLine(tab + tab + "}"); //Cerramos Método
            file.WriteLine();

            #region Imports

            file.WriteLine(tab + tab + "[DllImport(\"libEGL.so.1\", EntryPoint = \"eglGetProcAddress\")]");
		    file.WriteLine(tab + tab + "internal extern static IntPtr eglGetProcAddress(String MethodName);");
            file.WriteLine();

            file.WriteLine(tab + tab + "[DllImport(\"libX11\", EntryPoint = \"XOpenDisplay\")]");
            file.WriteLine(tab + tab + "internal extern static IntPtr XOpenDisplay(IntPtr display);");
            file.WriteLine();

            file.WriteLine(tab + tab + "[DllImport(\"libX11\", EntryPoint = \"XCloseDisplay\")]");
            file.WriteLine(tab + tab + "internal extern static void XCloseDisplay(IntPtr display);");
            file.WriteLine();
            	
            #endregion

            file.WriteLine(tab + "}"); //Cerramos Clase
            file.WriteLine("}"); //Cerramos Espacio de Nombres
            file.WriteLine();
            file.Close(); //Cerramos Archivo.

            if (verbose) //Si Verbose mode mostramos la finalización del proceso.
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Generated File");
                Console.ResetColor(); 
                Console.WriteLine(": GLInternalGLesTool.cs");
            }
        }
    }
}


