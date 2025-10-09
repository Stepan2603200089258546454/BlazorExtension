using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Pages.Manage
{
    [Route(IdentityConst.IdentityRoute.AccountManage.TwoFactorAuthentication)]
    public partial class TwoFactorAuthentication : ComponentBase
    {
        [Inject]
        public IAccountManager AccountManager { get; set; }

        private bool canTrack;
        private bool hasAuthenticator;
        private int recoveryCodesLeft;
        private bool is2faEnabled;
        private bool isMachineRemembered;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var res = await AccountManager.TwoFactorAuthenticationOnInitializedAsync(HttpContext);
            canTrack = res.canTrack;
            hasAuthenticator = res.hasAuthenticator;
            is2faEnabled = res.is2faEnabled;
            isMachineRemembered = res.isMachineRemembered;
            recoveryCodesLeft = res.recoveryCodesLeft;
        }

        private async Task OnSubmitForgetBrowserAsync()
        {
            await AccountManager.TwoFactorAuthenticationOnSubmitForgetBrowserAsync(HttpContext);
        }
    }
}
