using IdentityAbstractions.FormsModels;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityAbstractions.Interfaces
{
    public interface IUserManager
    {
        public string? Message { get; }

        public Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        public ValueTask ConfirmEmailPageAsync(HttpContext httpContext, string? userId, string? code);
        public ValueTask ConfirmEmailChangePageAsync(HttpContext httpContext, string? userId, string? email, string? code);
        public ValueTask<ExternalLoginInfo> ExternalLoginPageAsync(HttpContext httpContext, string? remoteError, string? action, string? returnUrl, EmailInputModel input);
        public ValueTask OnExternalLoginValidSubmitAsync(HttpContext httpContext, ExternalLoginInfo? externalLoginInfo, string? returnUrl, EmailInputModel Input);
        public ValueTask OnForgotPasswordValidSubmitAsync(EmailInputModel Input);
        public ValueTask LoginUser(LoginInputModel Input, string? ReturnUrl);
        public string GetUrlAccountRegister(string? ReturnUrl);
        public ValueTask IsGetSignOutAsync(HttpContext httpContext);
        public Task<ApplicationUser> GetTwoFactorAuthenticationUserAsync();
        public Task OnLoginWith2faValidSubmitAsync(ApplicationUser user, TwoFactorCodeInputModel Input, bool RememberMe, string? ReturnUrl);
        public Task OnLoginWithRecoveryCodeValidSubmitAsync(ApplicationUser user, RecoveryCodeInputModel Input, string? ReturnUrl);
        public Task RegisterUser(EditContext editContext, RegisterInputModel Input, string? ReturnUrl);
        public Task<string> RegisterConfirmationPageAsync(HttpContext HttpContext, string? Email, string? ReturnUrl);
        public Task OnResendEmailConfirmationValidSubmitAsync(EmailInputModel Input);
        public void OnResetPasswordInitialized(ResetPasswordInputModel Input, string? Code);
        public Task OnResetPasswordValidSubmitAsync(ResetPasswordInputModel Input);
    }
}
