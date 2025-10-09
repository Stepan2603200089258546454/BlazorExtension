using CommonComponents.Models.Bootstraps;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Components.Bootstraps
{
    public partial class TabList : ComponentBase
    {
        /// <summary>
        /// Настройки вкладок
        /// </summary>
        [Parameter, EditorRequired]
        public TabListSettings Settings { get; set; }
        /// <summary>
        /// Содержимое заголовков вкладок
        /// </summary>
        [Parameter, EditorRequired]
        public RenderFragment? HeaderContent { get; set; }
        /// <summary>
        /// Содержимое контента вкладок
        /// </summary>
        [Parameter, EditorRequired]
        public RenderFragment? BodyContent { get; set; }
    }
}
