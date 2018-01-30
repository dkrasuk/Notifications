using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AlfaBank.Logger;
using Notifications.BusinessLayer.Services.Interfaces;

namespace Notifications.BusinessLayer.Services
{
    public class SmtpService : ISmtpService
    {
        private readonly ILogger _logger;

        public SmtpService(ILogger logger)
        {
            _logger = logger;
        }

        private async Task SendAsync(string emailTo, string body, SmtpClient smtp, string chanel)
        {
            string emailFrom = ConfigurationManager.AppSettings["smtpSendFrom"];

            string subject = chanel;

            var mail = new MailMessage();

            mail.From = new MailAddress(emailFrom);

            mail.To.Add(emailTo);
            mail.Subject = subject;

            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, "text/html"));

            try
            {
                await smtp.SendMailAsync(mail);
                _logger.Info($"Sent email to {emailTo}");
            }
            catch (Exception e)
            {
                _logger.Error($"Error to send email to {emailTo}", e);
            }
        }

        public async Task SendAsync(string[] emailsTo, string body, string chanel)
        {
            using (SmtpClient smtp = GetSmtpClient())
            {
                smtp.UseDefaultCredentials = true;
                foreach (var email in emailsTo)
                {
                    await SendAsync(email, body, smtp, chanel);
                }
            }
        }

        public async Task SendAsync(string emailTo, string body, string chanel)
        {
            await SendAsync(new[] { emailTo }, body, chanel);
        }

        private SmtpClient GetSmtpClient()
        {
            int portNumber = 587;
            int.TryParse(ConfigurationManager.AppSettings["smtpPortNumber"], out portNumber);
            string smtpAddress = ConfigurationManager.AppSettings["smtpHostAddress"];
            return new SmtpClient(smtpAddress, portNumber);
        }
    }
}
