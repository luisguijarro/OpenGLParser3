using System;
using System.Collections.Generic;

namespace OpenGLParser.DataObjects
{
    public class glVersion
    {
        public List<string> Obsoletos; //Metodos que se han quedado obsoletos
        public List<string> Metodos; //Metodos soportados.
        public glVersion()
        {
            this.Obsoletos = new List<string>();
            this.Metodos = new List<string>();
        }
    }
}