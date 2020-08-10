using System;
using System.Collections.Generic;

namespace OpenGLParser.DataObjects
{
    public class glParam
    {
        public string tipo;
        // public int len; //Si es un Array tiene 0 o más
        public bool esArray;
        public glParam()
        {
            tipo = "";
            //this.len = -1; //Si es un Array tiene 0 o más
            esArray = false;
        }
    }
}