using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Выравнивание текста
    /// </summary>
    public enum TextAligned
    {
        /// <summary>
        /// По левому краю
        /// </summary>
        [StyleValue]
        Default = 0,
        /// <summary>
        /// По центру
        /// </summary>
        [StyleValue("text-center")]
        Center = 1,
        /// <summary>
        /// По правому краю
        /// </summary>
        [StyleValue("text-end")]
        End = 2,
    }
}
