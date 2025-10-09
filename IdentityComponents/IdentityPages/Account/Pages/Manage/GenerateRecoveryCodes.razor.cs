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
    [Route(IdentityConst.IdentityRoute.AccountManage.GenerateRecoveryCodes)]
    public partial class GenerateRecoveryCodes : ComponentBase
    {
        [Inject]
        public IAccountManager AccountManager { get; set; }

        private ApplicationUser user = default!;
        private IEnumerable<string>? recoveryCodes;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            user = await AccountManager.GenerateRecoveryCodesOnInitializedAsync(HttpContext);
        }

        private async Task OnSubmitAsync()
        {
            recoveryCodes = await AccountManager.GenerateRecoveryCodesOnSubmitAsync(user);
        }
    }
}
