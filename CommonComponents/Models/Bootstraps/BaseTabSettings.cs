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
        /// <summary>
        /// Значение типа элемента
        /// </summary>
        public string TypeValue => Type switch
        {
            TabListType.Tab => "tab",
            TabListType.Pill => "pill",
            _ => "tab"
        };
    }
}
