using Microsoft.Extensions.Configuration;
using System.Collections.Generic;


namespace Test.Helper
{
    internal class ConfigurationHelper
    {
        private static IConfiguration _config;

        public static IConfiguration SetConfigurations()
        {
            var myConfiguration = new Dictionary<string, string>
            {
                {"JWT:Secret","123456789123456789" },
                {"MailParams:SendGridKey", ""},
                {"MailParams:FromMail", ""},
                {"MailParams:FromMailDescription", "ONG Somos Mas"},
                {"MailParams:PathTemplate", ""},
                {"MailParams:ReplaceMailTitle", "{mail_title}"},
                {"MailParams:ReplaceMailBody", "{mail_body}"},
                {"MailParams:ReplaceMailContact", "{mail_contact}"},
                {"MailParams:WelcomeMailTitle", "Bienvenido a Ong Somos Mas!"},
                {"MailParams:WelcomeMailBody", "<p>¡Te damos la bienvenida a Ong Somos Mas!</p><p>Ahora puedes acceder a nuestro sitio, conocer nuestro trabajo, actividades y a nuestros colaboradores.</p>"},
                {"MailParams:WelcomeMailContact", "somosfundacionmas@gmail.com"}
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            return _config;
        }
    }
}
