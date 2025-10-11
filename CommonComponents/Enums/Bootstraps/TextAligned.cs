using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Выравнивание текста
    /// </summary>
    public enum TextAligned
    {
        /// <summary>
        /// По левому краю
        /// </summary>
        [StyleValue]
        Default = 0,
        /// <summary>
        /// По центру
        /// </summary>
        [StyleValue("text-center")]
        Center = 1,
        /// <summary>
        /// По правому краю
        /// </summary>
        [StyleValue("text-end")]
        End = 2,
    }
    /// <summary>
    /// Стили текста
    /// </summary>
    public enum TextStyle
    {
        [StyleValue]
        Default = 0,
        [StyleValue("text-primary")]
        Primary,
        [StyleValue("text-primary-emphasis")]
        PrimaryEmphasis,
        [StyleValue("text-secondary")]
        Secondary,
        [StyleValue("text-secondary-emphasis")]
        SecondaryEmphasis,
        [StyleValue("text-success")]
        Success,
        [StyleValue("text-success-emphasis")]
        SuccessEmphasis,
        [StyleValue("text-danger")]
        Danger,
        [StyleValue("text-danger-emphasis")]
        DangerEmphasis,
        [StyleValue("text-warning")]
        Warning,
        [StyleValue("text-warning-emphasis")]
        WarningEmphasis,
        [StyleValue("text-info")]
        Info,
        [StyleValue("text-info-emphasis")]
        InfoEmphasis,
        [StyleValue("text-light")]
        Light,
        [StyleValue("text-light-emphasis")]
        LightEmphasis,
        [StyleValue("text-dark")]
        Dark,
        [StyleValue("text-dark-emphasis")]
        DarkEmphasis,

        [StyleValue("text-body")]
        Body,
        [StyleValue("text-body-emphasis")]
        BodyEmphasis,
        [StyleValue("text-body-secondary")]
        BodySecondary,
        [StyleValue("text-body-tertiary")]
        BodyTertiary,

        [StyleValue("text-black")]
        Black,
        [StyleValue("text-black-50")]
        Black50,
        [StyleValue("text-white")]
        White,
        [StyleValue("text-white-50")]
        White50,
    }
    /// <summary>
    /// Непрозрачность текста
    /// </summary>
    public enum TextOpacity
    {
        [StyleValue]
        Default = 0,
        [StyleValue("text-opacity-75")]
        Opacity75,
        [StyleValue("text-opacity-50")]
        Opacity50,
        [StyleValue("text-opacity-25")]
        Opacity25,
    }
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
