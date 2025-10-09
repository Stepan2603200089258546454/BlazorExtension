using IdentityAbstractions.FormsModels;
using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Pages
{
    [Route(IdentityConst.IdentityRoute.Account.ExternalLogin)]
    public partial class ExternalLogin : ComponentBase
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        private ExternalLoginInfo? externalLoginInfo;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private EmailInputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? RemoteError { get; set; }

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        [SupplyParameterFromQuery]
        private string? Action { get; set; }

        private string? ProviderDisplayName => externalLoginInfo?.ProviderDisplayName;

        protected override async Task OnInitializedAsync()
        {
            externalLoginInfo = await UserManager.ExternalLoginPageAsync(HttpContext, RemoteError, Action, ReturnUrl, Input);
        }

        private async Task OnValidSubmitAsync()
        {
            await UserManager.OnExternalLoginValidSubmitAsync(HttpContext, externalLoginInfo, ReturnUrl, Input);
        }
    }
}
