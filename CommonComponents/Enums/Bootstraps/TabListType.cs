using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Тип TabList элемента
    /// </summary>
    public enum TabListType
    {
        /// <summary>
        /// Вкладки
        /// </summary>
        [StyleValue("nav-tabs")]
        [AttributeValue("data-bs-toggle", "tab")]
        Tab = 0,
        /// <summary>
        /// Кнопки
        /// </summary>
        [StyleValue("nav-pills")]
        [AttributeValue("data-bs-toggle", "pill")]
        Pill = 1,
    }
}
