using System.Collections.Generic;

namespace SweetMeSoft.Base.Connectivity
{
    public class GenericRequest<T>
    {
        public string Url { get; set; }

        public Authentication Authentication { get; set; }

        public HeaderType HeaderType { get; set; } = HeaderType.json;

        public T Data { get; set; }

        public List<KeyValuePair<string, string>> Headers { get; set; } = new List<KeyValuePair<string, string>>();

        public List<KeyValuePair<string, string>> AdditionalParams { get; set; } = new List<KeyValuePair<string, string>>();
    }

    public enum HeaderType
    {
        json,
        xwwwunlercoded,
        formdata
    }
}
