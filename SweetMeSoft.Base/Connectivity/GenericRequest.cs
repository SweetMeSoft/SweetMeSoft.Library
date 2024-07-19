namespace SweetMeSoft.Base.Connectivity
{
    public class GenericRequest<T>
    {
        public string Url { get; set; }

        public Authentication Authentication { get; set; }

        public HeaderType HeaderType { get; set; } = HeaderType.json;

        public T Data { get; set; }

        public Dictionary<string, string> Headers { get; set; } = [];

        public Dictionary<string, string> AdditionalParams { get; set; } = [];

        public bool BypassSSL { get; set; } = false;

        public bool AllowAutoRedirect { get; set; } = true;
    }

    public enum HeaderType
    {
        json,

        xwwwunlercoded,

        formdata
    }
}