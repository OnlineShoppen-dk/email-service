using System.Configuration;
using System.Net;
using System.Net.Mail;

public record EmailMessage(string Email, string Subject, string Body);

public static class Emailer
{
    public static async Task<bool> SendEmail(EmailMessage message)
    {
        return await SendEmail(message.Email, message.Subject, message.Body);
    }

    public static async Task<bool> SendEmail(string email, string subject, string body)
    {
        var mail = ConfigurationManager.AppSettings["EtherealUsername"];
        var pw = ConfigurationManager.AppSettings["EtherealPassword"];

        var smtpHost = "smtp.ethereal.email";
        var smtpPort = 587;

        var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(mail, pw)
        };
        try
        {
            var mailMessage = new MailMessage(from: mail,
                to: email,
                subject,
                body);
            mailMessage.IsBodyHtml = true;
            await client.SendMailAsync(mailMessage);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}