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
    [Route(IdentityConst.IdentityRoute.Account.ConfirmEmailChange)]
    public partial class ConfirmEmailChange : ComponentBase
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromQuery]
        private string? UserId { get; set; }

        [SupplyParameterFromQuery]
        private string? Email { get; set; }

        [SupplyParameterFromQuery]
        private string? Code { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await UserManager.ConfirmEmailChangePageAsync(HttpContext, UserId, Email, Code);
        }
    }
}
