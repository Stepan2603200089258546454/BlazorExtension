using DataContext.IdentityExtensions;
using IdentityAbstractions;
using IdentityAbstractions.FormsModels;
using IdentityAbstractions.Interfaces;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DataContext.IdentityServices
{
    public class IdentityAccountManager : IAccountManager
    {
        public string? Message { get; private set; }

        private readonly UserManager<ApplicationUser> UserManager;
        private readonly SignInManager<ApplicationUser> SignInManager;
        private readonly IdentityUserAccessor UserAccessor;
        private readonly IIdentityRedirectManager RedirectManager;
        private readonly ILogger<IdentityAccountManager> Logger;
        private readonly IEmailSender<ApplicationUser> EmailSender;
        private readonly NavigationManager NavigationManager;
        private readonly UrlEncoder UrlEncoder;
        private readonly IUserStore<ApplicationUser> UserStore;

        public IdentityAccountManager(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            IdentityUserAccessor userAccessor, 
            IIdentityRedirectManager redirectManager, 
            ILogger<IdentityAccountManager> logger,
            IEmailSender<ApplicationUser> emailSender,
            NavigationManager navigationManager,
            UrlEncoder urlEncoder,
            IUserStore<ApplicationUser> userStore)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            UserAccessor = userAccessor;
            RedirectManager = redirectManager;
            Logger = logger;
            EmailSender = emailSender;
            NavigationManager = navigationManager;
            UrlEncoder = urlEncoder;
            UserStore = userStore;
        }

        public async Task<ApplicationUser> ChangePasswordOnInitializedAsync(HttpContext HttpContext)
        {
            ApplicationUser user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            bool hasPassword = await UserManager.HasPasswordAsync(user);
            if (hasPassword == false)
            {
                RedirectManager.RedirectTo("Account/Manage/SetPassword");
            }
            return user;
        }
        public async Task ChangePasswordOnValidSubmitAsync(HttpContext HttpContext, ApplicationUser user, ChangePasswordInputModel Input)
        {
            IdentityResult changePasswordResult = await UserManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (changePasswordResult.Succeeded == false)
            {
                Message = $"Error: {string.Join(",", changePasswordResult.Errors.Select(error => error.Description))}";
                return;
            }

            await SignInManager.RefreshSignInAsync(user);
            Logger.LogInformation("Пользователь успешно сменил пароль..");

            RedirectManager.RedirectToCurrentPageWithStatus("Ваш пароль был изменен.", HttpContext);
        }

        public async Task<(ApplicationUser user, bool requirePassword)> DeletePersonalDataOnInitializedAsync(HttpContext HttpContext)
        {
            ApplicationUser user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            bool requirePassword = await UserManager.HasPasswordAsync(user);
            return (user, requirePassword);
        }
        public async Task DeletePersonalDataOnValidSubmitAsync(ApplicationUser user, bool requirePassword, DeletePersonalDataInputModel Input)
        {
            if (requirePassword && await UserManager.CheckPasswordAsync(user, Input.Password) == false)
            {
                Message = "Ошибка: неверный пароль.";
                return;
            }

            IdentityResult result = await UserManager.DeleteAsync(user);
            if (result.Succeeded == false)
            {
                throw new InvalidOperationException("Произошла непредвиденная ошибка при удалении пользователя.");
            }

            await SignInManager.SignOutAsync();

            string userId = await UserManager.GetUserIdAsync(user);
            Logger.LogInformation("Пользователь с идентификатором '{UserId}' удалил себя.", userId);

            RedirectManager.RedirectToCurrentPage();
        }

        public async Task<ApplicationUser> Disable2faOnInitializedAsync(HttpContext HttpContext)
        {
            ApplicationUser user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            if (HttpMethods.IsGet(HttpContext.Request.Method) && await UserManager.GetTwoFactorEnabledAsync(user) == false)
            {
                throw new InvalidOperationException("Невозможно отключить 2FA для пользователя, так как в данный момент она не включена.");
            }
            return user;
        }
        public async Task Disable2faOnSubmitAsync(HttpContext HttpContext, ApplicationUser user)
        {
            IdentityResult disable2faResult = await UserManager.SetTwoFactorEnabledAsync(user, false);
            if (disable2faResult.Succeeded == false)
            {
                throw new InvalidOperationException("Произошла непредвиденная ошибка при отключении 2FA..");
            }
            string userId = await UserManager.GetUserIdAsync(user);
            Logger.LogInformation("Пользователь с идентификатором '{UserId}' отключил 2FA.", userId);
            RedirectManager.RedirectToWithStatus(
                "Account/Manage/TwoFactorAuthentication",
                "Двухфакторная аутентификация отключена. Вы можете включить её снова при настройке приложения-аутентификатора.",
                HttpContext);
        }

        public async Task<(ApplicationUser user, string? email, bool isEmailConfirmed)> EmailOnInitializedAsync(HttpContext HttpContext)
        {
            ApplicationUser user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            string? email = await UserManager.GetEmailAsync(user);
            bool isEmailConfirmed = await UserManager.IsEmailConfirmedAsync(user);
            return (user, email, isEmailConfirmed);
        }
        public async Task EmailOnValidSubmitAsync(ApplicationUser user, NewEmailInputModel Input, string? email)
        {
            if (Input.NewEmail is null || Input.NewEmail == email)
            {
                Message = "Ваш адрес электронной почты не изменился.";
                return;
            }

            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmailChange").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["email"] = Input.NewEmail, ["code"] = code });

            await EmailSender.SendConfirmationLinkAsync(user, Input.NewEmail, HtmlEncoder.Default.Encode(callbackUrl));

            Message = "Ссылка для подтверждения изменения адреса электронной почты отправлена. Проверьте свою электронную почту.";
        }
        public async Task EmailOnSendEmailVerificationAsync(ApplicationUser user, string? email)
        {
            if (email is null)
            {
                return;
            }

            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });

            await EmailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(callbackUrl));

            Message = "Письмо с подтверждением отправлено. Проверьте почту.";
        }

        public async Task<(ApplicationUser user, string sharedKey, string authenticatorUri)> EnableAuthenticatorOnInitializedAsync(HttpContext HttpContext)
        {
            ApplicationUser user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            (string sharedKey, string authenticatorUri) res = await EnableAuthenticatorLoadSharedKeyAndQrCodeUriAsync(user);
            return (user, res.sharedKey, res.authenticatorUri);
        }
        public async Task<IEnumerable<string>?> EnableAuthenticatorOnValidSubmitAsync(HttpContext HttpContext, EnableAuthenticatorInputModel Input, ApplicationUser user)
        {
            // Удалить пробелы и дефисы
            string verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            bool is2faTokenValid = await UserManager.VerifyTwoFactorTokenAsync(
                user, UserManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (is2faTokenValid == false)
            {
                Message = "Ошибка: код подтверждения недействителен.";
                return null;
            }

            await UserManager.SetTwoFactorEnabledAsync(user, true);
            string userId = await UserManager.GetUserIdAsync(user);
            Logger.LogInformation("Пользователь с идентификатором '{UserId}' включил 2FA с помощью приложения-аутентификатора.", userId);

            Message = "Ваше приложение-аутентификатор проверено.";

            IEnumerable<string>? recoveryCodes = null;
            if (await UserManager.CountRecoveryCodesAsync(user) == 0)
            {
                recoveryCodes = await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            }
            else
            {
                RedirectManager.RedirectToWithStatus("Account/Manage/TwoFactorAuthentication", Message, HttpContext);
            }
            return recoveryCodes;
        }
        private async Task<(string sharedKey, string authenticatorUri)> EnableAuthenticatorLoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
        {
            // Загрузите ключ аутентификации и URI QR-кода для отображения в форме.
            string? unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await UserManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
            }
            string sharedKey = EnableAuthenticatorFormatKey(unformattedKey!);
            string? email = await UserManager.GetEmailAsync(user);
            string authenticatorUri = EnableAuthenticatorGenerateQrCodeUri(email!, unformattedKey!);
            return (sharedKey, authenticatorUri);
        }
        private string EnableAuthenticatorFormatKey(string unformattedKey)
        {
            StringBuilder result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition));
            }
            return result.ToString().ToLowerInvariant();
        }
        private string EnableAuthenticatorGenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                Const.AuthenticatorUriFormat,
                UrlEncoder.Encode("Microsoft.AspNetCore.Identity.UI"),
                UrlEncoder.Encode(email),
                unformattedKey);
        }

        public async Task<(ApplicationUser user, IList<UserLoginInfo> currentLogins, List<AuthenticationScheme> otherLogins, bool showRemoveButton)> ExternalLoginsOnInitializedAsync(HttpContext HttpContext, string? Action)
        {
            ApplicationUser user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            IList<UserLoginInfo> currentLogins = await UserManager.GetLoginsAsync(user);
            List<AuthenticationScheme> otherLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => currentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();

            string? passwordHash = null;
            if (UserStore is IUserPasswordStore<ApplicationUser> userPasswordStore)
            {
                passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);
            }

            bool showRemoveButton = passwordHash is not null || currentLogins.Count > 1;

            if (HttpMethods.IsGet(HttpContext.Request.Method) && Action == Const.LinkLoginCallbackAction)
            {
                await ExternalLoginsOnGetLinkLoginCallbackAsync(HttpContext, user);
            }

            return (user, currentLogins, otherLogins, showRemoveButton);
        }
        public async Task ExternalLoginsOnSubmitAsync(HttpContext HttpContext, ApplicationUser user, string? LoginProvider, string? ProviderKey)
        {
            IdentityResult result = await UserManager.RemoveLoginAsync(user, LoginProvider!, ProviderKey!);
            if (result.Succeeded == false)
            {
                RedirectManager.RedirectToCurrentPageWithStatus("Ошибка: внешний логин не был удален.", HttpContext);
            }
            await SignInManager.RefreshSignInAsync(user);
            RedirectManager.RedirectToCurrentPageWithStatus("Внешний вход был удален.", HttpContext);
        }
        private async Task ExternalLoginsOnGetLinkLoginCallbackAsync(HttpContext HttpContext, ApplicationUser user)
        {
            string userId = await UserManager.GetUserIdAsync(user);
            ExternalLoginInfo? info = await SignInManager.GetExternalLoginInfoAsync(userId);
            if (info is null)
            {
                RedirectManager.RedirectToCurrentPageWithStatus("Ошибка: не удалось загрузить внешние данные для входа.", HttpContext);
            }

            IdentityResult result = await UserManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                RedirectManager.RedirectToCurrentPageWithStatus("Ошибка: внешний логин не был добавлен. Внешние логины могут быть связаны только с одной учётной записью.", HttpContext);
            }
            // Очистите существующий внешний cookie-файл, чтобы обеспечить чистый процесс входа в систему
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            RedirectManager.RedirectToCurrentPageWithStatus("Добавлен внешний вход в систему.", HttpContext);
        }

        public async Task<ApplicationUser> GenerateRecoveryCodesOnInitializedAsync(HttpContext HttpContext)
        {
            ApplicationUser user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            bool isTwoFactorEnabled = await UserManager.GetTwoFactorEnabledAsync(user);
            if (isTwoFactorEnabled == false)
            {
                throw new InvalidOperationException("Невозможно сгенерировать коды восстановления для пользователя, так как у него не включена двухфакторная аутентификация.");
            }
            return user;
        }
        public async Task<IEnumerable<string>?> GenerateRecoveryCodesOnSubmitAsync(ApplicationUser user)
        {
            string userId = await UserManager.GetUserIdAsync(user);
            IEnumerable<string>? recoveryCodes = await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            Message = "Вы сгенерировали новые коды восстановления.";
            Logger.LogInformation("Пользователь с идентификатором '{UserId}' сгенерировал новые коды восстановления 2FA.", userId);
            return recoveryCodes;
        }

        public async Task<(ApplicationUser user, string? username, string? phoneNumber)> ManageIndexOnInitializedAsync(HttpContext HttpContext)
        {
            ApplicationUser user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            var username = await UserManager.GetUserNameAsync(user);
            string? phoneNumber = await UserManager.GetPhoneNumberAsync(user);
            return (user, username, phoneNumber);
        }
        public async Task ManageIndexOnValidSubmitAsync(HttpContext HttpContext, ManageIndexInputModel Input, ApplicationUser user, string? phoneNumber)
        {
            if (Input.PhoneNumber != phoneNumber)
            {
                IdentityResult setPhoneResult = await UserManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (setPhoneResult.Succeeded == false)
                {
                    RedirectManager.RedirectToCurrentPageWithStatus("Ошибка: не удалось установить номер телефона.", HttpContext);
                }
            }
            await SignInManager.RefreshSignInAsync(user);
            RedirectManager.RedirectToCurrentPageWithStatus("Ваш профиль был обновлен", HttpContext);
        }

        public async Task PersonalDataOnInitializedAsync(HttpContext HttpContext)
        {
            _ = await UserAccessor.GetRequiredUserAsync(HttpContext);
        }

        public async Task ResetAuthenticatorOnSubmitAsync(HttpContext HttpContext)
        {
            ApplicationUser user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            await UserManager.SetTwoFactorEnabledAsync(user, false);
            await UserManager.ResetAuthenticatorKeyAsync(user);
            string userId = await UserManager.GetUserIdAsync(user);
            Logger.LogInformation("Пользователь с идентификатором '{UserId}' сбросил свой ключ приложения аутентификации.", userId);

            await SignInManager.RefreshSignInAsync(user);

            RedirectManager.RedirectToWithStatus(
                "Account/Manage/EnableAuthenticator",
                "Ваш ключ приложения аутентификации был сброшен, вам необходимо настроить приложение аутентификации, используя новый ключ.",
                HttpContext);
        }

        public async Task<ApplicationUser> SetPasswordOnInitializedAsync(HttpContext HttpContext)
        {
            ApplicationUser user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            bool hasPassword = await UserManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                RedirectManager.RedirectTo("Account/Manage/ChangePassword");
            }
            return user;
        }
        public async Task SetPasswordOnValidSubmitAsync(HttpContext HttpContext, ApplicationUser user, SetPasswordInputModel Input)
        {
            IdentityResult addPasswordResult = await UserManager.AddPasswordAsync(user, Input.NewPassword!);
            if (addPasswordResult.Succeeded == false)
            {
                Message = $"Ошибка: {string.Join(",", addPasswordResult.Errors.Select(error => error.Description))}";
                return;
            }
            await SignInManager.RefreshSignInAsync(user);
            RedirectManager.RedirectToCurrentPageWithStatus("Ваш пароль установлен.", HttpContext);
        }

        public async Task<(bool canTrack, bool hasAuthenticator, bool is2faEnabled, bool isMachineRemembered, int recoveryCodesLeft)> TwoFactorAuthenticationOnInitializedAsync(HttpContext HttpContext)
        {
            ApplicationUser user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            bool canTrack = HttpContext.Features.Get<ITrackingConsentFeature>()?.CanTrack ?? true;
            bool hasAuthenticator = await UserManager.GetAuthenticatorKeyAsync(user) is not null;
            bool is2faEnabled = await UserManager.GetTwoFactorEnabledAsync(user);
            bool isMachineRemembered = await SignInManager.IsTwoFactorClientRememberedAsync(user);
            int recoveryCodesLeft = await UserManager.CountRecoveryCodesAsync(user);
            return (canTrack, hasAuthenticator, is2faEnabled, isMachineRemembered, recoveryCodesLeft);
        }
        public async Task TwoFactorAuthenticationOnSubmitForgetBrowserAsync(HttpContext HttpContext)
        {
            await SignInManager.ForgetTwoFactorClientAsync();
            RedirectManager.RedirectToCurrentPageWithStatus(
                "Текущий браузер забыт. При следующем входе в систему из этого браузера вам будет предложено ввести код 2FA.",
                HttpContext);
        }
    }
}
