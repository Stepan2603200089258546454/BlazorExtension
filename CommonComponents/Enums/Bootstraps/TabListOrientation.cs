using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Расположение вкладок
    /// </summary>
    public enum TabListOrientation
    {
        /// <summary>
        /// Горизонтальное расположение вкладок
        /// </summary>
        [StyleValue]
        [AttributeValue("div class")]
        [AttributeValue("aria-orientation", "horizontal")]
        Horizontal = 0,
        /// <summary>
        /// Вертикальное расположение вкладок
        /// </summary>
        [AttributeValue("div class", "d-flex align-items-start")]
        [AttributeValue("aria-orientation", "vertical")]
        [StyleValue("flex-column me-3")]
        Vertical = 1,
    }
}
