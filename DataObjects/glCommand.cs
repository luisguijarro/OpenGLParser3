using System;
using System.Collections.Generic;

namespace OpenGLParser.DataObjects
{
    public class glCommand
    {
        public bool EsInseguro; // Devuelve si en algún momento delmetodo se emplean punteros.
        public string FromVersion; //Versión desde la que aparece
        public string FromGlesVersion; //Versión desde la que aparece
        public string DeprecatedVersion; //Versión en la que se marca como Obsoleto
        public string DeprecatedGlesVersion; //Versión en la que se marca como Obsoleto
        public string ReturnedType; //Tipo de dato que retorna.
        public bool ReturnedTypePointer; //¿El valor retornado es Puntero?
        public Dictionary<string, glParam> Parametros; //<nombre del parametro, Parametro>,
        public glCommand()
        {
            EsInseguro = false; // no emplea punteros por defecto.
            ReturnedTypePointer = false; // no devuelve puntero por defecto.
            FromVersion = "";
            DeprecatedVersion = "";
            ReturnedType = "void"; //void por defecto.
            this.Parametros = new Dictionary<string, glParam>();
        }
    }

}