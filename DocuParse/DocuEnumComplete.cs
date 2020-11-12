using System;

namespace OpenGLParser
{
    public static partial class DocuParser
    {
        public static void CompleteEnums()
        {
            #region Completar Enumeradores con info de documentación. (MAL)

            foreach (string c_key in glReader.d_Commandos.Keys) // Repasamos los comandos parseados.
            {
                if (d_MethodParamValues.ContainsKey(c_key)) // Si el método está en la lista de Documentacion...
                {
                    foreach (string p_key in glReader.d_Commandos[c_key].Parametros.Keys) // Recorremos parametros del método
                    {
                        if (d_MethodParamValues[c_key].ContainsKey(p_key)) // Si el metodo en docu tiene param... Es enumerador
                        {
                            string s_tipo = glReader.d_Commandos[c_key].Parametros[p_key].tipo;
                            if (glReader.d_Enumerators.ContainsKey(s_tipo))
                            {
                                for (int v=0;v<d_MethodParamValues[c_key][p_key].Count;v++) // Recorremos valores del param en Docu.
                                {
                                    string s_val = d_MethodParamValues[c_key][p_key][v];
                                    if (!glReader.d_Enumerators[s_tipo].EnumValues.ContainsKey(s_val)) // Si no tiene le valor...
                                    {
                                        if (glReader.d_Valores.ContainsKey(s_val))
                                        {
                                            // Añadimos el valor al Enumerador.
                                            glReader.d_Enumerators[s_tipo].EnumValues.Add(s_val, glReader.d_Valores[s_val]);
                                        }
                                        else
                                        {
                                            #if DEBUG
                                            Console.WriteLine(c_key+":");
                                            Console.WriteLine("    - Error: El Valor "+s_tipo+" no existe.");
                                            #endif
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // ¿CrearEnumerador? ¿Asignar nuevo enumerador como tipo de valor?
                                #if DEBUG
                                Console.WriteLine(c_key+":");
                                Console.WriteLine("    - Error: El enumerador "+s_tipo+" No existe.");
                                #endif
                            }
                        }
                    }
                }
            }
                        /*if (DocuParser.d_MethodParamValues.ContainsKey(commandName)) // Si está en la lista es por que tiene enums
                        {
                            if (DocuParser.d_MethodParamValues[commandName].ContainsKey(s_ParamName)) // Si el parametro es enumerador.
                            {
                                if (d_Enumerators.ContainsKey(s_paramType)) // confirmamos que existe en la lista de enumeradores
                                {
                                    for (int en=0;en<DocuParser.d_MethodParamValues[commandName][s_ParamName].Count;en++) // Recorremos Valores obtenidos de la lectura de Docu.
                                    {
                                        string docuval = DocuParser.d_MethodParamValues[commandName][s_ParamName][en]; // Obtenemos el nombre del valor.
                                        if (!d_Enumerators[s_paramType].EnumValues.ContainsKey(docuval)) // Si el enumerador no tiene el valor.
                                        {
                                            if (d_Valores.ContainsKey(docuval))
                                            {
                                                d_Enumerators[s_paramType].EnumValues.Add(docuval, d_Valores[docuval]); // Añadimos el valor al enumerador.
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error: The value of "+docuval+" not finded");
                                            }
                                        }
                                    }
                                    //d_Enumerators[s_ParamType].EnumValues.ContainsKey()
                                }
                                else
                                {
                                    Console.WriteLine("Error: Enumerador "+s_paramType+" no finded");
                                }
                            }
                        }*/

            #endregion
        }
    }
}