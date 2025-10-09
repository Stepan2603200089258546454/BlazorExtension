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
    /// Страница с подтверждением регистрации
    /// </summary>
    [Route(IdentityConst.IdentityRoute.Account.RegisterConfirmation)]
    public partial class RegisterConfirmation : ComponentBase
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        private string? emailConfirmationLink;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromQuery]
        private string? Email { get; set; }

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            emailConfirmationLink = await UserManager.RegisterConfirmationPageAsync(HttpContext, Email, ReturnUrl);
        }
    }
}
