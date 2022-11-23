using System.Net;
using System.Net.Http;

namespace SweetMeSoft.Base
{
    public class GenericResponse
    {
        public HttpResponseMessage HttpResponse { get; set; }

        public CookieContainer CookieContainer { get; set; }
    }
}
