using IdentityAbstractions.FormsModels;
using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Server.AspNetCore;
using OpenIddictAbstractions.Constants;
using OpenIddictAbstractions.Models;
using OpenIddictServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace OpenIddictServer.Controllers
{
    public class OpenIddictServerController : Controller
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictAuthorizationManager _authorizationManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public OpenIddictServerController(
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictAuthorizationManager authorizationManager,
            IOpenIddictScopeManager scopeManager,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _applicationManager = applicationManager;
            _authorizationManager = authorizationManager;
            _scopeManager = scopeManager;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        /// <summary>
        /// Получить объект запроса OpenIddict
        /// </summary>
        private OpenIddictRequest GetRequest()
        {
            return HttpContext.GetOpenIddictServerRequest() ?? throw new InvalidOperationException("Запрос OpenID Connect не может быть получен.");
        }
        /// <summary>
        /// Получить объект приложения OpenIddict
        /// </summary>
        private async Task<OpenIddictEntityFrameworkCoreApplication> GetApplicationAsync(OpenIddictRequest request)
        {
            return await _applicationManager.FindByClientIdAsync(request.ClientId!) as OpenIddictEntityFrameworkCoreApplication ??
                throw new InvalidOperationException("Подробная информация о клиентском приложении не найдена.");
        }
        /// <summary>
        /// Поучить список авторизаций пользователя (активных)
        /// </summary>
        private async Task<IList<OpenIddictEntityFrameworkCoreAuthorization?>> GetAuthorizationsAsync(ApplicationUser user, OpenIddictEntityFrameworkCoreApplication application, OpenIddictRequest request)
        {
            List<object> baseList = await _authorizationManager.FindAsync(
                    subject: await _userManager.GetUserIdAsync(user),
                    client: await _applicationManager.GetIdAsync(application),
                    status: Statuses.Valid,
                    type: AuthorizationTypes.Permanent,
                    scopes: request.GetScopes())
                .ToListAsync();
            List<OpenIddictEntityFrameworkCoreAuthorization?> castList = baseList
                .Select(x => x as OpenIddictEntityFrameworkCoreAuthorization)
                .ToList();
            return baseList.Count == castList.Count ? castList : throw new InvalidOperationException("Неудачное преобразование списка авторизаций");
        }
        /// <summary>
        /// Получить список всех авторизаций пользователя
        /// </summary>
        private async Task<IList<OpenIddictEntityFrameworkCoreAuthorization?>> GetAuthorizationsAsync(ApplicationUser user, OpenIddictEntityFrameworkCoreApplication application)
        {
            List<object> baseList = await _authorizationManager.FindAsync(
                    subject: await _userManager.GetUserIdAsync(user),
                    client: await _applicationManager.GetIdAsync(application),
                    status: null,
                    type: null,
                    scopes: null,
                    cancellationToken: default)
                .ToListAsync();
            List<OpenIddictEntityFrameworkCoreAuthorization?> castList = baseList
                .Select(x => x as OpenIddictEntityFrameworkCoreAuthorization)
                .ToList();
            return baseList.Count == castList.Count ? castList : throw new InvalidOperationException("Неудачное преобразование списка авторизаций");
        }
        /// <summary>
        /// Создать постоянную авторизацию, чтобы избежать необходимости явного согласия 
        /// для будущих запросов авторизации или токенов, содержащих те же области действия.
        /// </summary>
        private async Task<OpenIddictEntityFrameworkCoreAuthorization?> CreatePermanentDefaultAuthorizationAsync(ClaimsIdentity identity, ApplicationUser user, OpenIddictEntityFrameworkCoreApplication application)
        {
            return await _authorizationManager.CreateAsync(
                identity: identity,
                subject: await _userManager.GetUserIdAsync(user),
                client: (await _applicationManager.GetIdAsync(application))!,
                type: AuthorizationTypes.Permanent,
                scopes: identity.GetScopes()) as OpenIddictEntityFrameworkCoreAuthorization;
        }


        /// <summary>
        /// Форма согласия на предоставление данных приложению
        /// </summary>
        [HttpGet(OpenIddictConst.Route.OppenIddictServer.AuthorizationEndpoint)]
        [HttpPost(OpenIddictConst.Route.OppenIddictServer.AuthorizationEndpoint)]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            OpenIddictRequest request = GetRequest();

            // Попробуйте извлечь принципала пользователя, сохраненного в файле cookie аутентификации, и перенаправить
            // агента пользователя на страницу входа (или к внешнему поставщику) в следующих случаях:
            //
            // - Если принципала пользователя невозможно извлечь или файл cookie слишком старый.
            // - Если клиентским приложением было указано prompt=login.
            // - Если клиентским приложением было указано max_age=0 (max_age=0 эквивалентно prompt=login).
            // - Если был предоставлен параметр max_age, а файл cookie аутентификации не считается достаточно «свежим».
            //
            // Для сценариев, в которых обработчик аутентификации по умолчанию, настроенный в параметрах аутентификации ASP.NET Core
            //, здесь можно указать определенную схему.
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            
            if (result is not { Succeeded: true } ||
                ((request.HasPromptValue(PromptValues.Login) || request.MaxAge is 0 ||
                 (request.MaxAge is not null && result.Properties?.IssuedUtc is not null &&
                  TimeProvider.System.GetUtcNow() - result.Properties.IssuedUtc > TimeSpan.FromSeconds(request.MaxAge.Value))) &&
                TempData["IgnoreAuthenticationChallenge"] is null or false))
            {
                // Если клиентское приложение запросило аутентификацию без подсказок,
                // вернуть ошибку, указывающую на то, что пользователь не вошел в систему.
                if (request.HasPromptValue(PromptValues.None))
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Пользователь не вошел в систему."
                        }));
                }
                // Чтобы избежать бесконечных перенаправлений конечная точка входа
                // -> конечная точка авторизации, используется специальная временная запись данных,
                // позволяющая пропустить вызов, если пользовательский агент уже был перенаправлен на конечную точку входа.
                //
                // Примечание: этот флаг не гарантирует, что пользователь согласился на повторную аутентификацию. Если такая гарантия
                // необходима, существующий файл cookie аутентификации ДОЛЖЕН быть удален И отозван
                // (например, с помощью функции штампа безопасности ASP.NET Core
                // Identity с чрезвычайно коротким периодом повторной проверки) перед запуском
                // вызова для перенаправления пользовательского агента на конечную точку входа.
                TempData["IgnoreAuthenticationChallenge"] = true;
                // Для сценариев, где обработчик вызовов по умолчанию, настроенный в параметрах аутентификации ASP.NET Core, 
                // не должен использоваться, здесь можно указать конкретную схему.
                return Challenge(new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType ? Request.Form : Request.Query)
                });
            }

            // Получить профиль вошедшего в систему пользователя.
            ApplicationUser user = await _userManager.GetUserAsync(result.Principal) ??
                throw new InvalidOperationException("Данные пользователя не могут быть восстановлены.");

            // Извлекаем данные приложения из базы данных.
            OpenIddictEntityFrameworkCoreApplication application = await GetApplicationAsync(request);

            // Извлечь постоянные авторизации, связанные с пользователем и вызывающим клиентским приложением.
            IList<OpenIddictEntityFrameworkCoreAuthorization?> authorizations = await GetAuthorizationsAsync(user, application, request);

            switch (await _applicationManager.GetConsentTypeAsync(application))
            {
                // Если согласие внешнее (например, когда авторизации предоставлены системным администратором),
                // немедленно вернуть ошибку, если авторизация не найдена в базе данных.
                case ConsentTypes.External when authorizations.Count is 0:
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "Вошедшему в систему пользователю не разрешен доступ к этому клиентскому приложению."
                        }));

                // Если согласие неявное или если авторизация была найдена,
                // вернуть ответ авторизации без отображения формы согласия.
                case ConsentTypes.Implicit:
                case ConsentTypes.External when authorizations.Count is not 0:
                case ConsentTypes.Explicit when authorizations.Count is not 0 && !request.HasPromptValue(PromptValues.Consent):
                    // Создайте идентификацию на основе утверждений, которая будет использоваться OpenIddict для генерации токенов.
                    ClaimsIdentity identity = new ClaimsIdentity(
                        authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                        nameType: Claims.Name,
                        roleType: Claims.Role);

                    // Добавьте утверждения, которые будут сохранены в токенах.
                    identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                            .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                            .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                            .SetClaim(Claims.PreferredUsername, await _userManager.GetUserNameAsync(user))
                            .SetClaims(Claims.Role, [.. (await _userManager.GetRolesAsync(user))]);

                    // Примечание: в этом примере предоставленные области соответствуют запрошенной области
                    // но вы можете разрешить пользователю снять отметку с определенных областей.
                    // Для этого просто ограничьте список областей перед вызовом SetScopes.
                    identity.SetScopes(request.GetScopes());
                    identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

                    // Автоматически создать постоянную авторизацию, чтобы избежать необходимости явного согласия
                    // для будущих запросов авторизации или токенов, содержащих те же области действия.
                    OpenIddictEntityFrameworkCoreAuthorization? authorization = authorizations.LastOrDefault();
                    authorization ??= await _authorizationManager.CreateAsync(
                        identity: identity,
                        subject: await _userManager.GetUserIdAsync(user),
                        client: (await _applicationManager.GetIdAsync(application))!,
                        type: AuthorizationTypes.Permanent,
                        scopes: identity.GetScopes()) as OpenIddictEntityFrameworkCoreAuthorization;

                    identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));
                    identity.SetDestinations(DestinationsHelpers.GetDestinations);

                    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // На данный момент авторизация в базе данных не найдена и должна быть возвращена ошибка
                // если клиентское приложение указало prompt=none в запросе авторизации.
                case ConsentTypes.Explicit when request.HasPromptValue(PromptValues.None):
                case ConsentTypes.Systematic when request.HasPromptValue(PromptValues.None):
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "Требуется интерактивное согласие пользователя."
                        }));

                // Во всех остальных случаях предоставьте форму согласия.
                default:
                    return View(new AuthorizeViewModel
                    {
                        ApplicationName = await _applicationManager.GetLocalizedDisplayNameAsync(application),
                        Scope = request.Scope
                    });
            }
        }
        /// <summary>
        /// Согласие с формы согласия
        /// </summary>
        [Authorize]
        [FormValueRequired("submit.Accept")]
        [HttpPost(OpenIddictConst.Route.OppenIddictServer.AuthorizationEndpoint)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept()
        {
            OpenIddictRequest request = GetRequest(); 

            // Получить профиль вошедшего в систему пользователя.
            ApplicationUser user = await _userManager.GetUserAsync(User) ??
                throw new InvalidOperationException("Данные пользователя не могут быть восстановлены.");

            // Извлекаем данные приложения из базы данных.
            OpenIddictEntityFrameworkCoreApplication application = await GetApplicationAsync(request);

            // Извлечь постоянные авторизации, связанные с пользователем и вызывающим клиентским приложением.
            IList<OpenIddictEntityFrameworkCoreAuthorization?> authorizations = await GetAuthorizationsAsync(user, application, request);

            // Примечание: та же проверка уже выполняется в другом действии, но повторяется
            // здесь, чтобы гарантировать, что злонамеренный пользователь не сможет злоупотребить этой конечной точкой, поддерживающей только POST, и
            // заставить ее вернуть допустимый ответ без внешней авторизации.
            if (authorizations.Count is 0 && await _applicationManager.HasConsentTypeAsync(application, ConsentTypes.External))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Вошедшему в систему пользователю не разрешен доступ к этому клиентскому приложению."
                    }));
            }

            // Создайте идентификацию на основе утверждений, которая будет использоваться OpenIddict для генерации токенов.
            ClaimsIdentity identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            // Добавьте утверждения, которые будут сохранены в токенах.
            identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                    .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                    .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                    .SetClaim(Claims.PreferredUsername, await _userManager.GetUserNameAsync(user))
                    .SetClaims(Claims.Role, [.. (await _userManager.GetRolesAsync(user))]);

            // Примечание: в этом примере предоставленные области соответствуют запрошенной области
            // но вы можете разрешить пользователю снять отметку с определенных областей.
            // Для этого просто ограничьте список областей перед вызовом SetScopes.
            identity.SetScopes(request.GetScopes());
            identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

            // Автоматически создать постоянную авторизацию, чтобы избежать необходимости явного согласия
            // для будущих запросов авторизации или токенов, содержащих те же области действия.
            OpenIddictEntityFrameworkCoreAuthorization? authorization = authorizations.LastOrDefault();
            authorization ??= await CreatePermanentDefaultAuthorizationAsync(identity, user, application);

            identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));
            identity.SetDestinations(DestinationsHelpers.GetDestinations);

            // Возврат SignInResult попросит OpenIddict выдать соответствующие токены доступа/идентификации.
            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        /// <summary>
        /// Отказ с формы согласия
        /// </summary>
        [Authorize]
        [FormValueRequired("submit.Deny")]
        [HttpPost(OpenIddictConst.Route.OppenIddictServer.AuthorizationEndpoint)]
        [ValidateAntiForgeryToken]
        public IActionResult Deny()
        {
            // Уведомить OpenIddict о том, что владелец ресурса отклонил разрешение на авторизацию
            // для перенаправления пользовательского агента в клиентское приложение с использованием соответствующего response_mode.
            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        /// <summary>
        /// Форма выхода из приложения
        /// </summary>
        [HttpGet(OpenIddictConst.Route.OppenIddictServer.EndSessionEndpoint)]
        public IActionResult Logout()
        {
            return View();
        }
        /// <summary>
        /// Согласие с формы выхода
        /// </summary>
        [ActionName(nameof(Logout))]
        [HttpPost(OpenIddictConst.Route.OppenIddictServer.EndSessionEndpoint)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            OpenIddictRequest request = GetRequest();
            // Получить профиль вошедшего в систему пользователя.
            ApplicationUser user = await _userManager.GetUserAsync(User) ??
                throw new InvalidOperationException("Данные пользователя не могут быть восстановлены.");
            // Извлекаем данные приложения из базы данных.
            OpenIddictEntityFrameworkCoreApplication application = await GetApplicationAsync(request);
            // Отозвать авторизации, связанные с пользователем и вызывающим клиентским приложением.
            long authorizationsCount = await _authorizationManager.RevokeAsync(
                subject: await _userManager.GetUserIdAsync(user),
                client: await _applicationManager.GetIdAsync(application),
                status: Statuses.Valid,
                type: null,
                cancellationToken: default);
            // Запросить ASP.NET Core Identity удалить локальные и внешние файлы cookie, созданные
            // когда пользовательский агент перенаправляется от внешнего поставщика удостоверений
            // после успешного прохождения аутентификации (например, Google или Facebook).
            await _signInManager.SignOutAsync();
            // Возврат SignOutResult попросит OpenIddict перенаправить агента пользователя
            // на post_logout_redirect_uri, указанный клиентским приложением, или на
            // RedirectUri, указанный в свойствах аутентификации, если ничего не было задано.
            return SignOut(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = "/"
                });
        }
        /// <summary>
        /// Работа с токенами
        /// </summary>
        [HttpPost(OpenIddictConst.Route.OppenIddictServer.TokenEndpoint)]
        [IgnoreAntiforgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            // Получим запрос
            OpenIddictRequest request = GetRequest();
            // Сюда придут приложения которым нужно подтвердить личность человека
            if (request.IsAuthorizationCodeGrantType())
            {
                // Извлечь принципал утверждений, сохраненный в коде авторизации/токене обновления.
                AuthenticateResult result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // Извлечь профиль пользователя, соответствующий коду авторизации/токену обновления.
                ApplicationUser? user = await _userManager.FindByIdAsync(result.Principal!.GetClaim(Claims.Subject)!);
                if (user is null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Токен больше не действителен."
                        }));
                }

                // Убедитесь, что пользователю по-прежнему разрешено входить в систему.
                if (await _signInManager.CanSignInAsync(user) == false)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Пользователю больше не разрешено входить в систему."
                        }));
                }

                ClaimsIdentity identity = new ClaimsIdentity(result.Principal!.Claims,
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role);

                // Переопределить пользовательские утверждения, присутствующие в принципале, в случае, если они
                // изменились с момента выдачи кода авторизации/токена обновления.
                identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                        .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                        .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                        .SetClaim(Claims.PreferredUsername, await _userManager.GetUserNameAsync(user))
                        .SetClaims(Claims.Role, [.. (await _userManager.GetRolesAsync(user))]);

                identity.SetDestinations(DestinationsHelpers.GetDestinations);

                // Возврат SignInResult попросит OpenIddict выдать соответствующие токены доступа/идентификации.
                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            // Сюда придут приложения которым нужно Обновить токен
            if (request.IsRefreshTokenGrantType())
            {
                // Извлечь принципал утверждений, сохраненный в коде авторизации/токене обновления.
                AuthenticateResult result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                // Получаем приложение которое к нам приходит
                // Примечание: учетные данные клиента автоматически проверяются OpenIddict:
                // если client_id или client_secret недействительны, это действие не будет вызвано.
                OpenIddictEntityFrameworkCoreApplication application = await GetApplicationAsync(request);

                // Извлечь профиль пользователя, соответствующий коду авторизации/токену обновления.
                ApplicationUser? user = await _userManager.FindByIdAsync(result.Principal!.GetClaim(Claims.Subject)!);
                if (user is null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Токен больше не действителен."
                        }));
                }
                // Извлекаем авторизации для пользователя
                IList<OpenIddictEntityFrameworkCoreAuthorization?> autorize = await GetAuthorizationsAsync(user, application);
                // проверим что есть хоть одна не отозванная
                if (autorize.Any(x => x.Status == Statuses.Valid) == false)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Вход отозван."
                        }));
                }

                // Убедитесь, что пользователю по-прежнему разрешено входить в систему.
                if (await _signInManager.CanSignInAsync(user) == false)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Пользователю больше не разрешено входить в систему."
                        }));
                }

                ClaimsIdentity identity = new ClaimsIdentity(result.Principal!.Claims,
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role);

                // Переопределить пользовательские утверждения, присутствующие в принципале, в случае, если они
                // изменились с момента выдачи кода авторизации/токена обновления.
                identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                        .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                        .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                        .SetClaim(Claims.PreferredUsername, await _userManager.GetUserNameAsync(user))
                        .SetClaims(Claims.Role, [.. (await _userManager.GetRolesAsync(user))]);

                identity.SetDestinations(DestinationsHelpers.GetDestinations);

                // Возврат SignInResult попросит OpenIddict выдать соответствующие токены доступа/идентификации.
                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            // сюда придут приложения которым нужно подтвердить свой доступ
            if (request.IsClientCredentialsGrantType())
            {
                // Примечание: учетные данные клиента автоматически проверяются OpenIddict:
                // если client_id или client_secret недействительны, это действие не будет вызвано.

                OpenIddictEntityFrameworkCoreApplication application = await GetApplicationAsync(request);

                // Создайте идентификацию на основе утверждений, которая будет использоваться OpenIddict для генерации токенов.
                ClaimsIdentity identity = new ClaimsIdentity(
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role);

                // Добавьте утверждения, которые будут сохранены в токенах (используйте client_id в качестве идентификатора субъекта).
                identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application));
                identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));

                // Примечание: в исходной спецификации OAuth 2.0 клиентские учетные данные предоставляют
                // не возвращают токен идентификации, что является концепцией OpenID Connect.
                //
                // Как нестандартизированное расширение, OpenIddict позволяет возвращать id_token
                // для передачи информации о клиентском приложении, когда область действия «openid»
                // предоставлена ​​(т. е. указана при вызове principal.SetScopes()). Когда область действия «openid»
                // явно не установлена, клиентскому приложению не возвращается токен идентификации.

                // Задаем список областей, предоставленных клиентскому приложению в access_token.
                identity.SetScopes(request.GetScopes());
                identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
                identity.SetDestinations(DestinationsHelpers.GetDestinations);

                return SignIn(
                    principal: new ClaimsPrincipal(identity), 
                    authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            // Сюда придет приложение которому надо зарегистрировать пользователя
            if (request.IsPasswordGrantType())
            {
                ApplicationUser? user = await _userManager.FindByNameAsync(request.Username!);
                if (user == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "Пара имя пользователя/пароль недействительна."
                        }));
                }

                // Проверьте параметры имени пользователя и пароля и убедитесь, что учетная запись не заблокирована.
                SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password!, lockoutOnFailure: true);
                if (result.Succeeded == false)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "Пара имя пользователя/пароль недействительна."
                        }));
                }

                // Создайте идентификацию на основе утверждений, которая будет использоваться OpenIddict для генерации токенов.
                ClaimsIdentity identity = new ClaimsIdentity(
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role);
                // Добавьте утверждения, которые будут сохранены в токенах.
                identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                        .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                        .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                        .SetClaim(Claims.PreferredUsername, await _userManager.GetUserNameAsync(user))
                        .SetClaims(Claims.Role, [.. (await _userManager.GetRolesAsync(user))]);
                // Примечание: в этом примере предоставленные области соответствуют запрошенной области
                // но вы можете разрешить пользователю снять отметку с определенных областей.
                // Для этого просто ограничьте список областей перед вызовом SetScopes.
                identity.SetScopes(request.GetScopes());
                identity.SetDestinations(DestinationsHelpers.GetDestinations);

                // Возврат SignInResult попросит OpenIddict выдать соответствующие токены доступа/идентификации.
                return SignIn(
                    principal: new ClaimsPrincipal(identity), 
                    authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            return BadRequest("Указанный тип гранта не поддерживается.");
        }
        /// <summary>
        /// Приложения получают нужную информацию о пользователе
        /// </summary>
        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet(OpenIddictConst.Route.OppenIddictServer.UserInfoEndpoint)]
        [HttpPost(OpenIddictConst.Route.OppenIddictServer.UserInfoEndpoint)]
        [Produces("application/json")]
        public async Task<IActionResult> UserInfo()
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(User.GetClaim(Claims.Subject)!);
            if (user == null)
            {
                return Challenge(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Указанный токен доступа привязан к учетной записи, которая больше не существует."
                    }));
            }

            Dictionary<string, object?> claims = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                // Примечание: утверждение «sub» является обязательным и должно быть включено в ответ JSON.
                [Claims.Subject] = await _userManager.GetUserIdAsync(user)
            };

            if (User.HasScope(Scopes.Email))
            {
                claims[Claims.Email] = await _userManager.GetEmailAsync(user);
                claims[Claims.EmailVerified] = await _userManager.IsEmailConfirmedAsync(user);
            }

            if (User.HasScope(Scopes.Phone))
            {
                claims[Claims.PhoneNumber] = await _userManager.GetPhoneNumberAsync(user);
                claims[Claims.PhoneNumberVerified] = await _userManager.IsPhoneNumberConfirmedAsync(user);
            }

            if (User.HasScope(Scopes.Roles))
            {
                claims[Claims.Role] = await _userManager.GetRolesAsync(user);
            }

            // Примечание: полный список стандартных утверждений, поддерживаемых спецификацией OpenID Connect
            // можно найти здесь: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

            return Ok(claims);
        }
    }
}
