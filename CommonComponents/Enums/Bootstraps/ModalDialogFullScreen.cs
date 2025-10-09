using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Настройки полноэкранного режима модального окна
    /// </summary>
    public enum ModalDialogFullScreen
    {
        /// <summary>
        /// Выключен
        /// </summary>
        [StyleValue]
        Default = 0,
        /// <summary>
        /// Всегда
        /// </summary>
        [StyleValue("modal-fullscreen")]
        FullScreen = 1,
        /// <summary>
        /// 
        /// </summary>
        [StyleValue("modal-fullscreen-sm-down")]
        SM = 2,
        /// <summary>
        /// 
        /// </summary>
        [StyleValue("modal-fullscreen-md-down")]
        MD = 3,
        /// <summary>
        /// 
        /// </summary>
        [StyleValue("modal-fullscreen-lg-down")]
        LG = 4,
        /// <summary>
        /// 
        /// </summary>
        [StyleValue("modal-fullscreen-xl-down")] 
        XL = 5,
        /// <summary>
        /// 
        /// </summary>
        [StyleValue("modal-fullscreen-xxl-down")] 
        XXL = 6,
    }
}
