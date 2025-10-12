using CommonComponents.Enums.Bootstraps;
using CommonComponents.Enums.Exstension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Models.Bootstraps
{
    public class AccordionSettings
    {
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Стили
        /// </summary>
        public AccordionStyleType StyleType { get; set; } = AccordionStyleType.Default;
        /// <summary>
        /// Тип открываемости
        /// </summary>
        public AccordionCollapsedType CollapsedType { get; set; } = AccordionCollapsedType.Collapsed;
        /// <summary>
        /// Произвольные стили
        /// </summary>
        public string AddClass { get; set; } = string.Empty;
        /// <summary>
        /// Итоговые стили
        /// </summary>
        public string Class => $"{StyleType.GetStyleValue()} {AddClass}".Trim();

        public AccordionItemSettings GetItemSettings(string id, bool isActive = false)
        {
            return new AccordionItemSettings(
                id, 
                CollapsedType == AccordionCollapsedType.Collapsed ? Id : string.Empty, 
                isActive);
        }
    }
}
