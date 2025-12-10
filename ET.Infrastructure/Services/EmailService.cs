using ET.Application.DTOs;
using System.Net;
using System.Net.Mail;
using System.Net.Security;

namespace ET.Infrastructure.Repository
{
    public static class SmtpEmailHelper
    {
        public static async Task SendEmailAsync(string email, string subject, string message, EmailSettings emailSettings)
        {
            try
            {
                using (var client = new SmtpClient(emailSettings.SmtpHost, emailSettings.SmtpPort))
                {
                    client.EnableSsl = emailSettings.EnableSsl;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(emailSettings.SmtpUser, emailSettings.SmtpPass);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // ✔ Proper HELO domain handling (No reflection)
                    client.TargetName = "STARTTLS/" + emailSettings.SmtpHost;

                    // ✔ Accept SSL certificate if required
                    ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback((s, cert, chain, errors) => true);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(emailSettings.From, "Expense Tracker"),
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SMTP Send Failed: " + ex.Message, ex);
            }
        }
    }
}
