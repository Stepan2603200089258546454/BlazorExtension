using CommonComponents.Models.Bootstraps;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Components.Bootstraps
{
    public partial class CardHeader : ComponentBase
    {
        [Parameter]
        public CardHeaderSettings Settings { get; set; } = new CardHeaderSettings();
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
