using System;
using System.Collections.Generic;

namespace OpenGLParser.DataObjects
{
    public class glExtension
    {
        public List<string> Metodos; //Metodos soportados.
        public glExtension()
        {
            this.Metodos = new List<string>();
        }
    }
}