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
    [Route(IdentityConst.IdentityRoute.AccountManage.ChangePassword)]
    public partial class ChangePassword : ComponentBase
    {
        [Inject]
        public IAccountManager AccountManager { get; set; }

        private ApplicationUser user = default!;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private ChangePasswordInputModel Input { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            user = await AccountManager.ChangePasswordOnInitializedAsync(HttpContext);
        }

        private async Task OnValidSubmitAsync()
        {
            await AccountManager.ChangePasswordOnValidSubmitAsync(HttpContext, user, Input);
        }
    }
}
