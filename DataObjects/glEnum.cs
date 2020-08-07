using System;
using System.Collections.Generic;

namespace OpenGLParser.DataObjects
{
    public class glEnum
    {
        public Dictionary<string,glEnumValue> EnumValues; //  <Nombre del valor, Valor>

        public Type Tipo; // Tipo de Valor del Enumerador ¿int? ¿uint? ¿long?....
        public glEnum()
        {
            Tipo = typeof(uint); //by default
            this.EnumValues = new Dictionary<string, glEnumValue>();
        }
    }
}