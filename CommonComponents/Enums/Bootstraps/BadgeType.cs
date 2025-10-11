using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    public enum BadgeStyleType
    {
        /// <summary>
        /// Без стилей
        /// </summary>
        [StyleValue]
        Default = 0,
        [StyleValue("badge text-bg-primary")]
        Primary = 1,
        [StyleValue("badge text-bg-secondary")]
        Secondary = 2,
        [StyleValue("badge text-bg-success")]
        Success = 3,
        [StyleValue("badge text-bg-danger")]
        Danger = 4,
        [StyleValue("badge text-bg-warning")]
        Warning = 5,
        [StyleValue("badge text-bg-info")]
        Info = 6,
        [StyleValue("badge text-bg-light")]
        Light = 7,
        [StyleValue("badge text-bg-dark")]
        Dark = 8,
    }
}
