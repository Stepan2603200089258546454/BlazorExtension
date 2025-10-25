namespace IdentityAbstractions.IdentityConstants
{
    public static partial class IdentityConst
    {
        public static class IdentityRoute
        {
            public static class Account
            {
                public const string ResetPasswordConfirmation = "/Account/ResetPasswordConfirmation";
                public const string ResetPassword = "/Account/ResetPassword";
                public const string ResendEmailConfirmation = "/Account/ResendEmailConfirmation";
                public const string RegisterConfirmation = "/Account/RegisterConfirmation";
                public const string Register = "/Account/Register";
                public const string LoginWithRecoveryCode = "/Account/LoginWithRecoveryCode";
                public const string LoginWith2fa = "/Account/LoginWith2fa";
                public const string Login = "/Account/Login";
                public const string Logout = "/Account/Logout";
                public const string Lockout = "/Account/Lockout";
                public const string InvalidUser = "/Account/InvalidUser";
                public const string InvalidPasswordReset = "/Account/InvalidPasswordReset";
                public const string ForgotPasswordConfirmation = "/Account/ForgotPasswordConfirmation";
                public const string ForgotPassword = "/Account/ForgotPassword";
                public const string ExternalLogin = "/Account/ExternalLogin";
                public const string ConfirmEmailChange = "/Account/ConfirmEmailChange";
                public const string ConfirmEmail = "/Account/ConfirmEmail";
                public const string AccessDenied = "/Account/AccessDenied";
                public const string PerformExternalLogin = "/Account/PerformExternalLogin";
            }
            public static class AccountManage
            {
                public const string TwoFactorAuthentication = "/Account/Manage/TwoFactorAuthentication";
                public const string SetPassword = "/Account/Manage/SetPassword";
                public const string ResetAuthenticator = "/Account/Manage/ResetAuthenticator";
                public const string PersonalData = "/Account/Manage/PersonalData";
                public const string Index = "/Account/Manage";
                public const string GenerateRecoveryCodes = "/Account/Manage/GenerateRecoveryCodes";
                public const string ExternalLogins = "/Account/Manage/ExternalLogins";
                public const string EnableAuthenticator = "/Account/Manage/EnableAuthenticator";
                public const string Email = "/Account/Manage/Email";
                public const string Disable2fa = "/Account/Manage/Disable2fa";
                public const string DeletePersonalData = "/Account/Manage/DeletePersonalData";
                public const string DownloadPersonalData = "/Account/Manage/DownloadPersonalData";
                public const string ChangePassword = "/Account/Manage/ChangePassword";
                public const string LinkExternalLogin = "/Account/Manage/LinkExternalLogin";
            }
            public static class Admins
            {
                public const string UserAndRoleEditor = "/Identity/Admin/UserAndRoleEditor";
                public const string OpenIddictEditor = "/Identity/Admin/OpenIddictEditor";
            }
        }
    }
}
