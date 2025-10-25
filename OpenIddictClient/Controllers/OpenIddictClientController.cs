using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.Client.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using OpenIddictAbstractions.Constants;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpenIddictClient.Controllers
{
    public class OpenIddictClientController : Controller
    {
        private readonly OpenIddictClientService _clientService;

        public OpenIddictClientController(OpenIddictClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Вызывается при вызове попытки авторизоваться
        /// </summary>
        [HttpGet(OpenIddictConst.Route.OppenIddictClient.LoginEndpoint)]
        public IActionResult LogIn(string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                // Разрешайте только локальные обратные URL-адреса, чтобы предотвратить открытые атаки перенаправления.
                RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/",
                // Возвращает или задает, следует ли разрешить обновление сеанса аутентификации.
                AllowRefresh = true,
            };

            // Попросите клиентское промежуточное программное обеспечение OpenIddict перенаправить пользовательский агент поставщику удостоверений.
            return Challenge(properties, OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        }
        /// <summary>
        /// Вызывается при вызове выхода
        /// </summary>
        [HttpPost(OpenIddictConst.Route.OppenIddictClient.LogoutEndpoint), ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut(string returnUrl)
        {
            // Извлекает идентификатор, сохраненный в локальном файле cookie для проверки подлинности. Если он недоступен,
            // это означает, что пользователь уже вышел из системы локально (или еще не входил в систему).
            //
            // Для сценариев, в которых обработчик проверки подлинности по умолчанию настроен в ASP.NET Core
            // параметры аутентификации использовать не следует, здесь можно указать конкретную схему.
            var result = await HttpContext.AuthenticateAsync();
            if (result is not { Succeeded: true })
            {
                // Разрешайте только локальные обратные URL-адреса, чтобы предотвратить открытые атаки перенаправления.
                return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
            }
            // Удалите файл cookie локальной аутентификации, прежде чем запускать перенаправление на удаленный сервер.
            //
            // Для сценариев, в которых не следует использовать signouthandler по умолчанию, настроенный в ASP.NET Core
            // параметры аутентификации, можно указать здесь конкретную схему.
            await HttpContext.SignOutAsync();

            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                // Хотя это и не требуется, спецификация рекомендует отправлять id_token_hint
                // параметр, содержащий идентификационный токен, возвращаемый сервером для этого пользователя.
                [OpenIddictClientAspNetCoreConstants.Properties.IdentityTokenHint] =
                    result.Properties.GetTokenValue(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken)
            })
            {
                // Разрешайте только локальные обратные URL-адреса, чтобы предотвратить открытые атаки перенаправления.
                RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/"
            };
            // Попросите клиентское промежуточное программное обеспечение OpenIddict перенаправить пользовательский агент поставщику удостоверений.
            return SignOut(properties, OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        }
        // Примечание: этот контроллер использует одно и то же действие обратного вызова для всех провайдеров
        // но для пользователей, которые предпочитают использовать разные действия для каждого провайдера,
        // следующее действие можно разделить на отдельные действия.
        [HttpGet(OpenIddictConst.Route.OppenIddictClient.CallbackLogin + "/{provider}")]
        [HttpPost(OpenIddictConst.Route.OppenIddictClient.CallbackLogin + "/{provider}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LogInCallback(string provider)
        {
            // Извлекает данные авторизации, проверенные OpenIddict, как часть обработки обратного вызова.
            var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
            // Существует несколько стратегий для обработки обратных вызовов OAuth 2.0/OpenID Connect, каждая из которых имеет свои плюсы и минусы:
            //
            // * Прямое использование токенов для выполнения необходимых действий от имени пользователя, что подходит
            // для приложений, которым не требуется долгосрочный доступ к ресурсам пользователя или которые не хотят хранить
            // токены доступа/обновления в базе данных или в файле cookie для аутентификации (что имеет значение для безопасности).
            // Он также подходит для приложений, которым не требуется аутентификация пользователей, а требуется только выполнить
            // действия от их имени путем выполнения вызовов API с использованием токена доступа, возвращаемого удаленным сервером.
            //
            // * Сохранение внешних утверждений/токенов в базе данных (и, при необходимости, сохранение основных утверждений в файле cookie для аутентификации
            //, чтобы не нарушались ограничения по размеру файлов cookie). Для приложений, использующих идентификатор ASP.NET
            // Core, используется UserManager.API SetAuthenticationTokenAsync() может использоваться для хранения внешних токенов.
            //
            // Примечание: в этом случае рекомендуется использовать шифрование столбцов для защиты токенов в базе данных.
            //
            // * Сохранение внешних утверждений/токенов в файле cookie для аутентификации, который не требует наличия
            // пользовательская база данных, но на нее могут влиять ограничения на размер файлов cookie, установленные большинством производителей браузеров
            // (например, Safari для macOS и Safari для iOS/iPadOS устанавливают ограничение в 4 КБАЙТ для всех файлов cookie для каждого домена).
            //
            // Примечание: здесь используется именно такой подход, но внешние утверждения сначала фильтруются, чтобы сохранялись только они
            // несколько утверждений, таких как идентификатор пользователя. Такой же подход используется для хранения токенов доступа/обновления.

            // Важно: если удаленный сервер не поддерживает OpenID Connect и не предоставляет конечную точку userinfo,
            // результат.Главный.Удостоверение личности будет представлять собой неаутентифицированное удостоверение личности и не будет содержать никаких требований пользователя.
            //
            // Такие идентификаторы нельзя использовать как есть для создания файла cookie аутентификации в ASP.NET Core (поскольку
            // для стека защиты от подделок требуется, по крайней мере, указание имени для привязки файлов cookie CSRF к идентификатору пользователя), но
            // токены доступа/обновления могут быть получены с помощью result.Свойства.getTokens() для выполнения вызовов API.
            if (result is not { Succeeded: true, Principal.Identity.IsAuthenticated: true })
            {
                throw new InvalidOperationException("Внешние авторизационные данные не могут быть использованы для аутентификации.");
            }
            // Создайте идентификатор на основе внешних утверждений, который будет использоваться для создания файла cookie для аутентификации.
            var identity = new ClaimsIdentity(
                authenticationType: "ExternalLogin",
                nameType: ClaimTypes.Name,
                roleType: ClaimTypes.Role);
            // По умолчанию OpenIDdict автоматически попытается сопоставить запросы на адрес электронной почты/имя и идентификатор имени с
            // их стандартным OpenID Connect или эквивалентом для конкретного провайдера, если он доступен. При необходимости, дополнительные
            // запросы могут быть разрешены с помощью внешнего идентификатора и скопированы в конечный файл cookie для аутентификации.
            identity.SetClaim(ClaimTypes.Email, result.Principal.GetClaim(ClaimTypes.Email))
                    .SetClaim(ClaimTypes.Name, result.Principal.GetClaim(ClaimTypes.Name))
                    .SetClaim(ClaimTypes.NameIdentifier, result.Principal.GetClaim(ClaimTypes.NameIdentifier));
            // добавим роли
            identity.SetClaims(ClaimTypes.Role, result.Principal.GetClaims(Claims.Role));
            // Сохраните регистрационные данные, чтобы иметь возможность исправить их позже.
            identity.SetClaim(Claims.Private.RegistrationId, result.Principal.GetClaim(Claims.Private.RegistrationId))
                    .SetClaim(Claims.Private.ProviderName, result.Principal.GetClaim(Claims.Private.ProviderName));
            // Важно: при использовании ASP.NET Core Identity и его пользовательского интерфейса по умолчанию идентификатор, созданный в результате этого действия, 
            // не сохраняется напрямую в конечном файле cookie для аутентификации (который в Identity называется "файл cookie приложения"), а
            // в промежуточном файле cookie для аутентификации, называемом "внешний файл cookie" (конечный файл cookie для аутентификации является
            // позже создается страницей Razor ExternalLogin в Identity с помощью вызова SignInManager.ExternalLoginSignInAsync()).
            //
            // К сожалению, этот процесс не сохраняет добавленные здесь утверждения, что предотвращает передачу утверждений
            // возвращается внешним провайдером вплоть до окончательного файла cookie для проверки подлинности. Для сценариев, которые
            // требуют этого, утверждения могут быть сохранены в базе данных Identity с помощью вызова UserManager.AddClaimAsync()
            // непосредственно в этом действии или путем создания структуры страницы ExternalLogin.cshtml, которая является частью пользовательского интерфейса по умолчанию:
            // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/additional-claims#add-and-update-user-claims.
            //
            // В качестве альтернативы, если предпочтительнее передавать утверждения из "внешнего файла cookie" в "файл cookie приложения".,
            // страница ExternalLogin.cshtml по умолчанию, предоставляемая Identity, может быть преобразована для замены вызова функции
            // SignInManager.ExternalLoginSignInAsync() операцией входа вручную, которая сохранит утверждения.
            // Для сценариев, в которых создание структуры страницы ExternalLogin.cshtml неудобно, можно также использовать пользовательский SignInManager
            // с переопределенным методом SignInOrTwoFactorAsync() для настройки логики идентификации по умолчанию.
            //
            // Для получения дополнительной информации смотрите https://haacked.com/archive/2019/07/16/external-claims/ и
            // https://stackoverflow.com/questions/42660568/asp-net-core-identity-extract-and-save-external-login-tokens-and-add-claims-to-l/42670559#42670559.

            // Расчет времени жизни токена и установка в куках этого времени
            var tokenExpirationDateStr = result.Properties.GetTokens().FirstOrDefault(x => x.Name is OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate)?.Value;
            DateTimeOffset? tokenExpirationDate = null;
            if (string.IsNullOrWhiteSpace(tokenExpirationDateStr) == false)
            {
                if (DateTime.TryParse(tokenExpirationDateStr, out var tokenExpirationDate2))
                    tokenExpirationDate = tokenExpirationDate2;
            }
            // Создайте свойства аутентификации на основе свойств, которые были добавлены при запуске запроса.
            var properties = new AuthenticationProperties(result.Properties.Items)
            {
                RedirectUri = result.Properties.RedirectUri ?? "/",
                // Установите для даты создания и истечения срока действия заявки значение null, чтобы соотнести время жизни
                // результирующего файла cookie для проверки подлинности со временем жизни идентификационного токена, возвращаемого
                // сервером авторизации (если применимо). В этом случае дата истечения срока действия будет
                // автоматически вычислена обработчиком файлов cookie с использованием времени жизни, заданного в настройках.
                //
                // Приложения, которые предпочитают привязывать срок действия билета, сохраненного в файле cookie для аутентификации
                // чтобы идентификационный токен, возвращаемый поставщиком идентификационных данных, мог удалить или прокомментировать эти две строки:
                IssuedUtc = null,
                //ExpiresUtc = null,
                ExpiresUtc = tokenExpirationDate,
                // Примечание: этот флаг определяет, будет ли файл cookie для проверки подлинности, который будет возвращен
                // браузеру, обрабатываться как сеансовый файл cookie(т.е.уничтожаться при закрытии браузера)
                // или как постоянный файл cookie. В обоих случаях время действия
                // аутентификационного запроса всегда сохраняется в виде защищенных данных,
                // что предотвращает попытки злоумышленников использовать
                // файл cookie для аутентификации после истечения срока действия самого аутентификационного запроса.
                IsPersistent = true,
                AllowRefresh = true,
            };
            // При необходимости токены, возвращаемые сервером авторизации, могут быть сохранены в файле cookie для аутентификации.
            //
            // Чтобы уменьшить количество файлов cookie, неиспользуемые токены отфильтровываются перед созданием файла cookie.
            properties.StoreTokens(result.Properties.GetTokens().Where(token => token.Name is
                // Сохраните токены доступа, идентификации и обновления, возвращенные в ответе на запрос о токене, если они доступны.
                //
                // Дата истечения срока действия токена доступа также сохраняется для последующего определения
                // истек ли срок действия токена доступа, и при необходимости обновите токены заблаговременно.
                OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken or
                OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate or
                OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken or
                OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken));
            // Попросите обработчик входа по умолчанию вернуть новый файл cookie и перенаправить
            // пользовательский агент на URL-адрес возврата, сохраненный в свойствах аутентификации.
            //
            // Для сценариев, в которых обработчик входа по умолчанию настроен в ASP.NET Core
            // параметры аутентификации использовать не следует, здесь можно указать конкретную схему.
            return SignIn(new ClaimsPrincipal(identity), properties);
        }
        // Примечание: этот контроллер использует одно и то же действие обратного вызова для всех провайдеров
        // но для пользователей, которые предпочитают использовать разные действия для каждого провайдера,
        // следующее действие можно разделить на отдельные действия.
        [HttpGet(OpenIddictConst.Route.OppenIddictClient.CallbackLogout + "/{provider}")]
        [HttpPost(OpenIddictConst.Route.OppenIddictClient.CallbackLogout + "/{provider}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LogOutCallback(string provider)
        {
            // Извлекает данные, сохраненные OpenIddict в токене состояния, созданном при запуске выхода из системы.
            var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
            // В этом примере локальный файл cookie для проверки подлинности всегда удаляется перед перенаправлением агента пользователя
            // на сервер авторизации. Приложения, которые предпочитают отложить удаление локального файла cookie, могут
            // удалить соответствующий код из действия выхода из системы и удалить файл cookie для проверки подлинности в этом действии.
            return Redirect(result?.Properties?.RedirectUri ?? "/");
        }
        [HttpGet(OpenIddictConst.Route.OppenIddictClient.ReloginEndpoint)]
        public async Task<IActionResult> ReLogin(string returnUrl)
        {
            // Разрешайте только локальные обратные URL-адреса, чтобы предотвратить открытые атаки перенаправления.
            string redirectUrl = "~/login";
            if (Url.IsLocalUrl(returnUrl))
            {
                redirectUrl += "?returnUrl=" + returnUrl;
            }
            // проверим есть ли вообще аутентификация
            var result = await HttpContext.AuthenticateAsync();
            if (result is not { Succeeded: true })
            {
                return Redirect(redirectUrl);
            }
            // Для сценариев, в которых не следует использовать обработчик аутентификации по умолчанию, настроенный в ASP.NET Core
            // параметры аутентификации, можно указать здесь конкретную схему.
            string? token = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
            // узнать дату окончания основного токена и перенаправить на перелогирование
            var expiresAtString = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate);
            if (DateTime.TryParse(expiresAtString, out var expiresAt))
            {
                if (expiresAt < DateTime.UtcNow.AddMinutes(1))
                {
                    string? refreshToken = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken);
                    //если нет токена обновления отправляем авторизоваться
                    if (string.IsNullOrWhiteSpace(refreshToken) == true)
                    {
                        return Redirect(redirectUrl);
                    }
                    else
                    {
                        OpenIddictClientModels.RefreshTokenAuthenticationResult refreshResult = await _clientService.AuthenticateWithRefreshTokenAsync(new()
                        {
                            RefreshToken = refreshToken
                        });
                        if (refreshResult is not { Principal.Identity.IsAuthenticated: true })
                        {
                            throw new InvalidOperationException("Внешние авторизационные данные не могут быть использованы для аутентификации.");
                        }
                        // Создайте идентификатор на основе внешних утверждений, который будет использоваться для создания файла cookie для аутентификации.
                        var identity = new ClaimsIdentity(
                            authenticationType: "ExternalLogin",
                            nameType: ClaimTypes.Name,
                            roleType: ClaimTypes.Role);
                        // По умолчанию OpenIDdict автоматически попытается сопоставить запросы на адрес электронной почты/имя и идентификатор имени с
                        // их стандартным OpenID Connect или эквивалентом для конкретного провайдера, если он доступен. При необходимости, дополнительные
                        // запросы могут быть разрешены с помощью внешнего идентификатора и скопированы в конечный файл cookie для аутентификации.
                        identity.SetClaim(ClaimTypes.Email, refreshResult.Principal.GetClaim(ClaimTypes.Email))
                                .SetClaim(ClaimTypes.Name, refreshResult.Principal.GetClaim(ClaimTypes.Name))
                                .SetClaim(ClaimTypes.NameIdentifier, refreshResult.Principal.GetClaim(ClaimTypes.NameIdentifier));
                        // добавим роли
                        identity.SetClaims(ClaimTypes.Role, refreshResult.Principal.GetClaims(Claims.Role));
                        // Сохраните регистрационные данные, чтобы иметь возможность исправить их позже.
                        identity.SetClaim(Claims.Private.RegistrationId, refreshResult.Principal.GetClaim(Claims.Private.RegistrationId))
                                .SetClaim(Claims.Private.ProviderName, refreshResult.Principal.GetClaim(Claims.Private.ProviderName));
                        // Расчет времени жизни токена и установка в куках этого времени
                        var tokenExpirationDateStr = result.Properties.GetTokens().FirstOrDefault(x => x.Name is OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate)?.Value;
                        DateTimeOffset? tokenExpirationDate = null;
                        if (string.IsNullOrWhiteSpace(tokenExpirationDateStr) == false)
                        {
                            if (DateTime.TryParse(tokenExpirationDateStr, out var tokenExpirationDate2))
                                tokenExpirationDate = tokenExpirationDate2;
                        }
                        // Создайте свойства аутентификации на основе свойств, которые были добавлены при запуске запроса.
                        var properties = new AuthenticationProperties(result.Properties.Items)
                        {
                            RedirectUri = result.Properties.RedirectUri ?? "/",
                            // Установите для даты создания и истечения срока действия заявки значение null, чтобы соотнести время жизни
                            // результирующего файла cookie для проверки подлинности со временем жизни идентификационного токена, возвращаемого
                            // сервером авторизации (если применимо). В этом случае дата истечения срока действия будет
                            // автоматически вычислена обработчиком файлов cookie с использованием времени жизни, заданного в настройках.
                            //
                            // Приложения, которые предпочитают привязывать срок действия билета, сохраненного в файле cookie для аутентификации
                            // чтобы идентификационный токен, возвращаемый поставщиком идентификационных данных, мог удалить или прокомментировать эти две строки:
                            IssuedUtc = null,
                            //ExpiresUtc = null,
                            ExpiresUtc = tokenExpirationDate,
                            // Примечание: этот флаг определяет, будет ли файл cookie для проверки подлинности, который будет возвращен
                            // браузеру, обрабатываться как сеансовый файл cookie(т.е.уничтожаться при закрытии браузера)
                            // или как постоянный файл cookie. В обоих случаях время действия
                            // аутентификационного запроса всегда сохраняется в виде защищенных данных,
                            // что предотвращает попытки злоумышленников использовать
                            // файл cookie для аутентификации после истечения срока действия самого аутентификационного запроса.
                            IsPersistent = true,
                            AllowRefresh = true,
                        };
                        // При необходимости токены, возвращаемые сервером авторизации, могут быть сохранены в файле cookie для аутентификации.
                        //
                        // Чтобы уменьшить количество файлов cookie, неиспользуемые токены отфильтровываются перед созданием файла cookie.
                        properties.StoreTokens(
                            new[] {
                                new AuthenticationToken()
                                {
                                    Name = OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken,
                                    Value = refreshResult.AccessToken,
                                },
                                new AuthenticationToken()
                                {
                                    Name = OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessTokenExpirationDate,
                                    Value = refreshResult.AccessTokenExpirationDate?.ToString(),
                                },
                                new AuthenticationToken()
                                {
                                    Name = OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken,
                                    Value = refreshResult.IdentityToken,
                                },
                                new AuthenticationToken()
                                {
                                    Name = OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken,
                                    Value = refreshResult.RefreshToken,
                                }
                            });
                        // Попросите обработчик входа по умолчанию вернуть новый файл cookie и перенаправить
                        // пользовательский агент на URL-адрес возврата, сохраненный в свойствах аутентификации.
                        //
                        // Для сценариев, в которых обработчик входа по умолчанию настроен в ASP.NET Core
                        // параметры аутентификации использовать не следует, здесь можно указать конкретную схему.
                        return SignIn(new ClaimsPrincipal(identity), properties);
                    }
                }
            }
            // по умолчанию мы перенаправляем на вход
            return Redirect(redirectUrl);
        }
    }
}
