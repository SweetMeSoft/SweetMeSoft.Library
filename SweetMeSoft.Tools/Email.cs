using System.Net.Mail;
using System.Net;
using SweetMeSoft.Base.Tools;

namespace SweetMeSoft.Tools
{
    public class Email
    {
        public static void Send(EmailOptions options)
        {
            if (string.IsNullOrEmpty(EmailOptions.Sender))
            {
                throw new ArgumentException("The sender must be initialized with EmailOptions.Sender");
            }

            if (string.IsNullOrEmpty(EmailOptions.Password))
            {
                throw new ArgumentException("The password must be initialized with EmailOptions.Password");
            }

            var port = options.Host == EmailHost.Gmail ? 587
                : options.Host == EmailHost.Outlook ? 25
                : options.Host == EmailHost.Webmail ? 80
                : 0;
            var host = options.Host == EmailHost.Gmail ? "smtp.gmail.com"
                : options.Host == EmailHost.Outlook ? "smtp.live.com"
                : options.Host == EmailHost.Webmail ? "smtpout.secureserver.net"
                : "";
            var ssl = options.Host == EmailHost.Gmail ? true
                : options.Host == EmailHost.Outlook ? true
                : options.Host == EmailHost.Webmail ? false
                : false;

            try
            {
                var client = new SmtpClient
                {
                    EnableSsl = ssl,
                    UseDefaultCredentials = false,
                    Port = port,
                    Host = host,
                    Credentials = new NetworkCredential(EmailOptions.Sender, EmailOptions.Password),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var message = new MailMessage()
                {
                    From = new MailAddress(EmailOptions.Sender),
                    Subject = options.Subject,
                    IsBodyHtml = true,
                    Body = options.HtmlBody
                };

                message.To.Add(new MailAddress(options.Destinatary));
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
