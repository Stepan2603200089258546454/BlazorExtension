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
    }
}
