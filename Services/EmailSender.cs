using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using Azure.Core;

namespace StepChat.Services
{
    public class EmailSender
    {
        public async Task SendEmailAsync(string? receiver, string? htmlMessage)
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
                smtp.Credentials = new NetworkCredential(from.Address, "bxbuntohuuwnvyyj");

                int length = 6;
                var random = new Random();
                var result = string.Join("", Enumerable.Range(0, length).Select(i => i % 2 == 0 ? (char)('A' + random.Next(26)) + "" : random.Next(1, 10) + ""));

                mm2.Body = htmlMessage;
                mm2.IsBodyHtml = true;

                await smtp.SendMailAsync(mm2);
            }
            else
            {
                throw new ArgumentException("Recipient not set");
            }
        }
    }
}
