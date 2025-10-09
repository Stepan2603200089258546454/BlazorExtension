using CommonComponents.Enums.Bootstraps;

namespace CommonComponents.Models.Bootstraps
{
    public abstract class BaseTabSettings
    {
        /// <summary>
        /// id объекта
        /// </summary>
        public required string Id { get; set; }
        /// <summary>
        /// Тип TabList элемента
        /// </summary>
        public TabListType Type { get; set; } = TabListType.Tab;
    }
}
