using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OngProject.Core.Interfaces
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message);
        public Task SendEmailWithTemplateAsync(string ToEmail, string mailTitle, string mailBody, string mailContact);
        public Task Execute(string apiKey, string subject, string message, string email);
    }
}
