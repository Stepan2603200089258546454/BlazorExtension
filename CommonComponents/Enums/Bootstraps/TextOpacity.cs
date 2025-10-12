using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
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
}
