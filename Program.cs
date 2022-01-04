using System;
using System.Net;
using System.ComponentModel;

namespace OpenGLParser
{
    class MainClass
    {
        static bool verbose;
        static bool download;
        static bool downloaded;
        static bool gitRefPages;
        static bool withgles;
        static bool ayuda;
        static string output;
        static string s_namespace;
        static int cursortop;
        static bool textoprocesado;
        public static void Main(string[] args)
        {
            Console.Clear();
            output = "./output/";
            s_namespace = "OpenGL";
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch (arg)
                {
                    case "-v":
                    case "-V":
                        verbose = true;
                        break;
                    case "-d":
                    case "-D":
                        download = true;
                        break;
                    case "-g":
                    case "-G":
                        gitRefPages = true;
                        break;
                    case "-es":
                    case "-Es":
                    case "-eS":
                    case "-ES":
                        withgles = true;
                        break;
                    case "-o":
                    case "-O":
                        output = args[i + 1];
                        i++;
                        break;
                    case "-n":
                    case "-N":
                        s_namespace = args[i + 1];
                        i++;
                        break;
                    case "-h":
                    case "--help":
                        ayuda = true;
                        break;
                }
                if (ayuda)
                {
                    ShowHelp(); //Muestra la ayuda
                    return; //Finaliza la aplicación
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("OpenGL Parser 3");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==========================================================");
            Console.ResetColor();

            if (download)
            {
                if (System.IO.File.Exists("./gl.xml"))
                {
                    Console.WriteLine("Ya tiene descargada una versión del archivo gl.xml");
                    Console.Write("¿Desea realmente descargarlo nuevamente?(s/N):");
                    
                    if (Console.ReadKey().KeyChar == 's')
                    {
                        Console.WriteLine();
                        Download_glxml();
                    }
                    else
                    {
                        downloaded = true;
                    }
                    Console.WriteLine();
                }
                else
                {
                    Download_glxml();
                }      
                while(!downloaded)
                {
                    //Esperar a que termine la descarga.
                    System.Threading.Thread.Sleep(100); //Aligerar consumo de recursos.
                }        
            }

            //Parsear;
            glParser.Parse("./gl.xml", s_namespace, output.Length > 0 ? output+"/" : output, verbose, gitRefPages, withgles);
            Console.WriteLine();
        }

        private static void ShowHelp()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("OpenGL Parser 3 Help Output:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==========================================================");
            Console.ResetColor();
            Console.WriteLine("oglp3 [OPTION] <option value>");
            Console.WriteLine("Options:");
            Console.WriteLine("  -v  -> Verbose Mode.");
            Console.WriteLine("  -d  -> Download new gl.xml file.");
            Console.WriteLine("         If gl.xml file dont exist -d is by default.");
            Console.WriteLine("  -o  -> Output Path of .cs Files. (./output/ by default)");
            Console.WriteLine("  -n  -> NameSpace of .cs Files. (OpenGL by default)");
            Console.WriteLine("  -g  -> Complete Enums with OpenGL-RefPages. requires git.");
            Console.WriteLine("  -es -> Parse with OpenGL|ES");
            Console.WriteLine("  -h  -> Show this Help. Ignore another options.");
        }

        private static void Download_glxml()
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Wc_DownloadProgressChanged);
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(Wc_DownloadFileCompleted);
                wc.BaseAddress = "";
                wc.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
                wc.Proxy = System.Net.WebRequest.GetSystemWebProxy();
                wc.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

                Uri uri = new Uri("https://www.khronos.org/registry/OpenGL/xml/gl.xml");
                
                Console.WriteLine();
                Console.WriteLine("Descargando gl.xml desde repositorio oficial.");
                cursortop = Console.CursorTop;
                textoprocesado = true;
                try
                {
                    wc.DownloadFileAsync(uri, "./gl.xml.temp");
                }
                catch (Exception e)
                {
                    Console.WriteLine("EXCEPTION: "+e.Message);
                }
            }
        }

        static void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (textoprocesado) //Solo se procesa la escritura de texto si la anterior se ha terminado.
            {
                textoprocesado = false;
                string s_line = "Downloading: ▕";
                Console.SetCursorPosition(0, cursortop);
                Console.Write(s_line);
                Console.ForegroundColor = ConsoleColor.Green;
                int con_width = Console.WindowWidth - (s_line.Length + 7);
                float i_variant = 100f / (float)(con_width);
                int value = e.ProgressPercentage;
                s_line = "";
                for (int i = 0; i < con_width; i++)
                {
                    string progreschar = " ";
                    if ((i_variant * i) <= value)
                    {
                        progreschar = "▆";
                    }
                    else
                    {
                        Console.ResetColor();
                        progreschar = "_";
                    }
                    Console.Write(progreschar);
                }
                Console.Write(s_line);
                Console.ResetColor();
                s_line = "▏ ";
                Console.Write(s_line);
                s_line =  value.ToString("D3") + "%";
                //Console.SetCursorPosition(0, cursortop);
                Console.Write(s_line);
                textoprocesado = true;
            }
        }

        static void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Console.WriteLine();
                Console.WriteLine("Descarga de gl.xml Cancelada");
            }
            if (e.Error != null)
            {
                Console.WriteLine();
                Console.WriteLine("Error en descarga de gl.xml");
                return;
            }
            if ((e.Error == null) && (!e.Cancelled))
            {
                System.IO.FileStream fs = System.IO.File.Open("./gl.xml.temp", System.IO.FileMode.Open);
                if (fs.Length > 0)
                {
                    if (System.IO.File.Exists("./gl.xml"))
                    {
                        System.IO.File.Replace("./gl.xml.temp", "./gl.xml", "./gl.xml.old");
                    }
                    else
                    {
                        System.IO.File.Move("./gl.xml.temp", "./gl.xml");
                    }

                    if (System.IO.File.Exists("./gl.xml.temp"))
                    {
                        System.IO.File.Delete("./gl.xml.temp");
                    }
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Descarga de gl.xml Realizada con Exito");
                    fs.Close();
                    downloaded = true;
                }
                else
                {
                    System.IO.File.Delete("./gl.xml.temp");
                    Console.WriteLine();
                    Console.WriteLine(e.Error.Message, "Error en descarga de gl.xml"); 
                    fs.Close();
                }
            }
        }
    }
}
