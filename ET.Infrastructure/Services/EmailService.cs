using ET.Application.DTOs;
using System.Net;
using System.Net.Mail;

namespace ET.Infrastructure.Repository
{
    public static class SmtpEmailHelper
    {
        public async static Task SendEmailAsync(string email, string subject, string message, EmailSettings emailSettings)
        {
            try
            {
                using (var client = new SmtpClient(emailSettings.SmtpHost, emailSettings.SmtpPort))
                {
                    client.Credentials = new NetworkCredential(
                        emailSettings.SmtpUser,
                        emailSettings.SmtpPass
                    );

                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                        System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                        System.Security.Cryptography.X509Certificates.X509Chain chain,
                        System.Net.Security.SslPolicyErrors sslPolicyErrors)
                            {
                                return true;
                            };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress( emailSettings.From),
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = true,
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
