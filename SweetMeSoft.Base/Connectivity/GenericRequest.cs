namespace SweetMeSoft.Base.Connectivity
{
    public class GenericRequest<T>
    {
        public string Url { get; set; }

        public Authentication Authentication { get; set; }

        public HeaderType HeaderType { get; set; } = HeaderType.json;

        public T Data { get; set; }

        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> AdditionalParams { get; set; } = new Dictionary<string, string>();

        public bool BypassSSL { get; set; } = false;
    }

    public enum HeaderType
    {
        json,
        xwwwunlercoded,
        formdata
    }
}
