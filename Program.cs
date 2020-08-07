using System;

namespace OpenGLParser
{
    class MainClass
    {
        static bool verbose;
        static bool download;
        static bool downloaded;
        static bool ayuda;
        static string output;
        static string s_namespace;
        static int cursortop;
        static bool textoprocesado;
        public static void Main(string[] args)
        {
            Console.WriteLine("OpenGL Parser 3");
            output = "./output/";
            s_namespace = "OpenGL";
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch (arg)
                {
                    case "-v":
                        verbose = true;
                        break;
                    case "-V":
                        verbose = true;
                        break;
                    case "-d":
                        download = true;
                        break;
                    case "-D":
                        download = true;
                        break;
                    case "-o":
                        output = args[i + 1];
                        i++;
                        break;
                    case "-O":
                        output = args[i + 1];
                        i++;
                        break;
                    case "-n":
                        s_namespace = args[i + 1];
                        i++;
                        break;
                    case "-N":
                        s_namespace = args[i + 1];
                        i++;
                        break;
                    case "-h":
                        ayuda = true;
                        break;
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
                }        
            }

            //Parsear;
            glParser.Parse("./gl.xml", s_namespace, output, verbose);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("OpenGL Parser 3 Help Output:");
            Console.WriteLine("========================================================");
            Console.WriteLine("oglp3 [OPTION] <option value>");
            Console.WriteLine("Options:");
            Console.WriteLine("  -v  -> Verbose Mode.");
            Console.WriteLine("  -d  -> Download new gl.xml file.");
            Console.WriteLine("         If gl.xml file dont exist -d is by default.");
            Console.WriteLine("  -o  -> Output Path of .cs Files. (./output/ by default)");
            Console.WriteLine("  -n  -> NameSpace of .cs Files. (OpenGL by default)");
            Console.WriteLine("  -h  -> Show this Help. Ignore another options.");
        }

        private static void Download_glxml()
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                wc.Proxy = System.Net.WebRequest.GetSystemWebProxy();
                wc.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

                Uri uri = new Uri("https://www.khronos.org/registry/OpenGL/xml/gl.xml");
                
                Console.WriteLine();
                Console.WriteLine("Descargando gl.xml desde repositorio oficial.");
                cursortop = Console.CursorTop;
                textoprocesado = true;
                wc.DownloadFileAsync(uri, "./gl.xml.temp");
            }
        }

        static void Wc_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            if (textoprocesado) //Solo se procesa la escritura de texto si la anterior se ha terminado.
            {
                textoprocesado = false;
                string s_line = "Downloading: ▕";
                int con_width = Console.WindowWidth - (s_line.Length + 6);
                float i_variant = 100f / (float)(con_width);
                int value = e.ProgressPercentage;
                for (int i = 0; i < con_width; i++)
                {
                    string progreschar = " ";
                    if ((i_variant * i) <= value)
                    {
                        progreschar = "▆";
                    }
                    else
                    {
                        progreschar = " ";
                    }
                    s_line += progreschar;
                }
                s_line += "▏ " + value.ToString("D3") + "%";
                Console.SetCursorPosition(0, cursortop);
                Console.Write(s_line);
                textoprocesado = true;
            }
        }

        static void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
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
