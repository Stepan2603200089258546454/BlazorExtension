using CommonComponents.Models.Bootstraps;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Components.Bootstraps
{
    public partial class AccordionItem : ComponentBase
    {
        /// <summary>
        /// Настройки элемента гармошки
        /// </summary>
        [Parameter, EditorRequired]
        public AccordionItemSettings Settings { get; set; }
        /// <summary>
        /// Контент заголовка
        /// </summary>
        [Parameter, EditorRequired]
        public RenderFragment? HeaderContent { get; set; }
        /// <summary>
        /// Контент тела
        /// </summary>
        [Parameter, EditorRequired]
        public RenderFragment? BodyContent { get; set; }
    }
}
