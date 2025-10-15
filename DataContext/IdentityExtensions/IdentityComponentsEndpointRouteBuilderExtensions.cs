using IdentityAbstractions;
using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OpenIddict.Server.AspNetCore;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace DataContext.IdentityExtensions
{
    internal static class IdentityComponentsEndpointRouteBuilderExtensions
    {
        // Эти конечные точки требуются компонентам Identity Razor, определенным в каталоге /Components/Account/Pages этого проекта.
        public static void MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
        {
            ArgumentNullException.ThrowIfNull(endpoints);

            endpoints.MapIdentityAccountEndpoints();
            endpoints.MapIdentityAccountManageEndpoints();
        }
        private static void MapIdentityAccountEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost(IdentityConst.IdentityRoute.Account.PerformExternalLogin, PerformExternalLogin)
                .ExcludeFromDescription(); 
            endpoints.MapPost(IdentityConst.IdentityRoute.Account.Logout, Logout)
                .ExcludeFromDescription();
        }
        private static void MapIdentityAccountManageEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost(IdentityConst.IdentityRoute.AccountManage.LinkExternalLogin, LinkExternalLogin)
                .RequireAuthorization()
                .ExcludeFromDescription();
            endpoints.MapPost(IdentityConst.IdentityRoute.AccountManage.DownloadPersonalData, DownloadPersonalData)
                .RequireAuthorization()
                .ExcludeFromDescription();
        }
        private static IResult PerformExternalLogin(HttpContext context, [FromServices] SignInManager<ApplicationUser> signInManager, [FromForm] string provider, [FromForm] string returnUrl)
        {
            IEnumerable<KeyValuePair<string, StringValues>> query = [
                    new("ReturnUrl", returnUrl),
                    new("Action", IdentityConst.LoginCallbackAction)];

            string redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                IdentityConst.IdentityRoute.Account.ExternalLogin,
                QueryString.Create(query));

            AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return TypedResults.Challenge(properties, [provider]);
        }
        private static async Task<IResult> Logout(ClaimsPrincipal user, [FromServices] SignInManager<ApplicationUser> signInManager, [FromForm] string returnUrl)
        {
            await signInManager.SignOutAsync();
            return TypedResults.LocalRedirect($"~/{returnUrl}");
        }
        private static async Task<IResult> LinkExternalLogin(HttpContext context, [FromServices] SignInManager<ApplicationUser> signInManager, [FromForm] string provider)
        {
            // Очистите существующий внешний cookie-файл, чтобы обеспечить чистый процесс входа в систему
            await context.SignOutAsync(IdentityConstants.ExternalScheme);

            string redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                IdentityConst.IdentityRoute.AccountManage.ExternalLogins,
                QueryString.Create("Action", IdentityConst.LinkLoginCallbackAction));

            string? userId = signInManager.UserManager.GetUserId(context.User);
            
            AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userId);
            return TypedResults.Challenge(properties, [provider]);
        }
        private static async Task<IResult> DownloadPersonalData(HttpContext context, ILoggerFactory loggerFactory, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] AuthenticationStateProvider authenticationStateProvider)
        {
            ILogger downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

            ApplicationUser? user = await userManager.GetUserAsync(context.User);
            if (user is null)
            {
                return Results.NotFound($"Не удалось загрузить пользователя с идентификатором '{userManager.GetUserId(context.User)}'.");
            }

            downloadLogger.LogInformation("Пользователь с идентификатором '{UserId}' запросил свои персональные данные.", await userManager.GetUserIdAsync(user));

            // Включать только персональные данные для загрузки
            Dictionary<string, string> personalData = new Dictionary<string, string>();
            
            IEnumerable<PropertyInfo> personalDataProps = typeof(ApplicationUser)
                .GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (PropertyInfo p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            IList<UserLoginInfo> logins = await userManager.GetLoginsAsync(user);
            foreach (UserLoginInfo l in logins)
            {
                personalData.Add($"{l.LoginProvider} ключ внешнего поставщика входа", l.ProviderKey);
            }

            personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);
            byte[] fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

            context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            return TypedResults.File(
                fileContents: fileBytes, 
                contentType: "application/json", 
                fileDownloadName: "PersonalData.json");
        }
    }
}
