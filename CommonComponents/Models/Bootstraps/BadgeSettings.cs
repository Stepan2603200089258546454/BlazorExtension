using CommonComponents.Enums.Bootstraps;
using CommonComponents.Enums.Exstension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Models.Bootstraps
{
    public class BadgeSettings
    {
        /// <summary>
        /// Стиль элемента
        /// </summary>
        public BadgeStyleType StyleType { get; set; } = BadgeStyleType.Danger;

        public string Class => $"{StyleType.GetStyleValue()}".Trim();
    }
}
