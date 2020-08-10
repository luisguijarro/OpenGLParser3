using System;
using System.Collections.Generic;

namespace OpenGLParser.DataObjects
{
    public class glCommand
    {
        public string ReturnedTipe;
        public Dictionary<string, glParam> Parametros; //<nombre del parametro, Parametro>,
        public glCommand()
        {
            ReturnedTipe = "void";
            this.Parametros = new Dictionary<string, glParam>();
        }
    }
}