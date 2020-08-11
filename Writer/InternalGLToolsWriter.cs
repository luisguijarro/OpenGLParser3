using System;
using System.IO;
using System.Collections.Generic;

using OpenGLParser.DataObjects;

namespace OpenGLParser
{
    public static partial class glWriter
    {
        private static void WriteInternalTools(string NameSpace, string outpath, bool verbose)
        {
            if (verbose) //Si Verbose mode mostramos inicio del proceso.
            {
                Console.WriteLine(); Console.WriteLine("Generating File: GLInternalTool.cs");
            }
            if (!Directory.Exists(outpath)) // Si la ruta no existe la creamos
            {
                Directory.CreateDirectory(outpath);
            }
            if (File.Exists(outpath + "GLInternalTool.cs")) //Si existe algun archivo previo lo eliminamos.
            {
                File.Delete(outpath + "GLInternalTool.cs");
            }
            StreamWriter file = File.CreateText(outpath + "GLInternalTool.cs"); //Generamos Contenido del archivo.
            file.WriteLine("// OpenGL Internal Tool.");
            file.WriteLine("// File Created with OpenGL Parser 3.");
            file.WriteLine("// Developed by Luis Guijarro Pérez.");
            file.WriteLine();

            file.WriteLine("using System;");
            file.WriteLine("using System.Runtime.InteropServices;");
            file.WriteLine();
            file.WriteLine("namespace " + NameSpace + ".OpenGL");
            file.WriteLine("{");

            string tab = "\t"; //Definimos tabulación.

            file.WriteLine(tab + "internal static class InternalTool"); //Declaramos Clase Estatica contenedora de los métodos.
            file.WriteLine(tab + "{"); //Abrimos clase 

            file.WriteLine(tab + tab + "internal static IntPtr lib;");
            file.WriteLine(tab + tab + "internal static Delegate GetGLMethodAdress(String MethodName, Type type_origen)");
            file.WriteLine(tab + tab + "{"); //Abrimos Método 

            file.WriteLine(tab + tab + tab + "IntPtr p_ret = IntPtr.Zero;");
            file.WriteLine(tab + tab + tab + "switch(OS)");
            file.WriteLine(tab + tab + tab + "{");
            file.WriteLine(tab + tab + tab + tab + "case OperatingSystem.WindowsVistaOrHigher:");
            file.WriteLine(tab + tab + tab + tab + "case OperatingSystem.Windows:");
            file.WriteLine(tab + tab + tab + tab + tab + "p_ret = wglGetProcAddress(MethodName);");
            file.WriteLine(tab + tab + tab + tab + tab + "if (p_ret == IntPtr.Zero)");
            file.WriteLine(tab + tab + tab + tab + tab + "{");
            file.WriteLine(tab + tab + tab + tab + tab + tab + "p_ret = GetProcAddress(lib, MethodName);");
            file.WriteLine(tab + tab + tab + tab + tab + "}");
            file.WriteLine(tab + tab + tab + tab + tab + "break;");
            file.WriteLine(tab + tab + tab + tab + "case OperatingSystem.Linux_X11:");
            file.WriteLine(tab + tab + tab + tab + tab + "p_ret = glXGetProcAddress(MethodName);");
            file.WriteLine(tab + tab + tab + tab + tab + "break;");
            file.WriteLine(tab + tab + tab + tab + "default:");
            file.WriteLine(tab + tab + tab + tab + tab + "p_ret = IntPtr.Zero;");
            file.WriteLine(tab + tab + tab + tab + tab + "break;");
            file.WriteLine(tab + tab + tab + "}");
            file.WriteLine();
            file.WriteLine(tab + tab + tab + "if (p_ret != IntPtr.Zero)");
            file.WriteLine(tab + tab + tab + "{");
            file.WriteLine(tab + tab + tab + tab + "return Marshal.GetDelegateForFunctionPointer(p_ret, type_origen);");
            file.WriteLine(tab + tab + tab + "}");
            file.WriteLine(tab + tab + tab + "else");
            file.WriteLine(tab + tab + tab + "{");
            file.WriteLine(tab + tab + tab + tab + "return null;");
            file.WriteLine(tab + tab + tab + "}");

            file.WriteLine(tab + tab + "}"); //Cerramos Método
            file.WriteLine();

            #region Imports

            file.WriteLine(tab + tab + "[System.Security.SuppressUnmanagedCodeSecurity]");
    	    file.WriteLine(tab + tab + "[DllImport(\"kernel32.dll\", SetLastError=true)] ");
    	    file.WriteLine(tab + tab + "internal static extern IntPtr GetProcAddress(IntPtr library, string Name);");
            file.WriteLine();

            file.WriteLine(tab + tab + "[DllImport(\"opengl32.dll\", EntryPoint = \"wglGetProcAddress\", SetLastError = true)]");
		    file.WriteLine(tab + tab + "internal extern static IntPtr wglGetProcAddress(String lpszProc);");
            file.WriteLine();

            file.WriteLine(tab + tab + "[DllImport(\"libGL.so.1\", EntryPoint = \"glXGetProcAddress\")]");
		    file.WriteLine(tab + tab + "internal extern static IntPtr glXGetProcAddress(String MethodName);");
            file.WriteLine();

            file.WriteLine(tab + tab + "[DllImport(\"libX11\", EntryPoint = \"XOpenDisplay\")]");
            file.WriteLine(tab + tab + "internal extern static IntPtr XOpenDisplay(IntPtr display);");
            file.WriteLine();

            file.WriteLine(tab + tab + "[DllImport(\"libX11\", EntryPoint = \"XCloseDisplay\")]");
            file.WriteLine(tab + tab + "internal extern static void XCloseDisplay(IntPtr display);");
            file.WriteLine();
            	
		    file.WriteLine(tab + tab + "[DllImport(\"kernel32.dll\", SetLastError = true)]");
            file.WriteLine(tab + tab + "internal static extern IntPtr LoadLibrary(string dllName);");
            file.WriteLine();

            #endregion

            #region OSDetector

            file.WriteLine(tab + tab + "internal static OperatingSystem OS;");

            file.WriteLine(tab + tab + "internal static void GetOS()");
            file.WriteLine(tab + tab + "{");
            file.WriteLine(tab + tab + tab + "if (!isX11())");
			file.WriteLine(tab + tab + tab + "{");
			file.WriteLine(tab + tab + tab + tab + "if (!isWindows())");
			file.WriteLine(tab + tab + tab + tab + "{");
			file.WriteLine(tab + tab + tab + tab + tab + "OS = OperatingSystem.NotSuported;");
			file.WriteLine(tab + tab + tab + tab + "}");
			file.WriteLine(tab + tab + tab + tab + "else");
			file.WriteLine(tab + tab + tab + tab + "{");
			file.WriteLine(tab + tab + tab + tab + tab + "lib = LoadLibrary(\"opengl32.dll\");");
			file.WriteLine(tab + tab + tab + tab + "}");
			file.WriteLine(tab + tab + tab + "}");
            file.WriteLine(tab + tab + "}");
            file.WriteLine();

            file.WriteLine(tab + tab + "internal static bool isX11()");
            file.WriteLine(tab + tab + "{");
            file.WriteLine(tab + tab + tab + "try");
            file.WriteLine(tab + tab + tab + "{");
            file.WriteLine(tab + tab + tab + tab + "IntPtr displaytemp;");
            file.WriteLine(tab + tab + tab + tab + "if ((displaytemp = XOpenDisplay(IntPtr.Zero)) != IntPtr.Zero)");
            file.WriteLine(tab + tab + tab + tab + "{");
            file.WriteLine(tab + tab + tab + tab + tab + "XCloseDisplay(displaytemp);");
            file.WriteLine(tab + tab + tab + tab + tab + "OS = OperatingSystem.Linux_X11;");
            file.WriteLine(tab + tab + tab + tab + tab + "return true;");
            file.WriteLine(tab + tab + tab + tab + "}");
            file.WriteLine(tab + tab + tab + tab + "return false;");
            file.WriteLine(tab + tab + tab + "}");
            file.WriteLine(tab + tab + tab + "catch { return false; }");
            file.WriteLine(tab + tab + "}");
            file.WriteLine();

            file.WriteLine(tab + tab + "internal static bool isWindows()");
            file.WriteLine(tab + tab + "{");
            file.WriteLine(tab + tab + tab + "switch(Environment.OSVersion.Platform)");
            file.WriteLine(tab + tab + tab + "{");
            file.WriteLine(tab + tab + tab + tab + "case PlatformID.Win32NT:");
            file.WriteLine(tab + tab + tab + tab + tab + "if (Environment.OSVersion.Version.Major >= 6)");
            file.WriteLine(tab + tab + tab + tab + tab + "{");
            file.WriteLine(tab + tab + tab + tab + tab + tab + "OS = OperatingSystem.WindowsVistaOrHigher;");
            file.WriteLine(tab + tab + tab + tab + tab + tab + "return true;");
            file.WriteLine(tab + tab + tab + tab + tab + "}");
            file.WriteLine(tab + tab + tab + tab + tab + "OS = OperatingSystem.Windows;");
            file.WriteLine(tab + tab + tab + tab + tab + "return true;");
            file.WriteLine(tab + tab + tab + tab + "case PlatformID.Win32S:");
            file.WriteLine(tab + tab + tab + tab + "case PlatformID.Win32Windows:");
            file.WriteLine(tab + tab + tab + tab + "case PlatformID.WinCE:");
            file.WriteLine(tab + tab + tab + tab + tab + "OS = OperatingSystem.Windows;");
            file.WriteLine(tab + tab + tab + tab + tab + "return true;");
            file.WriteLine(tab + tab + tab + "}");
            file.WriteLine(tab + tab + tab + "return false;");
            file.WriteLine(tab + tab + "}");
            file.WriteLine();

	        file.WriteLine(tab + tab + "internal enum OperatingSystem");
            file.WriteLine(tab + tab + "{");
            file.WriteLine(tab + tab + tab + "Windows, WindowsVistaOrHigher, Linux_X11, Linux_Wayland, NotSuported");
            file.WriteLine(tab + tab + "}");

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
                Console.WriteLine(": GLInternalTool.cs");
            }
        }
    }
}


