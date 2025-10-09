using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Pages.Manage
{
    [Route(IdentityConst.IdentityRoute.AccountManage.ExternalLogins)]
    public partial class ExternalLogins : ComponentBase
    {
        [Inject]
        public IAccountManager AccountManager { get; set; }

        private ApplicationUser user = default!;
        private IList<UserLoginInfo>? currentLogins;
        private IList<AuthenticationScheme>? otherLogins;
        private bool showRemoveButton;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private string? LoginProvider { get; set; }

        [SupplyParameterFromForm]
        private string? ProviderKey { get; set; }

        [SupplyParameterFromQuery]
        private string? Action { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var res = await AccountManager.ExternalLoginsOnInitializedAsync(HttpContext, Action);
            user = res.user;
            currentLogins = res.currentLogins;
            otherLogins = res.otherLogins;
            showRemoveButton = res.showRemoveButton;
        }

        private async Task OnSubmitAsync()
        {
            await AccountManager.ExternalLoginsOnSubmitAsync(HttpContext, user, LoginProvider, ProviderKey);
        }
    }
}
