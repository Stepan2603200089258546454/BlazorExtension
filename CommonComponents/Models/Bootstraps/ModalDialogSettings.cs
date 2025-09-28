using CommonComponents.Enums.Bootstraps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Models.Bootstraps
{
    public class ModalDialogSettings
    {
        public string Title { get; set; } = "Заголовок";
        /// <summary>
        /// Настройки закрываемости при нажатии вне модального окна
        /// </summary>
        public ModalDialogBackdrop Backdrop { get; set; } = ModalDialogBackdrop.Default;
        /// <summary>
        /// Настройки позиции модального окна
        /// </summary>
        public ModalDialogPosition Position { get; set; } = ModalDialogPosition.Default;
        private string PositionValueStyle => Position switch
        {
            ModalDialogPosition.Default => string.Empty,
            ModalDialogPosition.Center => "modal-dialog-centered",
            _ => string.Empty,
        };
        /// <summary>
        /// Настройки прокрутки модального окна
        /// </summary>
        public ModalDialogScrollable Scrollable { get; set; } = ModalDialogScrollable.Default;
        private string ScrollableValueStyle => Scrollable switch
        {
            ModalDialogScrollable.Default => string.Empty,
            ModalDialogScrollable.Scrollable => "modal-dialog-scrollable",
            _ => string.Empty,
        };
        /// <summary>
        /// Настройки размера модального окна
        /// </summary>
        public ModalDialogSize Size { get; set; } = ModalDialogSize.Default;
        private string SizeValueStyle => Size switch
        {
            ModalDialogSize.Default => string.Empty,
            ModalDialogSize.Large => "modal-lg",
            ModalDialogSize.Small => "modal-sm",
            ModalDialogSize.XL => "modal-xl",
            _ => string.Empty,
        };
        /// <summary>
        /// Настройки полноэкранного режима модального окна
        /// </summary>
        public ModalDialogFullScreen FullScreen { get; set; } = ModalDialogFullScreen.Default;
        private string FullScreenValueStyle => FullScreen switch
        {
            ModalDialogFullScreen.Default => string.Empty,
            ModalDialogFullScreen.FullScreen => "modal-fullscreen",
            ModalDialogFullScreen.SM => "modal-fullscreen-sm-down",
            ModalDialogFullScreen.MD => "modal-fullscreen-md-down",
            ModalDialogFullScreen.LG => "modal-fullscreen-lg-down",
            ModalDialogFullScreen.XL => "modal-fullscreen-xl-down",
            ModalDialogFullScreen.XXL => "modal-fullscreen-xxl-down",
            _ => string.Empty,
        };
        /// <summary>
        /// Итоговые стили модального окна
        /// </summary>
        public string ValueStyle => string.Join(' ', PositionValueStyle, ScrollableValueStyle, SizeValueStyle, FullScreenValueStyle);
        /// <summary>
        /// Включен ли футер модального окна
        /// </summary>
        public bool EnabledFooter { get; set; } = true;
        /// <summary>
        /// Включен ли заголовок модального окна
        /// </summary>
        public bool EnabledHeader { get; set; } = true;
        /// <summary>
        /// Включена ли кнопка закрытия в заголовке модального окна
        /// </summary>
        public bool EnabledButtonCancelHead { get; set; } = true;
        /// <summary>
        /// Включена ли кнопка закрытия модального окна
        /// </summary>
        public bool EnabledButtonCancel { get; set; } = true;
        /// <summary>
        /// Текст кнопки закрытия модального окна
        /// </summary>
        public string TextButtonCancel { get; set; } = "Закрыть";
        /// <summary>
        /// Включена ли кнопка подтверждения модального окна
        /// </summary>
        public bool EnabledButtonOk { get; set; } = true;
        /// <summary>
        /// Текст кнопки подтверждения модального окна
        /// </summary>
        public string TextButtonOk { get; set; } = "Да";
        /// <summary>
        /// Включена ли кнопка отказа модального окна
        /// </summary>
        public bool EnabledButtonNo { get; set; } = true;
        /// <summary>
        /// Текст кнопки отказа модального окна
        /// </summary>
        public string TextButtonNo { get; set; } = "Нет";
    }
}
