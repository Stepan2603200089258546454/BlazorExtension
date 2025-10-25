using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenIddict.Client.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using OpenIddictAbstractions.Constants;

namespace OpenIddictClient.Attributes
{
    /// <summary>
    /// Самописный атрибут который ещё смотрит когда истекает токен авторизации
    /// </summary>
    public class OpenIddictAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Асинхронная проверка авторизации
            ClaimsPrincipal user = context.HttpContext.User;

            if ((user.Identity?.IsAuthenticated ?? true) == false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Проверка ролей (если указаны)
            if (string.IsNullOrEmpty(Roles) == false)
            {
                string[] roles = Roles.Split(',');
                if (roles.Any(user.IsInRole) == false)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            PathString returnUrl = context.HttpContext.Request.Path;
            string redirectUrl = $"{OpenIddictConst.Route.OppenIddictClient.ReloginEndpoint}?returnUrl={Uri.EscapeDataString(returnUrl)}";

            // узнать дату окончания основного токена и перенаправить на перелогирование
            string? expiresAtString = await context.HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate);
            if (DateTime.TryParse(expiresAtString, out DateTime expiresAt))
            {
                if (expiresAt <= DateTime.Now.AddMinutes(-5)) //дата окончания меньше или равно сейчас - 5 минут (просрочка либо истечет в течении 5 минут)
                {
                    context.Result = new RedirectResult(redirectUrl);
                }
            }
        }
    }
}
