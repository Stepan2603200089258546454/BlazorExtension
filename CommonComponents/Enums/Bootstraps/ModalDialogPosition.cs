using CommonComponents.Enums.Exstension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Enums.Bootstraps
{
    /// <summary>
    /// Настройки позиции модального окна
    /// </summary>
    public enum ModalDialogPosition
    {
        /// <summary>
        /// Вверху экрана
        /// </summary>
        [StyleValue]
        Default = 0,
        /// <summary>
        /// По центру экрана
        /// </summary>
        [StyleValue("modal-dialog-centered")]
        Center = 1,
    }
}
