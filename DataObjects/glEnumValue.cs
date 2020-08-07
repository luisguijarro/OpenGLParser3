using System;

namespace OpenGLParser.DataObjects
{
    public struct glEnumValue
    {
        public string Name;
        public string Value;
        public glEnumValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}