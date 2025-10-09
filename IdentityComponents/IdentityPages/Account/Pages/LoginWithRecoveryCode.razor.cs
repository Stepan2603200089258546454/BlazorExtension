using IdentityAbstractions.FormsModels;
using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Pages
{
    /// <summary>
    /// Страница входа по коду восстановления
    /// </summary>
    [Route(IdentityConst.IdentityRoute.Account.LoginWithRecoveryCode)]
    public partial class LoginWithRecoveryCode : ComponentBase
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        private ApplicationUser user = default!;

        [SupplyParameterFromForm]
        private RecoveryCodeInputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // Убедитесь, что пользователь сначала прошел через экран ввода имени пользователя и пароля
            user = await UserManager.GetTwoFactorAuthenticationUserAsync();
        }

        private async Task OnValidSubmitAsync()
        {
            await UserManager.OnLoginWithRecoveryCodeValidSubmitAsync(user, Input, ReturnUrl);
        }
    }
}
