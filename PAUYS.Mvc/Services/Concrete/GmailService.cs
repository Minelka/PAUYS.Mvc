using PAUYS.AspNetCoreMvc.Services.Abstract;
using System.Net;
using System.Net.Mail;

namespace BookStore.AspNetCoreMvc.Services.Concrete
{
    public class GmailService : IMailService
    {
        private SmtpClient _smtpClient = new SmtpClient();
        private MailAddress _fromMailAddress = null!;
        private readonly bool _serviceStatus;

        public GmailService(IConfiguration config)
        {
            if (config.GetSection("Mail") != null)
            {
                string _host = config.GetSection("Mail:Host").Value ?? "-";
                string _port = config.GetSection("Mail:Port").Value ?? "0";
                string _enablessl = config.GetSection("Mail:EnableSsl").Value ?? "false";
                string _username = config.GetSection("Mail:UserName").Value ?? "";
                string _password = config.GetSection("Mail:Password").Value ?? "";

                _smtpClient.Host = _host; // "smtp.gmail.com";
                _smtpClient.Port = int.Parse(_port);
                _smtpClient.EnableSsl = bool.Parse(_enablessl);
                _smtpClient.Credentials = new NetworkCredential(_username, _password);

                _fromMailAddress = new MailAddress(config.GetSection("Mail:FromMail").Value ?? "", config.GetSection("Mail:FromMailDisplay").Value ?? "");

                _serviceStatus = true;
            }

        }

        public async Task SendMailAsync((string mailAddress, string displayName) to, string subject, string body, bool isBodyHtml = false)
        {
            if (_serviceStatus)
            {
                MailAddress toMailAddress = new MailAddress(to.mailAddress, to.displayName);

                MailMessage message = new MailMessage(_fromMailAddress, toMailAddress);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isBodyHtml;

                await _smtpClient.SendMailAsync(message);
            }
            else
            {
                throw new NullReferenceException("SMTP service configuration null. appsettings.json adding parametre");
            }
        }
    }
}
