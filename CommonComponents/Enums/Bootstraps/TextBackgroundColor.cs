using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    public enum TextBackgroundColor
    {
        [StyleValue]
        Default = 0,
        [StyleValue("text-bg-primary")]
        Primary,
        [StyleValue("text-bg-secondary")]
        Secondary,
        [StyleValue("text-bg-success")]
        Success,
        [StyleValue("text-bg-danger")]
        Danger,
        [StyleValue("text-bg-warning")]
        Warning,
        [StyleValue("text-bg-info")]
        Info,
        [StyleValue("text-bg-light")]
        Light,
        [StyleValue("text-bg-dark")]
        Dark,
    }
}
