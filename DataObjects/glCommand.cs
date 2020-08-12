using System;
using System.Collections.Generic;

namespace OpenGLParser.DataObjects
{
    public class glCommand
    {
        public bool EsInseguro;
        public string FromVersion; //Versión desde la que aparece
        public string DeprecatedVersion; //Versión en la que se marca como Obsoleto
        public string ReturnedTipe; //Tipo de dato que retorna.
        public Dictionary<string, glParam> Parametros; //<nombre del parametro, Parametro>,
        public glCommand()
        {
            FromVersion = "";
            DeprecatedVersion = "";
            ReturnedTipe = "void"; //void por defecto.
            this.Parametros = new Dictionary<string, glParam>();
        }
    }
}