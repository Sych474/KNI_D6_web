using System.Threading.Tasks;

namespace KNI_D6_web.Services.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}