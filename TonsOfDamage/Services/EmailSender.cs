using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TonsOfDamage.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private AuthMessageSenderOptions Options;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("fynz@hotmail.fr", "Damn Motherfucker"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);

            //var key = Options.SendGridKey;
            //var client = new SendGridClient(apiKey);
            //var from = new EmailAddress("test@example.com", "Example User");
            //var ssubject = "Sending with SendGrid is Fun";
            //var to = new EmailAddress(mail, "Example User");
            //var plainTextContent = "and easy to do anywhere, even with C#";
            //var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            //var msg = MailHelper.CreateSingleEmail(from, to, ssubject, plainTextContent, htmlContent);
            //var response = await client.SendEmailAsync(msg);
        }
    }

    //private static void Main()
    //{
    //    Execute().Wait();
    //}

    //static async Task Execute()
    //{
    //    var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
    //    var client = new SendGridClient(apiKey);
    //    var from = new EmailAddress("test@example.com", "Example User");
    //    var subject = "Sending with SendGrid is Fun";
    //    var to = new EmailAddress("test@example.com", "Example User");
    //    var plainTextContent = "and easy to do anywhere, even with C#";
    //    var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
    //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
    //    var response = await client.SendEmailAsync(msg);
    //}
}


//// using SendGrid's C# Library
//// https://github.com/sendgrid/sendgrid-csharp
//using SendGrid;
//using SendGrid.Helpers.Mail;
//using System;
//using System.Threading.Tasks;

//namespace Example
//{
//    internal class Example
//    {
//        private static void Main()
//        {
//            Execute().Wait();
//        }

//        static async Task Execute()
//        {
//            var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
//            var client = new SendGridClient(apiKey);
//            var from = new EmailAddress("test@example.com", "Example User");
//            var subject = "Sending with SendGrid is Fun";
//            var to = new EmailAddress("test@example.com", "Example User");
//            var plainTextContent = "and easy to do anywhere, even with C#";
//            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
//            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
//            var response = await client.SendEmailAsync(msg);
//        }
//    }
//}
