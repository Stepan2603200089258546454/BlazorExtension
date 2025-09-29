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
        Default = 0,
        /// <summary>
        /// Закрывается только по кнопке или скриптом
        /// </summary>
        Static = 1,
    }
}
