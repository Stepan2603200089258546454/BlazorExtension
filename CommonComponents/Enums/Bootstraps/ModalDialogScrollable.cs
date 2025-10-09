using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Настройки прокрутки модального окна
    /// </summary>
    public enum ModalDialogScrollable
    {
        /// <summary>
        /// Прокручиваемое модальное окно
        /// </summary>
        [StyleValue]
        Default = 0,
        /// <summary>
        /// Прокручиваемое содержимое модального окна
        /// </summary>
        [StyleValue("modal-dialog-scrollable")]
        Scrollable = 1,
    }
}
