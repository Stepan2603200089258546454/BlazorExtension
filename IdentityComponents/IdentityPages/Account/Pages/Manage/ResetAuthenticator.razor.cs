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
    [Route(IdentityConst.IdentityRoute.AccountManage.ResetAuthenticator)]
    public partial class ResetAuthenticator : ComponentBase
    {
        [Inject]
        public IAccountManager AccountManager { get; set; }

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        private async Task OnSubmitAsync()
        {
            await AccountManager.ResetAuthenticatorOnSubmitAsync(HttpContext);
        }
    }
}
