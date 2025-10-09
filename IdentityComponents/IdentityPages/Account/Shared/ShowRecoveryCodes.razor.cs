using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Shared
{
    public partial class ShowRecoveryCodes : ComponentBase
    {
        [Parameter]
        public string[] RecoveryCodes { get; set; } = [];

        [Parameter]
        public string? StatusMessage { get; set; }
    }
}
