using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Настройки закрываемости при нажатии вне модального окна
    /// </summary>
    public enum ModalDialogBackdrop
    {
        /// <summary>
        /// Закрываемое кликом вне модального окна
        /// </summary>
        [AttributeValue("data-bs-backdrop", "dynamic")]
        [AttributeValue("data-bs-keyboard", "true")]
        Default = 0,
        /// <summary>
        /// Закрывается только по кнопке или скриптом
        /// </summary>
        [AttributeValue("data-bs-backdrop", "static")]
        [AttributeValue("data-bs-keyboard", "false")]
        Static = 1,
    }
}
