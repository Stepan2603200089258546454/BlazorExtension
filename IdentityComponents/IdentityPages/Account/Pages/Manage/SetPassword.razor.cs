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
    [Route(IdentityConst.IdentityRoute.AccountManage.SetPassword)]
    public partial class SetPassword : ComponentBase
    {
        [Inject]
        public IAccountManager AccountManager { get; set; }

        private ApplicationUser user = default!;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private SetPasswordInputModel Input { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            user = await AccountManager.SetPasswordOnInitializedAsync(HttpContext);
        }

        private async Task OnValidSubmitAsync()
        {
            await AccountManager.SetPasswordOnValidSubmitAsync(HttpContext, user, Input);
        }
    }
}
