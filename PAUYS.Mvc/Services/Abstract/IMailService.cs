namespace PAUYS.AspNetCoreMvc.Services.Abstract
{
    public interface IMailService
    {
        Task SendMailAsync((string mailAddress, string displayName) to, string subject, string body, bool isBodyHtml = false);
    }
}
