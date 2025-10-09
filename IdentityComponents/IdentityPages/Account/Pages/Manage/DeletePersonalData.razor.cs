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
    [Route(IdentityConst.IdentityRoute.AccountManage.DeletePersonalData)]
    public partial class DeletePersonalData : ComponentBase
    {
        [Inject]
        public IAccountManager AccountManager { get; set; }

        private ApplicationUser user = default!;
        private bool requirePassword;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private DeletePersonalDataInputModel Input { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            Input ??= new();
            (ApplicationUser user, bool requirePassword) initValue = await AccountManager.DeletePersonalDataOnInitializedAsync(HttpContext);
            user = initValue.user;
            requirePassword = initValue.requirePassword;
        }

        private async Task OnValidSubmitAsync()
        {
            await AccountManager.DeletePersonalDataOnValidSubmitAsync(user, requirePassword, Input);
        }
    }
}
