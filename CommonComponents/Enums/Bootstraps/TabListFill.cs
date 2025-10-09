using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Заполнение ширины объекта вкладками
    /// </summary>
    public enum TabListFill
    {
        /// <summary>
        /// Выключено
        /// </summary>
        [StyleValue]
        Default = 0,
        /// <summary>
        /// Во всю ширину
        /// </summary>
        [StyleValue("nav-fill")]
        Fill = 1,
        /// <summary>
        /// Во всю ширину и одинаковые элементы по ширине
        /// </summary>
        [StyleValue("nav-justified")]
        Justified = 2,
    }
}
