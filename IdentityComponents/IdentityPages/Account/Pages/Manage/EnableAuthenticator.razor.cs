using IdentityAbstractions.FormsModels;
using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Pages.Manage
{
    [Route(IdentityConst.IdentityRoute.AccountManage.EnableAuthenticator)]
    public partial class EnableAuthenticator : ComponentBase
    {
        [Inject]
        public IAccountManager AccountManager { get; set; }

        private ApplicationUser user = default!;
        private string? sharedKey;
        private string? authenticatorUri;
        private IEnumerable<string>? recoveryCodes;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private EnableAuthenticatorInputModel Input { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            (ApplicationUser user, string sharedKey, string authenticatorUri) res = await AccountManager.EnableAuthenticatorOnInitializedAsync(HttpContext);
            user = res.user;
            sharedKey = res.sharedKey;
            authenticatorUri = res.authenticatorUri;
        }

        private async Task OnValidSubmitAsync()
        {
            recoveryCodes = await AccountManager.EnableAuthenticatorOnValidSubmitAsync(HttpContext, Input, user);
        }
    }
}
