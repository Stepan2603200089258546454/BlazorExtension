using CommonComponents.Enums.Bootstraps;
using CommonComponents.Enums.Exstension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Models.Bootstraps
{
    public class AlertSettings
    {
        /// <summary>
        /// Визуальный тип уведомления
        /// </summary>
        public AlertStyleType StyleType { get; set; } = AlertStyleType.Primary;
        /// <summary>
        /// Настройки закрываемости уведомления
        /// </summary>
        public AlertDismiss Dismiss { get; set; } = AlertDismiss.Disable;
        /// <summary>
        /// Дополнительные произвольные стили
        /// </summary>
        public string AddClass { get; set; } = string.Empty;
        /// <summary>
        /// Итоговые стили
        /// </summary>
        public string Class => $"{StyleType.GetStyleValue()} {Dismiss.GetStyleValue()} {AddClass}".Trim();
        /// <summary>
        /// Настройки кнопки закрытия
        /// </summary>
        public ButtonSettings ButtonSettings { get; private set; } = new ButtonSettings()
        {
            Id = string.Empty,
            Type = ButtonType.Button,
            StyleType = ButtonStyleType.Close,
            AttributeSettings = new ButtonAttributeSettings().CloseAlert().AddAriaLabel("Закрыть")
        };
    }
}
