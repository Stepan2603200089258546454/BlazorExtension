using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Визуальные стили
    /// </summary>
    public enum AlertStyleType
    {
        /// <summary>
        /// Без стилей
        /// </summary>
        [StyleValue]
        Default = 0,
        [StyleValue("alert alert-primary")]
        Primary = 1,
        [StyleValue("alert alert-secondary")]
        Secondary = 2,
        [StyleValue("alert alert-success")]
        Success = 3,
        [StyleValue("alert alert-danger")]
        Danger = 4,
        [StyleValue("alert alert-warning")]
        Warning = 5,
        [StyleValue("alert alert-info")]
        Info = 6,
        [StyleValue("alert alert-light")]
        Light = 7,
        [StyleValue("alert alert-dark")]
        Dark = 8,
    }
}
