using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Enums.Exstension
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal class StyleValueAttribute : Attribute
    {
        public StyleValueAttribute() : this(string.Empty) { }
        public StyleValueAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}
