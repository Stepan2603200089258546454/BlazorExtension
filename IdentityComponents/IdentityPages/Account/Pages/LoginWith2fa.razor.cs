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
    /// Страница входа через приложение аутентификатор
    /// </summary>
    [Route(IdentityConst.IdentityRoute.Account.LoginWith2fa)]
    public partial class LoginWith2fa : ComponentBase
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        private ApplicationUser user = default!;

        [SupplyParameterFromForm]
        private TwoFactorCodeInputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        [SupplyParameterFromQuery]
        private bool RememberMe { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // Убедитесь, что пользователь сначала прошел через экран ввода имени пользователя и пароля
            user = await UserManager.GetTwoFactorAuthenticationUserAsync();
        }

        private async Task OnValidSubmitAsync()
        {
            await UserManager.OnLoginWith2faValidSubmitAsync(user, Input, RememberMe, ReturnUrl);
        }
    }
}
