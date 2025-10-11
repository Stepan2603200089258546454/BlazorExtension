using CommonComponents.Models.Bootstraps;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Components.Bootstraps
{
    public partial class ButtonGroup : ComponentBase
    {
        [Parameter]
        public ButtonGroupSettings Settings { get; set; } = new ButtonGroupSettings();
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
