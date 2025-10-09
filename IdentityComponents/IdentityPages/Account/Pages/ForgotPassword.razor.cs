using IdentityAbstractions.FormsModels;
using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Pages
{
    /// <summary>
    /// Страница сброса пароля
    /// </summary>
    [Route(IdentityConst.IdentityRoute.Account.ForgotPassword)]
    public partial class ForgotPassword : ComponentBase
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        [SupplyParameterFromForm]
        private EmailInputModel Input { get; set; } = new();

        private async Task OnValidSubmitAsync()
        {
            await UserManager.OnForgotPasswordValidSubmitAsync(Input);
        }
    }
}
