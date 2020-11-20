using System;
using System.Collections.Generic;

namespace OpenGLParser.DataObjects
{
    public class glParam
    {
        public AccesParam Acces; // Tipo de Acceso al Parametro, A emplear para punteros.
        public string tipo;
        // public int len; //Si es un Array tiene 0 o más
        public bool esArray;
        public int esPuntero; //cambiamos bool por int para contar el numero de *
        public glParam()
        {
            Acces = AccesParam.Unknown;
            tipo = "";
            //this.len = -1; //Si es un Array tiene 0 o más
            esArray = false;
        }
    }


    public enum AccesParam
    {
        Unknown = 0, In, Out, Ref
    }
}