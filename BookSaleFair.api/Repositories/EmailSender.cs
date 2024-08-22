using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Net.Mail;

namespace BookSaleFair.api.Repositories
{
    public class EmailSender: IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "BSFSender@outlook.com";
            var pw = "2VD_Ni+wx(qb6*8";
            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials= new NetworkCredential(mail,pw)
            };
            var mailMessage = new MailMessage(
               from: mail,
               to: email,
               subject: subject,
               body: message
           );

            await client.SendMailAsync(mailMessage);
        }
    }
}
