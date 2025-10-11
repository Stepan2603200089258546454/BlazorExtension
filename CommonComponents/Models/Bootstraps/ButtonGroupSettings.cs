using CommonComponents.Enums.Bootstraps;
using CommonComponents.Enums.Exstension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Models.Bootstraps
{
    public class ButtonGroupSettings
    {
        /// <summary>
        /// Ориентация расположения кнопок
        /// </summary>
        public ButtonGroupOrientation Orientation { get; set; } = ButtonGroupOrientation.Horizontal;
        /// <summary>
        /// Размер кнопок
        /// </summary>
        public ButtonGroupSize Size { get; set; } = ButtonGroupSize.Default;
        /// <summary>
        /// Дополнительные произвольные классы
        /// </summary>
        public string AddClass { get; set; } = string.Empty;
        /// <summary>
        /// Итоговые стили
        /// </summary>
        public string Class => $"{Orientation.GetStyleValue()} {Size.GetStyleValue()} {AddClass}".Trim();
    }
}
