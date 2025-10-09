using CommonComponents.Models.Bootstraps;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Components.Bootstraps
{
    public partial class TabListHeaderItem : ComponentBase
    {
        [Parameter, EditorRequired]
        public TabListItemSettings Settings { get; set; }
        [Parameter, EditorRequired]
        public RenderFragment ChildContent { get; set; }
    }
}
