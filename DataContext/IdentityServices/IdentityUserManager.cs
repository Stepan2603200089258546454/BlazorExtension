using DataContext.IdentityExtensions;
using IdentityAbstractions;
using IdentityAbstractions.FormsModels;
using IdentityAbstractions.Interfaces;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DataContext.IdentityServices
{
    public class IdentityUserManager : IUserManager
    {
        public string? Message { get; private set; }

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IIdentityRedirectManager redirectManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IUserStore<ApplicationUser> userStore;
        private readonly IEmailSender<ApplicationUser> emailSender;
        private readonly NavigationManager navigationManager;
        private readonly ILogger<IdentityUserManager> logger;

        public IdentityUserManager(
            UserManager<ApplicationUser> UserManager, 
            IIdentityRedirectManager IdentityRedirectManager,
            SignInManager<ApplicationUser> SignInManager,
            IUserStore<ApplicationUser> UserStore,
            IEmailSender<ApplicationUser> EmailSender,
            NavigationManager NavigationManager,
            ILogger<IdentityUserManager> Logger)
        {
            userManager = UserManager;
            redirectManager = IdentityRedirectManager;
            signInManager = SignInManager;
            userStore = UserStore;
            emailSender = EmailSender;
            navigationManager = NavigationManager;
            logger = Logger;
        }

        public ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Невозможно создать экземпляр '{nameof(ApplicationUser)}'. " +
                    $"Убедитесь, что '{nameof(ApplicationUser)}' не является абстрактным классом и имеет конструктор без параметров");
            }
        }

        public IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Пользовательский интерфейс по умолчанию требует наличия хранилища пользователей с поддержкой по электронной почте.");
            }
            return (IUserEmailStore<ApplicationUser>)userStore;
        }

        public async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            return await signInManager.GetExternalAuthenticationSchemesAsync();
        }


        /// <summary>
        /// Для страницы /Account/ConfirmEmail
        /// </summary>
        public async ValueTask ConfirmEmailPageAsync(HttpContext httpContext, string? userId, string? code)
        {
            if (userId is null || code is null)
            {
                redirectManager.RedirectTo("");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                Message = $"Ошибка загрузки пользователя с идентификатором {userId}";
            }
            else
            {
                var _code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await userManager.ConfirmEmailAsync(user, _code);
                Message = result.Succeeded ? "Спасибо за подтверждение вашего адреса электронной почты." : "Ошибка подтверждения вашего адреса электронной почты.";
            }
        }
        /// <summary>
        /// Для страницы /Account/ConfirmEmailChange
        /// </summary>
        public async ValueTask ConfirmEmailChangePageAsync(HttpContext httpContext, string? userId, string? email, string? code)
        {
            if (userId is null || email is null || code is null)
            {
                redirectManager.RedirectToWithStatus(
                    "Account/Login", "Ошибка: неверная ссылка для подтверждения изменения адреса электронной почты.", httpContext);
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                Message = "Не удалось найти пользователя с идентификатором '{userId}'";
            }

            var _code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await userManager.ChangeEmailAsync(user, email, _code);
            if (!result.Succeeded)
            {
                Message = "Ошибка при изменении адреса электронной почты.";
            }

            // В нашем пользовательском интерфейсе адрес электронной почты и имя пользователя — это одно и то же, 
            // поэтому при обновлении адреса электронной почты нам нужно обновить и имя пользователя.
            var setUserNameResult = await userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                Message = "Ошибка при изменении имени пользователя.";
            }

            await signInManager.RefreshSignInAsync(user);
            Message = "Спасибо за подтверждение изменения адреса электронной почты.";
        }
        /// <summary>
        /// Для страницы /Account/ExternalLogin
        /// </summary>
        public async ValueTask<ExternalLoginInfo> ExternalLoginPageAsync(HttpContext httpContext, string? remoteError, string? action, string? returnUrl, EmailInputModel input)
        {
            ExternalLoginInfo? externalLoginInfo = null;
            if (remoteError is not null)
            {
                redirectManager.RedirectToWithStatus("Account/Login", $"Ошибка внешнего поставщика: {remoteError}", httpContext);
            }

            ExternalLoginInfo? info = await signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                redirectManager.RedirectToWithStatus("Account/Login", "Ошибка загрузки внешней информации для входа.", httpContext);
            }

            externalLoginInfo = info;

            if (HttpMethods.IsGet(httpContext.Request.Method))
            {
                if (action == Const.LoginCallbackAction)
                {
                    await OnExternalLoginCallbackAsync(httpContext, externalLoginInfo, returnUrl, input);
                    return externalLoginInfo;
                }

                // Мы должны попасть на эту страницу только через обратный вызов входа, поэтому перенаправляем обратно на
                // страницу входа, если мы попадём сюда другим путём.
                redirectManager.RedirectTo("Account/Login");
            }
            return externalLoginInfo;
        }
        private async ValueTask OnExternalLoginCallbackAsync(HttpContext httpContext, ExternalLoginInfo? externalLoginInfo, string? returnUrl, EmailInputModel input)
        {
            if (externalLoginInfo is null)
            {
                redirectManager.RedirectToWithStatus("Account/Login", "Ошибка загрузки внешней информации для входа.", httpContext);
            }

            // Авторизуйте пользователя с помощью этого внешнего поставщика входа, если у пользователя уже есть вход.
            var result = await signInManager.ExternalLoginSignInAsync(
                externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            if (result.Succeeded)
            {
                logger.LogInformation(
                    "{Name} выполнил вход через провайдера {LoginProvider}.",
                    externalLoginInfo.Principal.Identity?.Name,
                    externalLoginInfo.LoginProvider);
                redirectManager.RedirectTo(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                redirectManager.RedirectTo("Account/Lockout");
            }

            // Если у пользователя нет учетной записи, попросим пользователя создать учетную запись.
            if (externalLoginInfo.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                input.Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
            }
        }
        public async ValueTask OnExternalLoginValidSubmitAsync(HttpContext httpContext, ExternalLoginInfo? externalLoginInfo, string? returnUrl, EmailInputModel Input)
        {
            if (externalLoginInfo is null)
            {
                redirectManager.RedirectToWithStatus("Account/Login", "Ошибка загрузки внешней информации для входа во время подтверждения.", httpContext);
            }

            var emailStore = GetEmailStore();
            var user = CreateUser();

            await userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await userManager.AddLoginAsync(user, externalLoginInfo);
                if (result.Succeeded)
                {
                    logger.LogInformation("Пользователь создал учетную запись, используя провайдера {Name}.", externalLoginInfo.LoginProvider);

                    var userId = await userManager.GetUserIdAsync(user);
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = navigationManager.GetUriWithQueryParameters(
                        navigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                        new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });
                    await emailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

                    // Если требуется подтверждение учетной записи, нам нужно показать ссылку, если у нас нет реального отправителя электронной почты
                    if (userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        redirectManager.RedirectTo("Account/RegisterConfirmation", new() { ["email"] = Input.Email });
                    }

                    await signInManager.SignInAsync(user, isPersistent: false, externalLoginInfo.LoginProvider);
                    redirectManager.RedirectTo(returnUrl);
                }
            }

            Message = $"Ошибка: {string.Join(",", result.Errors.Select(error => error.Description))}";
        }
        public async ValueTask OnForgotPasswordValidSubmitAsync(EmailInputModel Input)
        {
            var user = await userManager.FindByEmailAsync(Input.Email);
            if (user is null || !(await userManager.IsEmailConfirmedAsync(user)))
            {
                // Не показывайте, что пользователь не существует или не подтверждён
                redirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
            }

            // Для получения дополнительной информации о том, как включить подтверждение учётной записи и сброс пароля, посетите
            // страницу https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = navigationManager.GetUriWithQueryParameters(
                navigationManager.ToAbsoluteUri("Account/ResetPassword").AbsoluteUri,
                new Dictionary<string, object?> { ["code"] = code });

            await emailSender.SendPasswordResetLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

            redirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
        }
        public async ValueTask IsGetSignOutAsync(HttpContext httpContext)
        {
            if (HttpMethods.IsGet(httpContext.Request.Method))
            {
                // Очистите существующий внешний cookie-файл, чтобы обеспечить чистый процесс входа в систему
                await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }
        }
        public async ValueTask LoginUser(LoginInputModel Input, string? ReturnUrl)
        {
            // Это не учитывает неудачные попытки входа в систему при блокировке учётной записи.
            // Чтобы разрешить блокировку учётной записи при неудачном вводе пароля, установите lockoutOnFailure: true.
            var result = await signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                logger.LogInformation("Пользователь вошел в систему.");
                redirectManager.RedirectTo(ReturnUrl);
            }
            else if (result.RequiresTwoFactor)
            {
                redirectManager.RedirectTo(
                    "Account/LoginWith2fa",
                    new() { ["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe });
            }
            else if (result.IsLockedOut)
            {
                logger.LogWarning("Учетная запись пользователя заблокирована.");
                redirectManager.RedirectTo("Account/Lockout");
            }
            else
            {
                Message = "Ошибка: неверная попытка входа.";
            }
        }
        public string GetUrlAccountRegister(string? ReturnUrl)
        {
            return navigationManager.GetUriWithQueryParameters("Account/Register", new Dictionary<string, object?> { ["ReturnUrl"] = ReturnUrl });
        }
        public async Task<ApplicationUser> GetTwoFactorAuthenticationUserAsync()
        {
            return await signInManager.GetTwoFactorAuthenticationUserAsync() ??
                throw new InvalidOperationException("Не удалось загрузить пользователя с двухфакторной аутентификацией.");
        }
        public async Task OnLoginWith2faValidSubmitAsync(ApplicationUser user, TwoFactorCodeInputModel Input, bool RememberMe, string? ReturnUrl)
        {
            var authenticatorCode = Input.TwoFactorCode!.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, RememberMe, Input.RememberMachine);
            var userId = await userManager.GetUserIdAsync(user);

            if (result.Succeeded)
            {
                logger.LogInformation("Пользователь с идентификатором '{UserId}' вошел в систему с помощью 2fa.", userId);
                redirectManager.RedirectTo(ReturnUrl);
            }
            else if (result.IsLockedOut)
            {
                logger.LogWarning("Учетная запись пользователя с идентификатором '{UserId}' заблокирована.", userId);
                redirectManager.RedirectTo("Account/Lockout");
            }
            else
            {
                logger.LogWarning("Неверный код аутентификации введен для пользователя с идентификатором '{UserId}'.", userId);
                Message = "Ошибка: неверный код аутентификатора.";
            }
        }
        public async Task OnLoginWithRecoveryCodeValidSubmitAsync(ApplicationUser user, RecoveryCodeInputModel Input, string? ReturnUrl)
        {
            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

            var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            var userId = await userManager.GetUserIdAsync(user);

            if (result.Succeeded)
            {
                logger.LogInformation("Пользователь с идентификатором '{UserId}' вошел в систему с кодом восстановления.", userId);
                redirectManager.RedirectTo(ReturnUrl);
            }
            else if (result.IsLockedOut)
            {
                logger.LogWarning("Учетная запись пользователя заблокирована.");
                redirectManager.RedirectTo("Account/Lockout");
            }
            else
            {
                logger.LogWarning("Неверный код восстановления введен для пользователя с идентификатором '{UserId}'", userId);
                Message = "Ошибка: введен неверный код восстановления.";
            }
        }
        public async Task RegisterUser(EditContext editContext, RegisterInputModel Input, string? ReturnUrl)
        {
            var user = CreateUser();

            await userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            var emailStore = GetEmailStore();
            await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            var result = await userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded == false)
            {
                Message = result.Errors is null ? null : $"Ошибка: {string.Join(", ", result.Errors.Select(error => error.Description))}";
                return;
            }

            logger.LogInformation("Пользователь создал новую учетную запись с паролем.");

            var userId = await userManager.GetUserIdAsync(user);
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = navigationManager.GetUriWithQueryParameters(
                navigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });

            await emailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

            if (userManager.Options.SignIn.RequireConfirmedAccount)
            {
                redirectManager.RedirectTo(
                    "Account/RegisterConfirmation",
                    new() { ["email"] = Input.Email, ["returnUrl"] = ReturnUrl });
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            redirectManager.RedirectTo(ReturnUrl);
        }
        public async Task<string> RegisterConfirmationPageAsync(HttpContext HttpContext, string? Email, string? ReturnUrl)
        {
            if (Email is null)
            {
                redirectManager.RedirectTo("");
            }

            var user = await userManager.FindByEmailAsync(Email);
            if (user is null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                Message = "Ошибка поиска пользователя по неизвестному адресу электронной почты";
            }
            else if (emailSender is IdentityNoOpEmailSender)
            {
                // После добавления настоящего отправителя электронной почты вам следует удалить этот код, позволяющий подтвердить учетную запись.
                var userId = await userManager.GetUserIdAsync(user);
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                return navigationManager.GetUriWithQueryParameters(
                    navigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                    new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });
            }
            return string.Empty;
        }
        public async Task OnResendEmailConfirmationValidSubmitAsync(EmailInputModel Input)
        {
            var user = await userManager.FindByEmailAsync(Input.Email!);
            if (user is null)
            {
                Message = "Письмо с подтверждением отправлено. Проверьте почту.";
            }

            var userId = await userManager.GetUserIdAsync(user);
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = navigationManager.GetUriWithQueryParameters(
                navigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });
            await emailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

            Message = "Письмо с подтверждением отправлено. Проверьте почту.";
        }
        public void OnResetPasswordInitialized(ResetPasswordInputModel Input, string? Code)
        {
            if (Code is null)
            {
                redirectManager.RedirectTo("Account/InvalidPasswordReset");
            }

            Input.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
        }
        public async Task OnResetPasswordValidSubmitAsync(ResetPasswordInputModel Input)
        {
            var user = await userManager.FindByEmailAsync(Input.Email);
            if (user is null)
            {
                // Не показывайте, что пользователя не существует
                redirectManager.RedirectTo("Account/ResetPasswordConfirmation");
            }

            var result = await userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                redirectManager.RedirectTo("Account/ResetPasswordConfirmation");
            }

            Message = result.Errors is null ? null : $"Ошибка: {string.Join(", ", result.Errors.Select(error => error.Description))}";
        }
    }
}
