using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System.Globalization;
using System.Net;

namespace SweetMeSoft.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate requestDelegate;

    public ErrorHandlerMiddleware(RequestDelegate requestDelegate)
    {
        this.requestDelegate = requestDelegate;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await requestDelegate(context);
        }
        catch (Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = exception switch
            {
                AppException e => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException e => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };
            var result = JsonConvert.SerializeObject(new ProblemDetails
            {
                Title = Tools.Utils.GetException(exception),
                Status = response.StatusCode,
                Detail = exception.StackTrace,
                Instance = Guid.NewGuid().ToString(),
                Type = "Unexpected Error"
            }, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });

            await response.WriteAsync(result);
        }
    }
}

// custom exception class for throwing application specific exceptions (e.g. for validation)
// that can be caught and handled within the application
public class AppException : Exception
{
    public AppException() : base()
    {
    }

    public AppException(string message) : base(message)
    {
    }

    public AppException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}