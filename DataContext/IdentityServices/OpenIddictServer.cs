using DataContext.Helpers;
using IdentityAbstractions.FormsModels;
using IdentityAbstractions.Interfaces;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Server.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace DataContext.IdentityServices
{
    public class OpenIddictServer : IOpenIddictServer
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictAuthorizationManager _authorizationManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public OpenIddictServer(
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

        public async Task<(AuthorizeViewModel? Model, bool? IgnoreAuthenticationChallenge)> AuthorizeAsync(HttpContext HttpContext, bool? IgnoreAuthenticationChallenge = null)
        {
            OpenIddictRequest request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("Запрос OpenID Connect не может быть получен.");

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
            var result = await HttpContext.AuthenticateAsync();
            if (result is not { Succeeded: true } ||
                ((request.HasPromptValue(PromptValues.Login) || request.MaxAge is 0 ||
                 (request.MaxAge is not null && result.Properties?.IssuedUtc is not null &&
                  TimeProvider.System.GetUtcNow() - result.Properties.IssuedUtc > TimeSpan.FromSeconds(request.MaxAge.Value))) &&
                IgnoreAuthenticationChallenge is null or false))
            {
                // Если клиентское приложение запросило аутентификацию без подсказок,
                // вернуть ошибку, указывающую на то, что пользователь не вошел в систему.
                if (request.HasPromptValue(PromptValues.None))
                {
                    await HttpContext.ForbidAsync(
                        scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Пользователь не вошел в систему."
                        }));
                    return (null, IgnoreAuthenticationChallenge);
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
                IgnoreAuthenticationChallenge = true;

                // Для сценариев, где обработчик вызовов по умолчанию, настроенный в параметрах аутентификации ASP.NET Core, 
                // не должен использоваться, здесь можно указать конкретную схему.
                await HttpContext.ChallengeAsync(
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = HttpContext.Request.PathBase + HttpContext.Request.Path + QueryString.Create(
                        HttpContext.Request.HasFormContentType ? HttpContext.Request.Form : HttpContext.Request.Query)
                    });
                return (null, IgnoreAuthenticationChallenge);
            }

            // Получить профиль вошедшего в систему пользователя.
            ApplicationUser user = await _userManager.GetUserAsync(result.Principal) ??
                throw new InvalidOperationException("Данные пользователя не могут быть восстановлены.");

            // Извлекаем данные приложения из базы данных.
            object application = await _applicationManager.FindByClientIdAsync(request.ClientId!) ??
                throw new InvalidOperationException("Подробная информация о клиентском приложении для звонков не найдена.");

            // Извлечь постоянные авторизации, связанные с пользователем и вызывающим клиентским приложением.
            List<object> authorizations = await _authorizationManager.FindAsync(
                subject: await _userManager.GetUserIdAsync(user),
                client: await _applicationManager.GetIdAsync(application),
                status: Statuses.Valid,
                type: AuthorizationTypes.Permanent,
                scopes: request.GetScopes()).ToListAsync();

            switch (await _applicationManager.GetConsentTypeAsync(application))
            {
                // Если согласие внешнее (например, когда авторизации предоставлены системным администратором),
                // немедленно вернуть ошибку, если авторизация не найдена в базе данных.
                case ConsentTypes.External when authorizations.Count is 0:
                    await HttpContext.ForbidAsync(
                        scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "Вошедшему в систему пользователю не разрешен доступ к этому клиентскому приложению."
                        }));
                    return (null, IgnoreAuthenticationChallenge);
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
                    var authorization = authorizations.LastOrDefault();
                    authorization ??= await _authorizationManager.CreateAsync(
                        identity: identity,
                        subject: await _userManager.GetUserIdAsync(user),
                        client: (await _applicationManager.GetIdAsync(application))!,
                        type: AuthorizationTypes.Permanent,
                        scopes: identity.GetScopes());

                    identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));
                    identity.SetDestinations(GetDestinations);

                    await HttpContext.SignInAsync(
                        scheme:OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, 
                        principal: new ClaimsPrincipal(identity));
                    return (null, IgnoreAuthenticationChallenge);

                // На данный момент авторизация в базе данных не найдена и должна быть возвращена ошибка
                // если клиентское приложение указало prompt=none в запросе авторизации.
                case ConsentTypes.Explicit when request.HasPromptValue(PromptValues.None):
                case ConsentTypes.Systematic when request.HasPromptValue(PromptValues.None):
                    await HttpContext.ForbidAsync(
                        scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "Требуется интерактивное согласие пользователя."
                        }));
                    return (null, IgnoreAuthenticationChallenge);
                // Во всех остальных случаях предоставьте форму согласия.
                default:
                    return (new AuthorizeViewModel
                    {
                        ApplicationName = await _applicationManager.GetLocalizedDisplayNameAsync(application),
                        Scope = request.Scope
                    }, IgnoreAuthenticationChallenge);
            }
        }
        public async Task AcceptAsync(HttpContext HttpContext)
        {
            OpenIddictRequest request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("Запрос OpenID Connect не может быть получен.");

            // Получить профиль вошедшего в систему пользователя.
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User) ??
                throw new InvalidOperationException("Данные пользователя не могут быть восстановлены.");

            // Извлекаем данные приложения из базы данных.
            object application = await _applicationManager.FindByClientIdAsync(request.ClientId!) ??
                throw new InvalidOperationException("Подробная информация о клиентском приложении не найдена.");

            // Извлечь постоянные авторизации, связанные с пользователем и вызывающим клиентским приложением.
            List<object> authorizations = await _authorizationManager.FindAsync(
                subject: await _userManager.GetUserIdAsync(user),
                client: await _applicationManager.GetIdAsync(application),
                status: Statuses.Valid,
                type: AuthorizationTypes.Permanent,
                scopes: request.GetScopes()).ToListAsync();

            // Примечание: та же проверка уже выполняется в другом действии, но повторяется
            // здесь, чтобы гарантировать, что злонамеренный пользователь не сможет злоупотребить этой конечной точкой, поддерживающей только POST, и
            // заставить ее вернуть допустимый ответ без внешней авторизации.
            if (authorizations.Count is 0 && await _applicationManager.HasConsentTypeAsync(application, ConsentTypes.External))
            {
                await HttpContext.ForbidAsync(
                    scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Вошедшему в систему пользователю не разрешен доступ к этому клиентскому приложению."
                    }));
                return;
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
            object? authorization = authorizations.LastOrDefault();
            authorization ??= await _authorizationManager.CreateAsync(
                identity: identity,
                subject: await _userManager.GetUserIdAsync(user),
                client: (await _applicationManager.GetIdAsync(application))!,
                type: AuthorizationTypes.Permanent,
                scopes: identity.GetScopes());

            identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));
            identity.SetDestinations(GetDestinations);

            // Возврат SignInResult попросит OpenIddict выдать соответствующие токены доступа/идентификации.
            await HttpContext.SignInAsync(
                scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                principal: new ClaimsPrincipal(identity));
            return;
        }
        public async Task DenyAsync(HttpContext HttpContext)
        {
            await HttpContext.ForbidAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            return;
        }

        public async Task LogoutAsync(HttpContext HttpContext)
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("Запрос OpenID Connect не может быть получен.");
            // Получить профиль вошедшего в систему пользователя.
            var user = await _userManager.GetUserAsync(HttpContext.User) ??
                throw new InvalidOperationException("Данные пользователя не могут быть восстановлены.");
            // Извлекаем данные приложения из базы данных.
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId!) ??
                throw new InvalidOperationException("Подробная информация о клиентском приложении не найдена.");
            // Отозвать авторизации, связанные с пользователем и вызывающим клиентским приложением.
            var authorizationsCount = await _authorizationManager.RevokeAsync(
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
            await HttpContext.SignOutAsync(
                scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = "/"
                });
            return;
        }

        public async Task ExchangeAsync(HttpContext HttpContext)
        {
            // Получим запрос
            OpenIddictRequest request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("Запрос OpenID Connect не может быть получен.");
            // Сюда придут приложения которым нужно подтвердить личность человека
            if (request.IsAuthorizationCodeGrantType())
            {
                // Извлечь принципал утверждений, сохраненный в коде авторизации/токене обновления.
                var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // Извлечь профиль пользователя, соответствующий коду авторизации/токену обновления.
                var user = await _userManager.FindByIdAsync(result.Principal!.GetClaim(Claims.Subject)!);
                if (user is null)
                {
                    await HttpContext.ForbidAsync(
                        scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Токен больше не действителен."
                        }));
                    return;
                }

                // Убедитесь, что пользователю по-прежнему разрешено входить в систему.
                if (!await _signInManager.CanSignInAsync(user))
                {
                    await HttpContext.ForbidAsync(
                        scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Пользователю больше не разрешено входить в систему."
                        }));
                    return;
                }

                var identity = new ClaimsIdentity(result.Principal!.Claims,
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

                identity.SetDestinations(GetDestinations);

                // Возврат SignInResult попросит OpenIddict выдать соответствующие токены доступа/идентификации.
                await HttpContext.SignInAsync(
                    scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    principal: new ClaimsPrincipal(identity));
                return;
            }
            // Сюда придут приложения которым нужно Обновить токен
            if (request.IsRefreshTokenGrantType())
            {
                // Извлечь принципал утверждений, сохраненный в коде авторизации/токене обновления.
                var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                // Получаем приложение которое к нам приходит
                // Примечание: учетные данные клиента автоматически проверяются OpenIddict:
                // если client_id или client_secret недействительны, это действие не будет вызвано.
                var application = await _applicationManager.FindByClientIdAsync(request.ClientId!)
                    as OpenIddictEntityFrameworkCoreApplication;
                if (application == null)
                {
                    throw new InvalidOperationException("Подробную информацию о заявке в базе данных найти не удалось.");
                }

                // Извлечь профиль пользователя, соответствующий коду авторизации/токену обновления.
                var user = await _userManager.FindByIdAsync(result.Principal!.GetClaim(Claims.Subject)!);
                if (user is null)
                {
                    await HttpContext.ForbidAsync(
                        scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Токен больше не действителен."
                        }));
                    return;
                }
                // Извлекаем авторизации для пользователя
                var autorize = (await _authorizationManager.FindAsync(
                    subject: user.Id,
                    client: application.Id,
                    status: null,
                    type: null,
                    scopes: null,
                    cancellationToken: default).ToListAsync()).Select(x => x as OpenIddictEntityFrameworkCoreAuthorization);
                // проверим что есть хоть одна не отозванная
                if (autorize.Any(x => x.Status == Statuses.Valid) == false)
                {
                    await HttpContext.ForbidAsync(
                        scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Вход отозван."
                        }));
                    return;
                }

                // Убедитесь, что пользователю по-прежнему разрешено входить в систему.
                if (!await _signInManager.CanSignInAsync(user))
                {
                    await HttpContext.ForbidAsync(
                        scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Пользователю больше не разрешено входить в систему."
                        }));
                    return;
                }

                var identity = new ClaimsIdentity(result.Principal!.Claims,
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

                identity.SetDestinations(GetDestinations);

                // Возврат SignInResult попросит OpenIddict выдать соответствующие токены доступа/идентификации.
                await HttpContext.SignInAsync(
                    scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, 
                    principal: new ClaimsPrincipal(identity));
                return;
            }
            // сюда придут приложения которым нужно подтвердить свой доступ
            if (request.IsClientCredentialsGrantType())
            {
                // Примечание: учетные данные клиента автоматически проверяются OpenIddict:
                // если client_id или client_secret недействительны, это действие не будет вызвано.

                var application = await _applicationManager.FindByClientIdAsync(request.ClientId!);
                if (application == null)
                {
                    throw new InvalidOperationException("Подробную информацию о заявке в базе данных найти не удалось.");
                }

                // Создайте идентификацию на основе утверждений, которая будет использоваться OpenIddict для генерации токенов.
                var identity = new ClaimsIdentity(
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
                identity.SetDestinations(GetDestinations);

                await HttpContext.SignInAsync(
                    scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    principal: new ClaimsPrincipal(identity));
                return;
            }
            // Сюда придет приложение которому надо зарегистрировать пользователя
            if (request.IsPasswordGrantType())
            {
                var user = await _userManager.FindByNameAsync(request.Username!);
                if (user == null)
                {
                    var properties = new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Пара имя пользователя/пароль недействительна."
                    });

                    await HttpContext.ForbidAsync(
                        scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, 
                        properties: properties);
                    return;
                }

                // Проверьте параметры имени пользователя и пароля и убедитесь, что учетная запись не заблокирована.
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password!, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    var properties = new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "Пара имя пользователя/пароль недействительна."
                    });

                    await HttpContext.ForbidAsync(
                        scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, 
                        properties: properties);
                    return;
                }

                // Создайте идентификацию на основе утверждений, которая будет использоваться OpenIddict для генерации токенов.
                var identity = new ClaimsIdentity(
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
                identity.SetDestinations(GetDestinations);

                // Возврат SignInResult попросит OpenIddict выдать соответствующие токены доступа/идентификации.
                await HttpContext.SignInAsync(
                    scheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, 
                    principal: new ClaimsPrincipal(identity));
                return;
            }

            throw new InvalidOperationException("Указанный тип гранта не поддерживается.");
        }

        private static IEnumerable<string> GetDestinations(Claim claim)
        {
            // Примечание: по умолчанию утверждения НЕ включаются автоматически в токены доступа и идентификации.
            // Чтобы разрешить OpenIddict сериализовать их, необходимо прикрепить к ним назначение, которое указывает
            // следует ли их включать в токены доступа, в токены идентификации или в оба.

            switch (claim.Type)
            {
                case Claims.Name or Claims.PreferredUsername:
                    yield return Destinations.AccessToken;

                    if (claim.Subject!.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Name or Claims.Subject:
                    yield return Destinations.AccessToken;
                    yield return Destinations.IdentityToken;
                    yield break;

                case Claims.Email:
                    yield return Destinations.AccessToken;

                    if (claim.Subject!.HasScope(Scopes.Email))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (claim.Subject!.HasScope(Scopes.Roles))
                        yield return Destinations.IdentityToken;

                    yield break;

                // Никогда не включайте штамп безопасности в токены доступа и идентификации, так как это секретное значение.
                case "AspNet.Identity.SecurityStamp": yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }
    }
}
