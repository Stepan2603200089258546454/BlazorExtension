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
    [Route(IdentityConst.IdentityRoute.AccountManage.Disable2fa)]
    public partial class Disable2fa : ComponentBase
    {
        [Inject]
        public IAccountManager AccountManager { get; set; }

        private ApplicationUser user = default!;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            user = await AccountManager.Disable2faOnInitializedAsync(HttpContext);
        }

        private async Task OnSubmitAsync()
        {
            await AccountManager.Disable2faOnSubmitAsync(HttpContext, user);
        }
    }
}
