namespace SweetMeSoft.Base.Tools
{
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
            StreamFiles = new List<StreamFile>()
            {
                file
            };
        }

        public string Destinatary { get; set; }

        public string Subject { get; set; } = "";

        public string HtmlBody { get; set; } = "";

        public List<string> AdditionalDestinataries { get; set; } = new List<string>();

        public List<string> CC { get; set; } = new List<string>();

        public List<StreamFile> StreamFiles { get; set; } = new List<StreamFile>();
    }
}