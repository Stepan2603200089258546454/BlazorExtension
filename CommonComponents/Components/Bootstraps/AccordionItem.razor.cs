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
        [Parameter, EditorRequired]
        public AccordionItemSettings Settings { get; set; }
        [Parameter, EditorRequired]
        public RenderFragment? HeaderContent { get; set; }
        [Parameter, EditorRequired]
        public RenderFragment? BodyContent { get; set; }
    }
}
