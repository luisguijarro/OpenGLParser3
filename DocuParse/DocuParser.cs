using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace OpenGLParser
{
    ///<sumary>
    ///Parsear documentación para completar Enumeradores.
    ///</sumary>
    public static partial class DocuParser
    {
        public static Dictionary<string, Dictionary<string, List<string>>> d_MethodParamValues;
        public static void CloneFromGit()
        {
            Console.WriteLine();
            Console.WriteLine("Git operations for OpenGL-Refpages repository");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==========================================================");
            Console.ResetColor();
            if (Directory.Exists("./OpenGL-Refpages"))
            {
                Console.WriteLine("FETCHING:");
                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory()+"/OpenGL-Refpages");
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/git",
                        Arguments = "fetch origin master",
                        UseShellExecute = false, 
                        RedirectStandardOutput = true,
                        CreateNoWindow = true //No cargar ventana de shell.
                    }
                };
                proc.Start(); //Lanzar proceso
                proc.WaitForExit(); //Esperar a que termine.
                proc.Close();

                Console.WriteLine("PULLING:");
                proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/git",
                        Arguments = "pull origin master",
                        UseShellExecute = false, 
                        RedirectStandardOutput = true,
                        CreateNoWindow = true //No cargar ventana de shell.
                    }
                };
                proc.Start(); //Lanzar proceso
                proc.WaitForExit(); //Esperar a que termine.
                proc.Close();
                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory().Replace("/OpenGL-Refpages", ""));
            }
            else
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/git",
                        Arguments = "clone https://github.com/KhronosGroup/OpenGL-Refpages.git",
                        UseShellExecute = false, 
                        RedirectStandardOutput = true,
                        CreateNoWindow = true //No cargar ventana de shell.
                    }
                };
                proc.Start(); //Lanzar proceso
                proc.WaitForExit(); //Esperar a que termine.
                proc.Close();
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==========================================================");
            Console.ResetColor();
            Console.WriteLine();            
        }

        ///<sumary>
        ///Parsear documentación OpenGL 2.1
        ///</sumary>
        public static bool Parse21() //Devuelve False si hay algún fallo.
        {
            return Parse("./OpenGL-Refpages/gl2.1/");
        }

        ///<sumary>
        ///Parsear documentación OpenGL 4
        ///</sumary>
        public static bool Parse4() //Devuelve False si hay algún fallo.
        {
            return Parse("./OpenGL-Refpages/gl4/");
        }

        public static bool Parse(string s_Path) //Devuelve False si hay algún fallo.
        {
            if (d_MethodParamValues == null) { d_MethodParamValues = new Dictionary<string, Dictionary<string, List<string>>>();}
            if (!Directory.Exists(s_Path))
            {
                Console.WriteLine("OpenGL-Refpages repository is not cloned");
                return false;
            }
            string[] Filenames = Directory.GetFiles(s_Path, "gl*.xml"); //Obtener ficheros xml.

            foreach (string filename in Filenames)
            {
                string s_file = File.ReadAllText(filename);
                s_file = s_file.Replace("&",""); // Prevenir Reference to undeclared entity Exception.
                
                var sr = new MemoryStream();
                var writer = new StreamWriter(sr);
                writer.Write(s_file);
                writer.Flush();
                sr.Position = 0;

                #if DEBUG
                Console.WriteLine("======================================================================================");
                #endif

                XmlDocument xdoc = new XmlDocument();
                
                XmlReaderSettings settings = new XmlReaderSettings 
                { 
                    NameTable = new NameTable(),
                    DtdProcessing = DtdProcessing.Ignore,
                    ValidationType = ValidationType.None
                };
                
                XmlNamespaceManager manager = new XmlNamespaceManager(settings.NameTable);                
                manager.AddNamespace("mml", "http://www.w3.org/2001/XMLSchema-instance"); // Prevenir undeclared prefix exception.           
                manager.AddNamespace("xlink", "http://www.w3.org/2001/XMLSchema-instance"); // Prevenir undeclared prefix exception.
                XmlParserContext context = new XmlParserContext(null, manager, "", XmlSpace.Default);                
                XmlReader reader = XmlReader.Create(sr, settings, context);                

                #if DEBUG
                Console.WriteLine("Filename: "+filename);
                #endif
                try
                {
                    xdoc.Load(reader);

                    //CODIGODE PARSEO.
                    
                    List<string> GenumParams = new List<string>();

                    XmlNodeList xml_Grps_Methods = xdoc.SelectNodes("refentry/refsynopsisdiv"); // Grupos de Metodos
                    List<Dictionary<string, List<string>>> GrupoMetodos = new List<Dictionary<string, List<string>>>();
                    
                    for (int g=0;g<xml_Grps_Methods.Count;g++)
                    {
                        Dictionary<string, List<string>> d_methodsParams = new Dictionary<string, List<string>>(); //nombre mtodo / Lista de nombres de parametros.
                    
                        XmlNodeList xml_mthds = xml_Grps_Methods[g].SelectNodes("funcsynopsis/funcprototype");
                        for (int m=0;m<xml_mthds.Count;m++)
                        {
                            string methodName = xml_mthds[m].SelectSingleNode("funcdef/function").InnerText; // Obtenemos nombre del método.
                            #if DEBUG
                            Console.WriteLine("Method: " + methodName);
                            #endif
                            XmlNodeList xml_paramNames = xml_mthds[m].SelectNodes("paramdef"); // Obtener nombres de parametros del método.
                            List<string> paramNames = new List<string>();
                            for (int p=0;p<xml_paramNames.Count;p++) // Recorremos parametros.
                            {
                                //if (xml_paramNames [p].InnerText.Contains("GLenum")) // Buscamos solo los parametros enumerados.
                                //{
                                    //Console.WriteLine("Innertext: "+xml_paramNames [p].InnerText);
                                    GenumParams.Add(xml_paramNames[p].SelectSingleNode("parameter").InnerText); // Posible atajo.
                                    //Console.WriteLine("ParamName: "+xml_paramNames[p].SelectSingleNode("parameter").InnerText);
                                    paramNames.Add(xml_paramNames[p].SelectSingleNode("parameter").InnerText); //Obtenemos nombre dle parametro.
                                //}
                            }
                            d_methodsParams.Add(methodName, paramNames); // Añadimos metodo y parametros al Diccionario.
                        }
                        GrupoMetodos.Add(d_methodsParams);
                    }
                    
                    #if DEBUG
                    Console.WriteLine("Getting Values");
                    #endif

                    Dictionary<string, List<string>> ParamValues= new Dictionary<string, List<string>>(); // Param, Values

                    for (int g=1;g<=GrupoMetodos.Count;g++) //Repasamos por grupo.
                    {
                        string s_plus = (g==1) ? "" : g.ToString();
                        XmlNodeList xml_param = xdoc.SelectNodes("refentry/refsect1[@id='parameters" + s_plus + "']/variablelist/varlistentry"); // Obtenemos nodos de parametros.
                        #if DEBUG
                        Console.WriteLine("Count: "+xml_param.Count);
                        #endif
                        for (int p=0;p<xml_param.Count;p++) // Recorremos parametros.
                        {
                            string param_name = xml_param[p].SelectSingleNode("term/parameter").InnerText;
                            #if DEBUG
                            Console.WriteLine("Parameter: "+param_name);
                            #endif
                            if (GenumParams.Contains(param_name)) // Coger solo los que sean GEnum. // Esto no tiene efectopor que se han añadido todos.
                            {
                                param_name = Tools.FixedParamName(param_name);
                                List<string> l_values = new List<string>();
                                XmlNodeList cml_values = xml_param[p].SelectNodes("listitem/para/constant"); //Obtenemos nodos de valores
                                
                                for (int v=0;v<cml_values.Count;v++)
                                {
                                    #if DEBUG
                                    Console.WriteLine("    - "+cml_values[v].InnerText);
                                    #endif
                                    l_values.Add(cml_values[v].InnerText); // Obtenemos el valor.
                                }
                                if (l_values.Count > 0) // Solo cogemos si tienen valores constantes y por lo tanto de enumerador.
                                {
                                    if (!ParamValues.ContainsKey(param_name))
                                    {
                                        ParamValues.Add(param_name, l_values); // Añadimos los posibles valores al parametro.
                                    }
                                    else
                                    {
                                        for (int pv=0;pv<l_values.Count;pv++)
                                        {
                                            if (!ParamValues[param_name].Contains(l_values[pv]))
                                            {
                                                ParamValues[param_name].Add(l_values[pv]); // Añadimos los posibles valores al parametro.
                                            }
                                        }                                    
                                    }
                                }
                            }
                        }

                        foreach (string keyf in GrupoMetodos[g-1].Keys) // En este grupo
                        {
                            for (int pf=0;pf<GrupoMetodos[g-1][keyf].Count;pf++) // Recorremos los metodos
                            {
                                if (!d_MethodParamValues.ContainsKey(keyf)) // Si el metodo no existe.
                                {
                                    d_MethodParamValues.Add(keyf, ParamValues); // Añadimos metodo y añadimos los parametros y sus values.
                                }
                                else // Si el metodo ya existe
                                {
                                    foreach (string parval in ParamValues.Keys) // Repasamos los parametros
                                    {
                                        for (int val=0;val<ParamValues[parval].Count;val++) // Recorremos los valores obtenidos
                                        {
                                            if(!d_MethodParamValues[keyf][parval].Contains(ParamValues[parval][val])) // Si el metodo no tiene el valor
                                            {
                                                d_MethodParamValues[keyf][parval].Add(ParamValues[parval][val]); // Añadimos el valor.
                                            }
                                        }
                                    }
                                }
                            }                            
                        }
                        

                        // No se que coño he programado.

                        
                    }

                    // AHORA TOCA Buscar los metodos, repasar sus parametros y a los enumeradores pasarles los valores que no tengan.

                    
                    /*XmlNodeList xml_param = xdoc.SelectNodes("refentry/refsect1[@id='parameters']/varlistentry"); // Obtenemos nodos de parametros.
                    foreach (XmlNode node in xml_param)
                    {
                        string s_paramName = node.SelectSingleNode("term/parameter").Value; // Nombre de parametro.
                        XmlNodeList xml_values = node.SelectNodes("listitem/param/constant");
                        foreach (XmlNode val_node in xml_values)
                        {
                            string param_value = val_node.Value; // Obtenemos valor del parametro.
                            
                        }
                    }*/
                }
                catch (XmlException e)
                {
                    Console.WriteLine(e.GetType().ToString());
                    Console.WriteLine(e.HelpLink);
                    Console.WriteLine(e.HResult);
                    Console.WriteLine(e.InnerException);
                    Console.WriteLine(e.LineNumber);
                    Console.WriteLine(e.LinePosition);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Source);
                    Console.WriteLine(e.SourceUri);
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.TargetSite);
                    Console.WriteLine(e.Data);
                }

                sr.Close();
            }

            //Directory.Delete("./OpenGL-Refpages", true); //Eliminar, ya no es necesario.
            return true; //Exito.
        }

        
    }
}