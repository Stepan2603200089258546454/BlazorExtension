using CommonComponents.Enums.Bootstraps;
using CommonComponents.Enums.Exstension;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Models.Bootstraps
{
    public class ModalDialogSettings
    {
        /// <summary>
        /// Id модального окна
        /// </summary>
        public required string Id { get; set; }
        /// <summary>
        /// Вычисляемое поле связки для браузера
        /// </summary>
        public string IdLabel => Id + "Label";
        /// <summary>
        /// Настройки закрываемости при нажатии вне модального окна
        /// </summary>
        public ModalDialogBackdrop Backdrop { get; set; } = ModalDialogBackdrop.Default;
        /// <summary>
        /// Настройки позиции модального окна
        /// </summary>
        public ModalDialogPosition Position { get; set; } = ModalDialogPosition.Default;
        /// <summary>
        /// Настройки прокрутки модального окна
        /// </summary>
        public ModalDialogScrollable Scrollable { get; set; } = ModalDialogScrollable.Default;
        /// <summary>
        /// Настройки размера модального окна
        /// </summary>
        public ModalDialogSize Size { get; set; } = ModalDialogSize.Default;
        /// <summary>
        /// Настройки полноэкранного режима модального окна
        /// </summary>
        public ModalDialogFullScreen FullScreen { get; set; } = ModalDialogFullScreen.Default;
        /// <summary>
        /// Итоговые стили модального окна
        /// </summary>
        public string Class => string.Join(' ', Position.GetStyleValue(), Scrollable.GetStyleValue(), Size.GetStyleValue(), FullScreen.GetStyleValue()).Trim();
        /// <summary>
        /// Включена ли кнопка закрытия в заголовке модального окна
        /// </summary>
        public bool EnabledButtonCancelHead { get; set; } = true;
        /// <summary>
        /// Настройки кнопки в заголовке модального окна
        /// </summary>
        public ButtonSettings HeaderButtonCancelSettings { get; set; } = ButtonSettings.GetPresetCloseModal(string.Empty, ButtonStyleType.Close);
        /// <summary>
        /// Настройки кнопки закрытия модального окна
        /// </summary>
        public ButtonSettings ButtonCancelSettings { get; set; } = ButtonSettings.GetPresetCloseModal(string.Empty, ButtonStyleType.Secondary);
        /// <summary>
        /// Настройки кнопки подтверждения модального окна
        /// </summary>
        public ButtonSettings ButtonOkSettings { get; set; } = ButtonSettings.GetPresetCloseModal(string.Empty, ButtonStyleType.Primary);
        /// <summary>
        /// Настройки кнопки отклонения модального окна
        /// </summary>
        public ButtonSettings ButtonNoSettings { get; set; } = ButtonSettings.GetPresetCloseModal(string.Empty, ButtonStyleType.Danger);
    }
}
