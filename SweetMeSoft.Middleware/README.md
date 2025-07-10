# SweetMeSoft.Middleware

Librería con un conjunto de Middlewares y servicios para aplicaciones ASP.NET Core.

## Descripción

`SweetMeSoft.Middleware` es una librería para .NET Standard 2.1 que ofrece componentes para ASP.NET Core, diseñados para centralizar la gestión de errores, registrar peticiones y facilitar el acceso a servicios a través de un patrón Factory.

## Componentes Principales

### 1. `ErrorHandlerMiddleware`

Un middleware para la gestión global de excepciones. Captura las excepciones no controladas en la aplicación y las transforma en una respuesta JSON estandarizada con formato `ProblemDetails`.

**Comportamiento:**
-   `AppException` (excepción personalizada): Devuelve `400 Bad Request`.
-   `KeyNotFoundException`: Devuelve `404 Not Found`.
-   Cualquier otra excepción: Devuelve `500 Internal Server Error`.

### 2. `RequestLoggingMiddleware`

Un middleware para registrar los detalles de las peticiones y respuestas. Es altamente configurable, ya que permite inyectar una función de logging personalizada.

**Comportamiento:**
-   Intercepta la petición y la respuesta para leer sus cuerpos.
-   Invoca una función `writeLog` que le pasas en el constructor, entregándole el `HttpContext`, el cuerpo de la respuesta, el código de estado y el cuerpo de la petición.
-   También incluye su propio manejo de excepciones, que registra y formatea como `ProblemDetails`.

### 3. `ServiceFactory`

Una implementación del patrón Factory (`IServiceFactory`) que se integra con el contenedor de inyección de dependencias de ASP.NET Core. Permite desacoplar la lógica de negocio de la implementación directa de `IServiceProvider`.

## Dependencias

-   [Microsoft.AspNetCore.Http.Abstractions](https://www.nuget.org/packages/Microsoft.AspNetCore.Http.Abstractions)
-   [Microsoft.AspNetCore.Mvc.Core](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Core)
-   [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
-   [SweetMeSoft.Tools](https://www.nuget.org/packages/SweetMeSoft.Tools/)

## Instalación

```bash
dotnet add package SweetMeSoft.Middleware
```

## Uso

### Configuración en `Startup.cs`

Debes registrar los componentes en tu clase `Startup.cs` (o `Program.cs` en .NET 6+).

```csharp
using SweetMeSoft.Middleware;
using SweetMeSoft.Middleware.Interface;
using SweetMeSoft.Middleware.Service;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ... otros servicios
        
        // 1. Registrar el ServiceFactory
        services.AddScoped<IServiceFactory, ServiceFactory>();
        
        // ... registrar tus propios servicios (ej. IMyService, MyService)
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...
        
        // 2. Añadir los middlewares al pipeline
        // Es importante el orden. El ErrorHandler generalmente va primero.
        app.UseMiddleware<ErrorHandlerMiddleware>();

        // Para el RequestLoggingMiddleware, debes proporcionar tu propia lógica de logging
        app.UseMiddleware<RequestLoggingMiddleware>(async (httpContext, responseBody, statusCode, requestBody) => 
        {
            // Tu lógica de logging aquí
            // Ejemplo:
            Console.WriteLine($"Request: {requestBody} | Status: {statusCode} | Response: {responseBody}");
            await Task.CompletedTask;
        });

        app.UseRouting();
        // ...
    }
}
```

### Uso de `IServiceFactory` en un Controlador

Inyecta `IServiceFactory` en tus controladores o servicios para obtener instancias de otros servicios.

```csharp
using SweetMeSoft.Middleware.Interface;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class MyController : ControllerBase
{
    private readonly IMyService myService;

    // Inyecta la factory en el constructor
    public MyController(IServiceFactory factory)
    {
        // Usa la factory para obtener el servicio que necesitas
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

### Lanzar Excepciones Personalizadas

Desde tu lógica de negocio, puedes lanzar una `AppException` para generar una respuesta `400 Bad Request` controlada.

```csharp
public class MyService : IMyService
{
    public string GetData()
    {
        // ...
        if (string.IsNullOrEmpty(someValue))
        {
            // Esto será capturado por ErrorHandlerMiddleware
            throw new AppException("El valor no puede ser nulo.");
        }
        // ...
    }
}
```

## Licencia

Este proyecto está bajo la licencia MIT. 