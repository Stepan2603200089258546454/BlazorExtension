using IdentityAbstractions.Interfaces;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.IdentityExtensions
{
    public sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager, IIdentityRedirectManager redirectManager)
    {
        public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                redirectManager.RedirectToWithStatus("Account/InvalidUser",
                    $"Ошибка: невозможно загрузить пользователя с идентификатором '{userManager.GetUserId(context.User)}'.", context);
            }

            return user;
        }
    }
}
