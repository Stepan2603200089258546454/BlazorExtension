using IdentityAbstractions.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Shared
{
    public partial class ExternalLoginPicker : ComponentBase
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        private IEnumerable<AuthenticationScheme> externalLogins = [];

        protected override async Task OnInitializedAsync()
        {
            externalLogins = await UserManager.GetExternalAuthenticationSchemesAsync();
        }
    }
}
