using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OngProject.Core.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OngProject.Core.Helper
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IConfiguration configuration)
        {
            _config = configuration;
        }
        public IConfiguration _config;

        public async Task SendEmailWithTemplateAsync(string ToEmail, string mailTitle, string mailBody, string mailContact)
        {
            string template = File.ReadAllText(_config["MailParams:PathTemplate"]);
            template = template.Replace(_config["MailParams:ReplaceMailTitle"], mailTitle);
            template = template.Replace(_config["MailParams:ReplaceMailBody"], mailBody);
            template = template.Replace(_config["MailParams:ReplaceMailContact"], mailContact);
            await SendEmailAsync(ToEmail, mailTitle, template);
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(_config["MailParams:SendGridKey"], subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_config["MailParams:FromMail"], _config["MailParams:FromMailDescription"]),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
