using IdentityAbstractions.FormsModels;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace IdentityAbstractions.Interfaces
{
    public interface IAccountManager
    {
        public string? Message { get; }

        public Task<ApplicationUser> ChangePasswordOnInitializedAsync(HttpContext HttpContext);
        public Task ChangePasswordOnValidSubmitAsync(HttpContext HttpContext, ApplicationUser user, ChangePasswordInputModel Input);

        public Task<(ApplicationUser user, bool requirePassword)> DeletePersonalDataOnInitializedAsync(HttpContext HttpContext);
        public Task DeletePersonalDataOnValidSubmitAsync(ApplicationUser user, bool requirePassword, DeletePersonalDataInputModel Input);

        public Task<ApplicationUser> Disable2faOnInitializedAsync(HttpContext HttpContext);
        public Task Disable2faOnSubmitAsync(HttpContext HttpContext, ApplicationUser user);

        public Task<(ApplicationUser user, string? email, bool isEmailConfirmed)> EmailOnInitializedAsync(HttpContext HttpContext);
        public Task EmailOnValidSubmitAsync(ApplicationUser user, NewEmailInputModel Input, string? email);
        public Task EmailOnSendEmailVerificationAsync(ApplicationUser user, string? email);

        public Task<(ApplicationUser user, string sharedKey, string authenticatorUri)> EnableAuthenticatorOnInitializedAsync(HttpContext HttpContext);
        public Task<IEnumerable<string>?> EnableAuthenticatorOnValidSubmitAsync(HttpContext HttpContext, EnableAuthenticatorInputModel Input, ApplicationUser user);

        public Task<(ApplicationUser user, IList<UserLoginInfo> currentLogins, List<AuthenticationScheme> otherLogins, bool showRemoveButton)> ExternalLoginsOnInitializedAsync(HttpContext HttpContext, string? Action);
        public Task ExternalLoginsOnSubmitAsync(HttpContext HttpContext, ApplicationUser user, string? LoginProvider, string? ProviderKey);

        public Task<ApplicationUser> GenerateRecoveryCodesOnInitializedAsync(HttpContext HttpContext);
        public Task<IEnumerable<string>?> GenerateRecoveryCodesOnSubmitAsync(ApplicationUser user);

        public Task<(ApplicationUser user, string? username, string? phoneNumber)> ManageIndexOnInitializedAsync(HttpContext HttpContext);
        public Task ManageIndexOnValidSubmitAsync(HttpContext HttpContext, ManageIndexInputModel Input, ApplicationUser user, string? phoneNumber);

        public Task PersonalDataOnInitializedAsync(HttpContext HttpContext);

        public Task ResetAuthenticatorOnSubmitAsync(HttpContext HttpContext);

        public Task<ApplicationUser> SetPasswordOnInitializedAsync(HttpContext HttpContext);
        public Task SetPasswordOnValidSubmitAsync(HttpContext HttpContext, ApplicationUser user, SetPasswordInputModel Input);

        public Task<(bool canTrack, bool hasAuthenticator, bool is2faEnabled, bool isMachineRemembered, int recoveryCodesLeft)> TwoFactorAuthenticationOnInitializedAsync(HttpContext HttpContext);
        public Task TwoFactorAuthenticationOnSubmitForgetBrowserAsync(HttpContext HttpContext);

    }
}
