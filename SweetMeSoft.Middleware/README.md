# SweetMeSoft.Middleware

Library with a set of Middlewares and services for ASP.NET Core applications.

## Description

`SweetMeSoft.Middleware` is a library for .NET Standard 2.1 that offers components for ASP.NET Core, designed to centralize error management, log requests and facilitate access to services through a Factory pattern.

## Main Components

### 1. `ErrorHandlerMiddleware`

A middleware for global exception management. Captures unhandled exceptions in the application and transforms them into a standardized JSON response with `ProblemDetails` format.

**Behavior:**
-   `AppException` (custom exception): Returns `400 Bad Request`.
-   `KeyNotFoundException`: Returns `404 Not Found`.
-   Any other exception: Returns `500 Internal Server Error`.

### 2. `RequestLoggingMiddleware`

A middleware to log request and response details. It is highly configurable, as it allows injecting a custom logging function.

**Behavior:**
-   Intercepts the request and response to read their bodies.
-   Invokes a `writeLog` function that you pass in the constructor, providing it with the `HttpContext`, response body, status code and request body.
-   Also includes its own exception handling, which logs and formats as `ProblemDetails`.

### 3. `ServiceFactory`

An implementation of the Factory pattern (`IServiceFactory`) that integrates with the ASP.NET Core dependency injection container. It allows decoupling business logic from direct implementation of `IServiceProvider`.

## Dependencies

-   [Microsoft.AspNetCore.Http.Abstractions](https://www.nuget.org/packages/Microsoft.AspNetCore.Http.Abstractions)
-   [Microsoft.AspNetCore.Mvc.Core](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Core)
-   [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
-   [SweetMeSoft.Tools](https://www.nuget.org/packages/SweetMeSoft.Tools/)

## Installation

```bash
dotnet add package SweetMeSoft.Middleware
```

## Usage

### Configuration in `Startup.cs`

You must register the components in your `Startup.cs` class (or `Program.cs` in .NET 6+).

```csharp
using SweetMeSoft.Middleware;
using SweetMeSoft.Middleware.Interface;
using SweetMeSoft.Middleware.Service;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ... other services
        
        // 1. Register the ServiceFactory
        services.AddScoped<IServiceFactory, ServiceFactory>();
        
        // ... register your own services (e.g. IMyService, MyService)
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...
        
        // 2. Add middlewares to the pipeline
        // Order is important. ErrorHandler usually goes first.
        app.UseMiddleware<ErrorHandlerMiddleware>();

        // For RequestLoggingMiddleware, you must provide your own logging logic
        app.UseMiddleware<RequestLoggingMiddleware>(async (httpContext, responseBody, statusCode, requestBody) => 
        {
            // Your logging logic here
            // Example:
            Console.WriteLine($"Request: {requestBody} | Status: {statusCode} | Response: {responseBody}");
            await Task.CompletedTask;
        });

        app.UseRouting();
        // ...
    }
}
```

### Using `IServiceFactory` in a Controller

Inject `IServiceFactory` into your controllers or services to get instances of other services.

```csharp
using SweetMeSoft.Middleware.Interface;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class MyController : ControllerBase
{
    private readonly IMyService myService;

    // Inject the factory in the constructor
    public MyController(IServiceFactory factory)
    {
        // Use the factory to get the service you need
        this.myService = factory.Get<IMyService>();
    }

    [HttpGet]
    public IActionResult Get()
    {
        var data = myService.GetData();
        return Ok(data);
    }
}
```

### Throw Custom Exceptions

From your business logic, you can throw an `AppException` to generate a controlled `400 Bad Request` response.

```csharp
public class MyService : IMyService
{
    public string GetData()
    {
        // ...
        if (string.IsNullOrEmpty(someValue))
        {
            // This will be caught by ErrorHandlerMiddleware
            throw new AppException("The value cannot be null.");
        }
        // ...
    }
}
```

## License

This project is under the MIT license.