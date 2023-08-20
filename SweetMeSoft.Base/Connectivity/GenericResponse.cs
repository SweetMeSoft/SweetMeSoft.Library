using System.Net;
using System.Net.Http;

namespace SweetMeSoft.Base.Connectivity
{
    public class GenericResponse<T>
    {
        public HttpResponseMessage HttpResponse { get; set; }

        public CookieContainer CookieContainer { get; set; }

        public T Object { get; set; }
    }
}
