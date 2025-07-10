# SweetMeSoft.Connectivity

Librería para simplificar y estandarizar las peticiones a APIs y servicios web.

## Descripción

`SweetMeSoft.Connectivity` es una librería para .NET Standard 2.1 que proporciona una clase `ApiReq` como un wrapper sobre `HttpClient`. Su objetivo es facilitar la realización de peticiones HTTP (GET, POST, PUT, DELETE), el manejo de diferentes tipos de contenido, la gestión de cookies y la deserialización de respuestas de una manera estructurada y reutilizable.

## Características

-   **Wrapper de HttpClient:** Abstrae la complejidad de `HttpClient` en una API más simple.
-   **Soporte para Métodos HTTP:** Implementa `GET`, `POST`, `PUT` y `DELETE`.
-   **Manejo de Contenido:** Soporta `application/json`, `application/x-www-form-urlencoded` y `multipart/form-data`.
-   **Clases Genéricas:** Utiliza `GenericReq<T>` para las peticiones y `GenericRes<T>` para las respuestas, permitiendo un tipado fuerte.
-   **Gestión de Cookies:** Administra automáticamente las cookies entre peticiones.
-   **Descarga de Archivos:** Incluye un método `DownloadFile` para obtener archivos como un `StreamFile`.
-   **Patrón Singleton:** Proporciona una instancia única a través de `ApiReq.Instance` para un fácil acceso.

## Dependencias

-   [Microsoft.AspNet.WebApi.Client](https://www.nuget.org/packages/Microsoft.AspNet.WebApi.Client/)
-   [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
-   [SweetMeSoft.Base](https://www.nuget.org/packages/SweetMeSoft.Base/)

## Instalación

```bash
dotnet add package SweetMeSoft.Connectivity
```

## Uso

La librería se utiliza a través de la instancia singleton `ApiReq.Instance`.

### `GenericReq<T>`: La Petición

Este objeto se usa para configurar todos los detalles de la petición.

| Propiedad         | Tipo             | Descripción                                                                 |
| ----------------- | ---------------- | --------------------------------------------------------------------------- |
| `Url`             | `string`         | **Requerido.** La URL del endpoint.                                         |
| `Data`            | `T`              | El objeto con los datos a enviar en el cuerpo de la petición (para POST y PUT). |
| `HeaderType`      | `HeaderType`     | El tipo de contenido (`json`, `xwwwunlercoded`, `formdata`).                |
| `Authentication`  | `Authentication` | Objeto para configurar la autenticación (ej. Bearer Token).                 |
| `Cookies`         | `string`         | Cookies a enviar en la petición.                                            |
| `Headers`         | `HttpHeaders`    | Cabeceras HTTP adicionales.                                                 |
| `AdditionalParams`| `List<KeyValuePair<string, string>>` | Parámetros adicionales para `xwwwunlercoded`.             |

### `GenericRes<T>`: La Respuesta

Este objeto contiene el resultado de la petición.

| Propiedad        | Tipo                  | Descripción                                               |
| ---------------- | --------------------- | --------------------------------------------------------- |
| `Object`         | `T`                   | El objeto de respuesta deserializado (si la petición tuvo éxito). |
| `HttpResponse`   | `HttpResponseMessage` | La respuesta HTTP original.                               |
| `Error`          | `ErrorDetails`        | Detalles del error (si la petición falló).                |
| `Cookies`        | `string`              | Los cookies recibidos en la respuesta.                    |
| `IsSuccess`      | `bool`                | Indica si la petición fue exitosa (`true`/`false`).       |

### Ejemplo: Petición GET

```csharp
// Definir el modelo de la respuesta esperada
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// Realizar la petición
var response = await ApiReq.Instance.Get<User>("https://api.example.com/users/1");

if (response.IsSuccess)
{
    User user = response.Object;
    // ... usar el objeto 'user'
}
else
{
    // ... manejar el error con response.Error
}
```

### Ejemplo: Petición POST

```csharp
// Definir el modelo de la petición
public class CreateUserInput
{
    public string Name { get; set; }
    public string Email { get; set; }
}

// Definir el modelo de la respuesta
public class CreateUserOutput
{
    public int Id { get; set; }
    public string CreatedAt { get; set; }
}

// Preparar la petición
var requestData = new CreateUserInput { Name = "John Doe", Email = "john.doe@example.com" };
var request = new GenericReq<CreateUserInput>
{
    Url = "https://api.example.com/users",
    Data = requestData,
    HeaderType = HeaderType.json // o xwwwunlercoded, formdata
};

// Realizar la petición
var response = await ApiReq.Instance.Post<CreateUserInput, CreateUserOutput>(request);

if (response.IsSuccess)
{
    CreateUserOutput newUser = response.Object;
    // ...
}
```

## Licencia

Este proyecto está bajo la licencia MIT. 