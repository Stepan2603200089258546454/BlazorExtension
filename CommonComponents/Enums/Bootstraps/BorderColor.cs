using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Цвета обводки 
    /// </summary>
    public enum BorderColor
    {
        [StyleValue]
        Default = 0,
        [StyleValue("border-primary")]
        Primary,
        [StyleValue("border-secondary")]
        Secondary,
        [StyleValue("border-success")]
        Success,
        [StyleValue("border-danger")]
        Danger,
        [StyleValue("border-warning")]
        Warning,
        [StyleValue("border-info")]
        Info,
        [StyleValue("border-light")]
        Light,
        [StyleValue("border-dark")]
        Dark,
    }
}
