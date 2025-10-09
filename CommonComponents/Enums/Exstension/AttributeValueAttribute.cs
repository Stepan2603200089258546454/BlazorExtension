namespace CommonComponents.Enums.Exstension
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    internal class AttributeValueAttribute : Attribute
    {
        public AttributeValueAttribute() : this(string.Empty, string.Empty)
        {

        }
        public AttributeValueAttribute(string key) : this(key, string.Empty)
        {

        }
        public AttributeValueAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }
    }
}
