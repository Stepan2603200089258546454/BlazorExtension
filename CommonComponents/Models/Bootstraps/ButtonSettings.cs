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
        /// <summary>
        /// Id модального окна
        /// </summary>
        public required string Id { get; set; }
        public string AddClassesValue { get; set; } = string.Empty;
        public ButtonType Type { get; set; } = ButtonType.Button;
        public ButtonStyleType StyleType { get; set; } = ButtonStyleType.Primary;
        public ButtonSize Size { get; set; } = ButtonSize.Default;

        public string Class => $"{StyleType.GetStyleValue()} {Size.GetStyleValue()} {AddClassesValue}".Trim();
    }
}
