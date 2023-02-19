using FinalProject.ViewModels.AccountViewModels;

namespace FinalProject.Abstractions.MailService
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequestViewModel mailRequest);
    }
}
