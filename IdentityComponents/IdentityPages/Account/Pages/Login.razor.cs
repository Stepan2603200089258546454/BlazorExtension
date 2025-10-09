using IdentityAbstractions.FormsModels;
using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Pages
{
    /// <summary>
    /// Страница входа
    /// </summary>
    [Route(IdentityConst.IdentityRoute.Account.Login)]
    public partial class Login : ComponentBase
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private LoginInputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        private string UrlAccountRegister => UserManager.GetUrlAccountRegister(ReturnUrl);

        protected override async Task OnInitializedAsync()
        {
            await UserManager.IsGetSignOutAsync(HttpContext);
        }

        public async Task LoginUser()
        {
            await UserManager.LoginUser(Input, ReturnUrl);
        }
    }
}
