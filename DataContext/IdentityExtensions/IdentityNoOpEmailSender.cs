using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace DataContext.IdentityExtensions
{
    // Удалите блок «else if (EmailSender is IdentityNoOpEmailSender)» из
    // RegisterConfirmation.razor после обновления с использованием реальной реализации.
    public sealed class IdentityNoOpEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IEmailSender emailSender = new NoOpEmailSender();

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            emailSender.SendEmailAsync(email, "Подтвердите свой адрес электронной почты",
                $"Пожалуйста, подтвердите свой аккаунт <a href='{confirmationLink}'>нажав здесь</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            emailSender.SendEmailAsync(email, "Сбросить пароль",
                $"Пожалуйста, сбросьте свой пароль <a href='{resetLink}'>нажав здесь</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            emailSender.SendEmailAsync(email, "Сбросить пароль",
                $"Пожалуйста, сбросьте свой пароль, используя следующий код: {resetCode}");
    }
}
