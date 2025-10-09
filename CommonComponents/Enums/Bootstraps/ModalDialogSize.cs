using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Настройки размера модального окна
    /// </summary>
    public enum ModalDialogSize
    {
        /// <summary>
        /// Нормальный
        /// </summary>
        [StyleValue]
        Default = 0,
        /// <summary>
        /// Маленький
        /// </summary>
        [StyleValue("modal-sm")]
        Small = 1,
        /// <summary>
        /// Большой
        /// </summary>
        [StyleValue("modal-lg")]
        Large = 2,
        /// <summary>
        /// Очень большой
        /// </summary>
        [StyleValue("modal-xl")]
        XL = 3,
    }
}
