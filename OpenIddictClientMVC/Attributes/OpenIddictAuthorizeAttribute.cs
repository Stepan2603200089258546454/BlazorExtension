using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenIddict.Client.AspNetCore;

namespace OpenIddictClientMVC.Attributes
{
    /// <summary>
    /// Самописный атрибут который ещё смотрит когда истекает токен авторизации
    /// </summary>
    public class OpenIddictAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter, IAsyncAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Проверка аутентификации
            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Проверка ролей (если указаны)
            if (!string.IsNullOrEmpty(Roles))
            {
                var roles = Roles.Split(',');
                if (!roles.Any(user.IsInRole))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            string redirectPoint = "~/relogin";
            PathString returnUrl = context.HttpContext.Request.Path;
            string redirectUrl = $"{redirectPoint}?returnUrl={Uri.EscapeDataString(returnUrl)}";

            // узнать дату окончания основного токена и перенаправить на перелогирование
            var expiresAtString = context.HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate).Result;
            if (DateTime.TryParse(expiresAtString, out var expiresAt))
            {
                if (expiresAt <= DateTime.UtcNow.AddMinutes(-5)) //дата окончания меньше или равно сейчас - 5 минут (просрочка либо истечет в течении 5 минут)
                {
                    context.Result = new RedirectResult(redirectUrl);
                }
            }
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Асинхронная проверка авторизации
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Проверка ролей (если указаны)
            if (!string.IsNullOrEmpty(Roles))
            {
                var roles = Roles.Split(',');
                if (!roles.Any(user.IsInRole))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            string redirectPoint = "~/relogin";
            PathString returnUrl = context.HttpContext.Request.Path;
            string redirectUrl = $"{redirectPoint}?returnUrl={Uri.EscapeDataString(returnUrl)}";

            // узнать дату окончания основного токена и перенаправить на перелогирование
            var expiresAtString = await context.HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate);
            if (DateTime.TryParse(expiresAtString, out var expiresAt))
            {
                if (expiresAt <= DateTime.UtcNow.AddMinutes(-5)) //дата окончания меньше или равно сейчас - 5 минут (просрочка либо истечет в течении 5 минут)
                {
                    context.Result = new RedirectResult(redirectUrl);
                }
            }
        }
    }
}
