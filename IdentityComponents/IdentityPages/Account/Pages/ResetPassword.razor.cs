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
    [Route(IdentityConst.IdentityRoute.Account.ResetPassword)]
    public partial class ResetPassword : ComponentBase
    {
        [Inject]
        public IUserManager UserManager { get; set; }
        
        [SupplyParameterFromForm]
        private ResetPasswordInputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? Code { get; set; }

        protected override void OnInitialized()
        {
            UserManager.OnResetPasswordInitialized(Input, Code);
        }

        private async Task OnValidSubmitAsync()
        {
            await UserManager.OnResetPasswordValidSubmitAsync(Input);
        }
    }
}
