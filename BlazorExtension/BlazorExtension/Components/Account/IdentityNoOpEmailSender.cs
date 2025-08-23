using DataContext.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BlazorExtension.Components.Account
{
    // ������� ���� �else if (EmailSender is IdentityNoOpEmailSender)� ��
    // RegisterConfirmation.razor ����� ���������� � �������������� �������� ����������.
    internal sealed class IdentityNoOpEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IEmailSender emailSender = new NoOpEmailSender();

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            emailSender.SendEmailAsync(email, "����������� ���� ����� ����������� �����", 
                $"����������, ����������� ���� ������� <a href='{confirmationLink}'>����� �����</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            emailSender.SendEmailAsync(email, "�������� ������", 
                $"����������, �������� ���� ������ <a href='{resetLink}'>����� �����</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            emailSender.SendEmailAsync(email, "�������� ������", 
                $"����������, �������� ���� ������, ��������� ��������� ���: {resetCode}");
    }
}
