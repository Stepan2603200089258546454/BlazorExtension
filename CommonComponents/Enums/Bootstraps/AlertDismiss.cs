using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Варианты закрываемости
    /// </summary>
    public enum AlertDismiss
    {
        /// <summary>
        /// Не добавлять кнопку закрытия + не добавлять стили
        /// </summary>
        [StyleValue]
        Disable = 0,
        /// <summary>
        /// Добавить кнопку закрыть + добавить стили
        /// </summary>
        [StyleValue("alert-dismissible fade show")]
        Enable = 1,
    }
}
