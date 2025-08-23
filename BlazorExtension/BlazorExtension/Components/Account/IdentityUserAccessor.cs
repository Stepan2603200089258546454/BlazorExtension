using DataContext.IdentityModels;
using Microsoft.AspNetCore.Identity;

namespace BlazorExtension.Components.Account
{
    internal sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager)
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
