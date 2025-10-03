using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using SweetMeSoft.Tools;

namespace SweetMeSoft.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, Func<HttpContext, string, int, string, Task> writeLog)
{
    private readonly JsonSerializerSettings jsonSettings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        Formatting = Formatting.Indented
    };

    public async Task InvokeAsync(HttpContext context)
    {
        var body = await GetRequestBody(context);
        var originalBody = context.Response.Body;
        using var memStream = new MemoryStream();
        try
        {
            context.Response.Body = memStream;
            await next(context);
            memStream.Position = 0;
            string responseBody = new StreamReader(memStream).ReadToEnd();
            memStream.Position = 0;
            await memStream.CopyToAsync(originalBody);
            context.Response.Body = originalBody;

            if (context.Response.StatusCode != 200)
            {
                await writeLog(context, responseBody, context.Response.StatusCode, body);
                context.Response.ContentType = "application/json";
                var statusCode = context.Response.StatusCode;
                string result = JsonConvert.SerializeObject(new ProblemDetails
                {
                    Title = statusCode == 401 ? "Unauthorized" : statusCode == 403 ? "Forbidden" : statusCode == 404 ? "Not Found" : statusCode == 405 ? "Method Not Allowed" : "Error",
                    Status = statusCode,
                    Detail = statusCode == 401 ? "401 Unauthorized" : statusCode == 403 ? "403 Forbidden" : statusCode == 404 ? "404 Not Found" : statusCode == 405 ? "405 Method Not Allowed" : "Error",
                    Instance = Guid.NewGuid().ToString(),
                    Type = statusCode == 401 ? "Unauthorized Exception" : statusCode == 403 ? "Forbidden Exception" : statusCode == 404 ? "Not Found Exception" : statusCode == 405 ? "Method Not Allowed Exception" : "Error Exception",
                }, jsonSettings);
                result = Utils.MinifyJson(result);
                context.Response.Body = originalBody;
                await context.Response.WriteAsync(result);
            }
            else
            {
                await writeLog(context, "", context.Response.StatusCode, body);
            }
        }
        catch (Exception exception)
        {
            context.Response.ContentType = "application/json";
            int statusCode = exception is AppException ? 400 : exception is not KeyNotFoundException ? 500 : 404;

            context.Response.StatusCode = statusCode;
            string result = JsonConvert.SerializeObject(new ProblemDetails
            {
                Title = exception.Source,
                Status = context.Response.StatusCode,
                Detail = Utils.GetException(exception),
                Instance = Guid.NewGuid().ToString(),
                Type = exception.GetType().Name,
            }, jsonSettings);
            result = Utils.MinifyJson(result);
            await writeLog(context, result, statusCode, body);
            context.Response.Body = originalBody;
            await context.Response.WriteAsync(result);
        }
    }

    private async Task<string> GetRequestBody(HttpContext context)
    {
        var ms = new MemoryStream();
        await context.Request.Body.CopyToAsync(ms);
        ms.Position = 0;
        var bodyReader = new StreamReader(ms);
        var body = await bodyReader.ReadToEndAsync();
        ms.Position = 0;
        context.Request.Body = ms;
        return Utils.MinifyJson(body);
    }
}