using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    public enum AccordionStyleType
    {
        /// <summary>
        /// Без стилей
        /// </summary>
        [StyleValue]
        Empty = -1,
        /// <summary>
        /// Стиль с краями и рамками
        /// </summary>
        [StyleValue("accordion")]
        Default = 0,
        /// <summary>
        /// Стиль без краев и рамок
        /// </summary>
        [StyleValue("accordion accordion-flush")]
        Flush = 1,
    }
}
