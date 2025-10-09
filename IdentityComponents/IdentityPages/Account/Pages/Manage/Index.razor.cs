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
    [Route(IdentityConst.IdentityRoute.AccountManage.Index)]
    public partial class Index : ComponentBase
    {
        [Inject]
        public IAccountManager AccountManager { get; set; }

        private ApplicationUser user = default!;
        private string? username;
        private string? phoneNumber;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private ManageIndexInputModel Input { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var res = await AccountManager.ManageIndexOnInitializedAsync(HttpContext);
            user = res.user;
            username = res.username;
            phoneNumber = res.phoneNumber;

            Input.PhoneNumber ??= phoneNumber;
        }

        private async Task OnValidSubmitAsync()
        {
            await AccountManager.ManageIndexOnValidSubmitAsync(HttpContext, Input, user, phoneNumber);
        }
    }
}
