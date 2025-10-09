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
        public required string Id { get; set; }
        public AccordionType Type { get; set; } = AccordionType.Default;
        public AccordionCollapsedType CollapsedType { get; set; } = AccordionCollapsedType.Collapsed;

        public string Class => $"accordion {Type.GetStyleValue()}".Trim();

        public AccordionItemSettings GetItemSettings(string id, bool isActive = false)
        {
            return new AccordionItemSettings(id, CollapsedType == AccordionCollapsedType.Collapsed ? Id : string.Empty, isActive);
        }
    }
}
