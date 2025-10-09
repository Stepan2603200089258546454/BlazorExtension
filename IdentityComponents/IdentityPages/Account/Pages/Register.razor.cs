using IdentityAbstractions.FormsModels;
using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Pages
{
    /// <summary>
    /// Страница регистрации
    /// </summary>
    [Route(IdentityConst.IdentityRoute.Account.Register)]
    public partial class Register : ComponentBase
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        [SupplyParameterFromForm]
        private RegisterInputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        public async Task RegisterUser(EditContext editContext)
        {
            await UserManager.RegisterUser(editContext, Input, ReturnUrl);
        }
    }
}
