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
    [Route(IdentityConst.IdentityRoute.AccountManage.Email)]
    public partial class Email : ComponentBase
    {
        [Inject]
        public IAccountManager AccountManager { get; set; }

        private string? message;
        private ApplicationUser user = default!;
        private string? email;
        private bool isEmailConfirmed;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm(FormName = "change-email")]
        private NewEmailInputModel Input { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            (ApplicationUser user, string? email, bool isEmailConfirmed) initValue = await AccountManager.EmailOnInitializedAsync(HttpContext);
            user = initValue.user;
            email = initValue.email;
            isEmailConfirmed = initValue.isEmailConfirmed;

            Input.NewEmail ??= email;
        }

        private async Task OnValidSubmitAsync()
        {
            await AccountManager.EmailOnValidSubmitAsync(user, Input, email);
        }

        private async Task OnSendEmailVerificationAsync()
        {
            await AccountManager.EmailOnSendEmailVerificationAsync(user, email);
        }
    }
}
