using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Pages
{
    [Route(IdentityConst.IdentityRoute.Account.ConfirmEmail)]
    public partial class ConfirmEmail : ComponentBase
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromQuery]
        private string? UserId { get; set; }

        [SupplyParameterFromQuery]
        private string? Code { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await UserManager.ConfirmEmailPageAsync(HttpContext, UserId, Code);
        }
    }
}
