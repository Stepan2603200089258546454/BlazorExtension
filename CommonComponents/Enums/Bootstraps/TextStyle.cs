using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
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
}
