using CommonComponents.Models.Bootstraps;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Components.Bootstraps
{
    public partial class CardFooter : ComponentBase
    {
        [Parameter]
        public CardFooterSettings Settings { get; set; } = new CardFooterSettings();
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
