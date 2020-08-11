using System;

namespace OpenGLParser.DataObjects
{
    public struct glEnumValue
    {
        public string Name;
        public string Value;
        public string Group;
        public glEnumValue(string name, string value, string group)
        {
            Name = name;
            Value = value;
            Group = group;
        }
    }
}