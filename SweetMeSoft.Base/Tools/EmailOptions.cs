namespace SweetMeSoft.Base.Tools;

public class EmailOptions
{
    public static string Sender { get; set; }

    public static string Password { get; set; }

    public EmailOptions(string destinatary)
    {
        Destinatary = destinatary;
    }

    public EmailOptions(string destinatary, StreamFile file)
    {
        Destinatary = destinatary;
        Attachments = [new EmailAttachment(file)];
    }

    public string Destinatary { get; set; }

    public EmailHost Host { get; set; } = EmailHost.Gmail;

    public string Subject { get; set; } = "";

    public string HtmlBody { get; set; } = "";

    public List<string> AdditionalDestinataries { get; set; } = [];

    public List<string> CC { get; set; } = [];

    public List<string> CCO { get; set; } = [];

    public List<EmailAttachment> Attachments { get; set; } = [];
}