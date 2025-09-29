namespace CommonComponents.Enums.Exstension
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal class StringValueAttribute : Attribute
    {
        public StringValueAttribute() : this(string.Empty) { }
        public StringValueAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}
