using System;
using System.Net.Mail;
using System.Net;
using SweetMeSoft.Base.Tools;

namespace SweetMeSoft.Tools
{
    public class Email
    {
        public static void Send(string destinatary, string subject, string htmlBody, EmailOptions options = null)
        {
            try
            {
                options ??= new EmailOptions();

                var client = new SmtpClient
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Port = 587,
                    Host = "smtp.gmail.com",
                    Credentials = new NetworkCredential(options.EmailFrom, options.Password),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var message = new MailMessage()
                {
                    From = new MailAddress(options.EmailFrom),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = htmlBody
                };

                message.To.Add(new MailAddress(destinatary));
                foreach (var additionalDestinatary in options.AdditionalDestinataries)
                {
                    message.To.Add(new MailAddress(additionalDestinatary));
                }

                foreach (var cc in options.CC)
                {
                    message.CC.Add(new MailAddress(cc));
                }

                foreach (var file in options.StreamFiles)
                {
                    file.Stream.Position = 0;
                    message.Attachments.Add(new Attachment(file.Stream, file.FileName, file.GetContentType()));
                }

                client.Send(message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
