using IdentityAbstractions.Interfaces;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Shared
{
    public partial class ManageNavMenu : ComponentBase
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        private bool hasExternalLogins;

        protected override async Task OnInitializedAsync()
        {
            hasExternalLogins = (await UserManager.GetExternalAuthenticationSchemesAsync()).Any();
        }
    }
}
