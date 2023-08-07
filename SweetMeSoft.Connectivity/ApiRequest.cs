using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using SweetMeSoft.Base.Attributes;
using Newtonsoft.Json;
using SweetMeSoft.Base.Connectivity;
using System.Web;

namespace SweetMeSoft.Connectivity
{
    public class ApiRequest
    {
        private static ApiRequest instance;

        public static ApiRequest Instance => instance ??= new ApiRequest();

        public async Task<GenericResponse> PostRequest<T>(GenericRequest<T> request)
        {
            try
            {
                var cookies = new CookieContainer();
                using var httpClient = CreateClient(request, cookies);

                if (request.Data == null)
                {
                    var response = await httpClient.PostAsync(request.Url, null);
                    return new GenericResponse()
                    {
                        HttpResponse = response,
                        CookieContainer = cookies
                    };
                }

                var properties = typeof(T).GetProperties();
                switch (request.HeaderType)
                {
                    case HeaderType.xwwwunlercoded:
                        var bodyProperties = new List<KeyValuePair<string, string>>();

                        foreach (var property in properties)
                        {
                            var attr = property.GetCustomAttributes(true).FirstOrDefault(model => model.GetType().Name == "RequestAttribute");
                            var columnAttr = attr == null ? new RequestAttribute(property.Name) : attr as RequestAttribute;
                            if (property.PropertyType == typeof(string) || property.PropertyType == typeof(int))
                            {
                                if (property.GetValue(request.Data) != null)
                                {
                                    bodyProperties.Add(new KeyValuePair<string, string>(columnAttr.Name, property.GetValue(request.Data).ToString()));
                                }
                            }

                            if (property.PropertyType == typeof(List<int>))
                            {
                                var list = property.GetValue(request.Data) as List<int>;
                                foreach (var dir in list)
                                {
                                    bodyProperties.Add(new KeyValuePair<string, string>(columnAttr.Name, dir.ToString()));
                                }
                            }
                        }

                        bodyProperties.AddRange(request.AdditionalParams);

                        return new GenericResponse()
                        {
                            HttpResponse = await httpClient.PostAsync(request.Url, new FormUrlEncodedContent(bodyProperties)),
                            CookieContainer = cookies
                        };
                    case HeaderType.formdata:
                        var formContent = new MultipartFormDataContent();
                        foreach (var property in properties)
                        {
                            var attr = property.GetCustomAttributes(true).FirstOrDefault(model => model.GetType().Name == "RequestAttribute");
                            var columnAttr = attr == null ? new RequestAttribute(property.Name) : attr as RequestAttribute;
                            formContent.Add(new StringContent(property.GetValue(request.Data, null).ToString()), columnAttr.Name);
                        }

                        return new GenericResponse()
                        {
                            HttpResponse = await httpClient.PostAsync(request.Url, formContent),
                            CookieContainer = cookies
                        };
                }

                var content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
                return new GenericResponse()
                {
                    HttpResponse = await httpClient.PostAsync(request.Url, content),
                    CookieContainer = cookies
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<GenericResponse> GetRequest<T>(GenericRequest<T> request) where T : class
        {
            try
            {
                var cookies = new CookieContainer();
                using var httpClient = CreateClient(request, cookies);
                var parameters = "";
                if (request.Data != null)
                {
                    var properties = request.Data.GetType().GetProperties()
                        .Select(model => new
                        {
                            Key = model.Name,
                            Value = model.GetValue(request.Data)?.ToString()
                        })
                        .Where(p => !string.IsNullOrEmpty(p.Value))
                        .Select(p => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value)}");
                    parameters = "?" + string.Join("&", properties);
                }

                return new GenericResponse()
                {
                    HttpResponse = await httpClient.GetAsync(request.Url + parameters),
                    CookieContainer = cookies
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<GenericResponse> PutRequest<T>(GenericRequest<T> request)
        {
            try
            {
                var cookies = new CookieContainer();
                using var httpClient = CreateClient(request, cookies);

                switch (request.HeaderType)
                {
                    case HeaderType.json:
                        var content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
                        return new GenericResponse()
                        {
                            HttpResponse = await httpClient.PutAsync(request.Url, content),
                            CookieContainer = cookies
                        };
                    case HeaderType.formdata:
                    case HeaderType.xwwwunlercoded:
                        //TODO
                        break;
                }

                return new GenericResponse()
                {
                    HttpResponse = await httpClient.PutAsJsonAsync(request.Url, request.Data),
                    CookieContainer = cookies
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<GenericResponse> DeleteRequest<T>(GenericRequest<T> request)
        {
            try
            {
                var cookies = new CookieContainer();
                using var httpClient = CreateClient(request, cookies);

                return new GenericResponse()
                {
                    HttpResponse = await httpClient.DeleteAsync(request.Url),
                    CookieContainer = cookies
                };
            }
            catch
            {
                throw;
            }
        }

        private HttpClient CreateClient<T>(GenericRequest<T> request, CookieContainer cookies)
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = cookies,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            if (request.BypassSSL)
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };
            }

            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Clear();
            client.Timeout = TimeSpan.FromSeconds(600);
            if (request.Authentication != null)
            {
                switch (request.Authentication.Type)
                {
                    case AuthenticationType.Bearer:
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.Authentication.Value);
                        break;
                    case AuthenticationType.ApiKey:
                        client.DefaultRequestHeaders.Add(request.Authentication.Key, request.Authentication.Value);
                        break;
                    case AuthenticationType.Cookie:
                        client.DefaultRequestHeaders.Add("Cookie", request.Authentication.Value);
                        break;
                }
            }

            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/111.0");
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US;q=0.7,en;q=0.3");

            if (request.Headers.Count > 0)
            {
                foreach (var header in request.Headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            return client;
        }
    }
}