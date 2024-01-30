namespace SweetMeSoft.Base.Tools
{
    public class EmailAttachment
    {
        public EmailAttachment(StreamFile streamFile)
        {
            Stream = streamFile;
            Cdi = "";
        }

        public EmailAttachment(StreamFile stream, string cdi)
        {
            Stream = stream;
            Cdi = cdi;
        }

        public StreamFile Stream { get; }
        public string Cdi { get; }
        public bool IsLinked
        {
            get => !string.IsNullOrEmpty(Cdi);
        }
    }
}
