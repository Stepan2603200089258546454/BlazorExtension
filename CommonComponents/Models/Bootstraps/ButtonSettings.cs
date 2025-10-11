using CommonComponents.Enums.Bootstraps;
using CommonComponents.Enums.Exstension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Models.Bootstraps
{
    public class ButtonSettings
    {
        public static ButtonSettings GetPresetDefault(string id, ButtonStyleType StyleType = ButtonStyleType.Primary)
        {
            return new ButtonSettings()
            {
                Id = id,
                StyleType = StyleType,
            };
        }
        public static ButtonSettings GetPresetOpenModal(string id, string idModal, ButtonStyleType StyleType = ButtonStyleType.Primary)
        {
            return new ButtonSettings()
            {
                Id = id,
                StyleType = StyleType,
                AttributeSettings = new ButtonAttributeSettings().OpenModal(idModal)
            };
        }
        public static ButtonSettings GetPresetCloseModal(string Id, ButtonStyleType StyleType = ButtonStyleType.Primary)
        {
            return new ButtonSettings()
            {
                Id = Id,
                StyleType = StyleType,
                AttributeSettings = new ButtonAttributeSettings().CloseModal()
            };
        }
        public static ButtonSettings GetPresetAccordionItemHeader(string HeaderClass, string Id, bool IsActive)
        {
            return new ButtonSettings()
            {
                Id = string.Empty,
                StyleType = ButtonStyleType.Empty,
                AddClass = HeaderClass,
                Type = ButtonType.Button,
                AttributeSettings = new ButtonAttributeSettings().AccordionItemHeader(Id, IsActive)
            };
        }

        /// <summary>
        /// Id модального окна
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Дополнительные стили (записываются в class)
        /// </summary>
        public string AddClass { get; set; } = string.Empty;
        /// <summary>
        /// Тип кнопки
        /// </summary>
        public ButtonType Type { get; set; } = ButtonType.Button;
        /// <summary>
        /// Стиль кнопки
        /// </summary>
        public ButtonStyleType StyleType { get; set; } = ButtonStyleType.Primary;
        /// <summary>
        /// Размер кнопки
        /// </summary>
        public ButtonSize Size { get; set; } = ButtonSize.Default;
        /// <summary>
        /// Дополнительные атрибуты
        /// </summary>
        public ButtonAttributeSettings? AttributeSettings { get; set; }

        public string Class => $"{StyleType.GetStyleValue()} {Size.GetStyleValue()} {AddClass}".Trim();
    }
}
