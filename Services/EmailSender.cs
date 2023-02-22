using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using Azure.Core;
using System.Configuration;
using StepChat.Classes.Configuration;

namespace StepChat.Services
{
    public class EmailSender
    {
        private readonly IConfigService _configService;

        public EmailSender(IConfigService configService)
        {
            _configService = configService;
        }


        public async Task SendEmailAsync(string receiver, string htmlMessage)
        {
            if (receiver != null)
            {
                MailAddress from = new("salaxetdinovorxan@gmail.com", "Step Chat");
                MailAddress to = new(receiver);
                MailMessage mm2 = new(from, to);
                mm2.Subject = "Email Authorization";

                SmtpClient smtp = new();

                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(from.Address, _configService?.GetValue("Email:Token"));
                mm2.Body = htmlMessage;
                mm2.IsBodyHtml = true;

                await smtp.SendMailAsync(mm2);
            }
            else
            {
                throw new ArgumentException("Receiver not set");
            }
        }
    }
}
