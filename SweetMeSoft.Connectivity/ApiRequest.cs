using System.Net.Http.Headers;
using System.Net;
using System.Text;
using SweetMeSoft.Base.Attributes;
using Newtonsoft.Json;
using SweetMeSoft.Base.Connectivity;
using System.Web;
using SweetMeSoft.Base;

namespace SweetMeSoft.Connectivity
{
    public class ApiRequest
    {
        private static ApiRequest instance;

        public static ApiRequest Instance => instance ??= new ApiRequest();

        public static async Task<string> GetPageHtml(string url)
        {
            var response = await Instance.GetRequest<string, string>(new GenericRequest<string>
            {
                Url = url
            });
            return await response.HttpResponse.Content.ReadAsStringAsync();
        }

        public async Task<GenericResponse<StreamFile>> DownloadFile(string url)
        {
            return await DownloadFile(new GenericRequest<StreamFile>()
            {
                Url = url
            });
        }

        public async Task<GenericResponse<StreamFile>> DownloadFile<TReq>(GenericRequest<TReq> request)
        {
            var cookies = new CookieContainer();
            using var httpClient = CreateClient(request, cookies);
            var response = await httpClient.GetAsync(request.Url);
            var ct = response.Content.Headers.ContentType?.MediaType;
            return new GenericResponse<StreamFile>()
            {
                HttpResponse = response,
                CookieContainer = cookies,
                Object = response.IsSuccessStatusCode ? new StreamFile()
                {
                    Stream = await response.Content.ReadAsStreamAsync(),
                    FileName = Guid.NewGuid().ToString("N"),
                    ContentType = Constants.GetContentType(ct)
                } : null,
                Error = response.IsSuccessStatusCode ? null : new ErrorDetails { Detail = await response.Content.ReadAsStringAsync() }
            };
        }

        public async Task<GenericResponse<TRes>> PostRequest<TReq, TRes>(GenericRequest<TReq> request)
        {
            try
            {
                var cookies = new CookieContainer();
                var response = new HttpResponseMessage();
                using var httpClient = CreateClient(request, cookies);

                if (request.Data == null)
                {
                    response = await httpClient.PostAsync(request.Url, null);
                    return await ManageResponse<TRes>(response, cookies);
                }

                var properties = typeof(TReq).GetProperties();
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

                        response = await httpClient.PostAsync(request.Url, new FormUrlEncodedContent(bodyProperties));
                        return await ManageResponse<TRes>(response, cookies);
                    case HeaderType.formdata:
                        var formContent = new MultipartFormDataContent();
                        foreach (var property in properties)
                        {
                            var attr = property.GetCustomAttributes(true).FirstOrDefault(model => model.GetType().Name == "RequestAttribute");
                            var columnAttr = attr == null ? new RequestAttribute(property.Name) : attr as RequestAttribute;
                            formContent.Add(new StringContent(property.GetValue(request.Data, null).ToString()), columnAttr.Name);
                        }

                        response = await httpClient.PostAsync(request.Url, formContent);
                        return await ManageResponse<TRes>(response, cookies);
                }

                var content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
                response = await httpClient.PostAsync(request.Url, content);
                return await ManageResponse<TRes>(response, cookies);
            }
            catch
            {
                throw;
            }
        }

        public async Task<GenericResponse<TRes>> GetRequest<TRes>(string url)
        {
            return await Instance.GetRequest<string, TRes>(new GenericRequest<string>
            {
                Url = url
            });
        }

        public async Task<GenericResponse<TRes>> GetRequest<TReq, TRes>(GenericRequest<TReq> request) where TReq : class
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

                var response = await httpClient.GetAsync(request.Url + parameters);
                return await ManageResponse<TRes>(response, cookies);
            }
            catch
            {
                throw;
            }
        }

        public async Task<GenericResponse<TRes>> PutRequest<TReq, TRes>(GenericRequest<TReq> request)
        {
            try
            {
                var cookies = new CookieContainer();
                var response = new HttpResponseMessage();
                using var httpClient = CreateClient(request, cookies);

                switch (request.HeaderType)
                {
                    case HeaderType.json:
                        var content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
                        response = await httpClient.PutAsync(request.Url, content);
                        return await ManageResponse<TRes>(response, cookies);
                    case HeaderType.formdata:
                    case HeaderType.xwwwunlercoded:
                        //TODO
                        break;
                }

                response = await httpClient.PutAsJsonAsync(request.Url, request.Data);
                return await ManageResponse<TRes>(response, cookies);
            }
            catch
            {
                throw;
            }
        }

        public async Task<GenericResponse<TRes>> DeleteRequest<TReq, TRes>(GenericRequest<TReq> request)
        {
            try
            {
                var cookies = new CookieContainer();
                using var httpClient = CreateClient(request, cookies);
                var response = await httpClient.DeleteAsync(request.Url);
                return await ManageResponse<TRes>(response, cookies);
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

            if (request.Headers.Any())
            {
                foreach (var header in request.Headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            return client;
        }

        private async Task<GenericResponse<TRes>> ManageResponse<TRes>(HttpResponseMessage response, CookieContainer cookies)
        {
            var error = new ErrorDetails();
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    error = JsonConvert.DeserializeObject<ErrorDetails>(await response.Content.ReadAsStringAsync());
                }
                catch
                {
                    error = new ErrorDetails
                    {
                        Status = (int)response.StatusCode,
                        Detail = await response.Content.ReadAsStringAsync(),
                        Title = response.ReasonPhrase,
                        Instance = response.RequestMessage.RequestUri.ToString(),
                        Type = response.StatusCode.ToString()
                    };
                }
            }

            if (typeof(TRes) == typeof(string))
            {
                return new GenericResponse<TRes>()
                {
                    HttpResponse = response,
                    CookieContainer = cookies,
                    Object = response.IsSuccessStatusCode ? (TRes)(object)(await response.Content.ReadAsStringAsync()) : default,
                    Error = response.IsSuccessStatusCode ? null : error
                };
            }

            return new GenericResponse<TRes>()
            {
                HttpResponse = response,
                CookieContainer = cookies,
                Object = response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<TRes>() : default,
                Error = response.IsSuccessStatusCode ? null : error
            };
        }
    }
}