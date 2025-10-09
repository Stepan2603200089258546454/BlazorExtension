using System.Reflection;

namespace CommonComponents.Enums.Exstension
{
    internal static class EnumHelper
    {
        public static T? GetAttribute<T>(this Enum value) where T : Attribute 
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            T? attribute = field?.GetCustomAttribute<T>();
            return attribute;
        }
        public static IEnumerable<T>? GetAttributes<T>(this Enum value) where T : Attribute
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            IEnumerable<T>? attribute = field?.GetCustomAttributes<T>();
            return attribute;
        }
        public static string GetStyleValue(this Enum value)
        {
            StyleValueAttribute? attribute = value.GetAttribute<StyleValueAttribute>();
            return attribute?.Value ?? string.Empty;
        }
        public static string GetStringValue(this Enum value)
        {
            StringValueAttribute? attribute = value.GetAttribute<StringValueAttribute>();
            return attribute?.Value ?? string.Empty;
        }
        public static string GetAttributeValue(this Enum value, string key)
        {
            IEnumerable<AttributeValueAttribute>? attributes = value.GetAttributes<AttributeValueAttribute>();
            return attributes?.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty;
        }
    }
}
