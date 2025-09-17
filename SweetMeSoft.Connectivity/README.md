# SweetMeSoft.Connectivity

Library to simplify and standardize requests to APIs and web services.

## Description

`SweetMeSoft.Connectivity` is a library for .NET Standard 2.1 that provides an `ApiReq` class as a wrapper over `HttpClient`. Its goal is to facilitate making HTTP requests (GET, POST, PUT, DELETE), handling different content types, cookie management and response deserialization in a structured and reusable way.

## Features

-   **HttpClient Wrapper:** Abstracts the complexity of `HttpClient` into a simpler API.
-   **HTTP Methods Support:** Implements `GET`, `POST`, `PUT` and `DELETE`.
-   **Content Handling:** Supports `application/json`, `application/x-www-form-urlencoded` and `multipart/form-data`.
-   **Generic Classes:** Uses `GenericReq<T>` for requests and `GenericRes<T>` for responses, allowing strong typing.
-   **Cookie Management:** Automatically manages cookies between requests.
-   **File Download:** Includes a `DownloadFile` method to get files as a `StreamFile`.
-   **Singleton Pattern:** Provides a single instance through `ApiReq.Instance` for easy access.

## Dependencies

-   [Microsoft.AspNet.WebApi.Client](https://www.nuget.org/packages/Microsoft.AspNet.WebApi.Client/)
-   [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
-   [SweetMeSoft.Base](https://www.nuget.org/packages/SweetMeSoft.Base/)

## Installation

```bash
dotnet add package SweetMeSoft.Connectivity
```

## Usage

The library is used through the singleton instance `ApiReq.Instance`.

### `GenericReq<T>`: The Request

This object is used to configure all request details.

| Property          | Type             | Description                                                                 |
| ----------------- | ---------------- | --------------------------------------------------------------------------- |
| `Url`             | `string`         | **Required.** The endpoint URL.                                            |
| `Data`            | `T`              | The object with data to send in the request body (for POST and PUT).       |
| `HeaderType`      | `HeaderType`     | The content type (`json`, `xwwwunlercoded`, `formdata`).                   |
| `Authentication`  | `Authentication` | Object to configure authentication (e.g. Bearer Token).                    |
| `Cookies`         | `string`         | Cookies to send in the request.                                            |
| `Headers`         | `HttpHeaders`    | Additional HTTP headers.                                                    |
| `AdditionalParams`| `List<KeyValuePair<string, string>>` | Additional parameters for `xwwwunlercoded`.              |

### `GenericRes<T>`: The Response

This object contains the request result.

| Property         | Type                  | Description                                               |
| ---------------- | --------------------- | --------------------------------------------------------- |
| `Object`         | `T`                   | The deserialized response object (if the request was successful). |
| `HttpResponse`   | `HttpResponseMessage` | The original HTTP response.                               |
| `Error`          | `ErrorDetails`        | Error details (if the request failed).                   |
| `Cookies`        | `string`              | The cookies received in the response.                     |
| `IsSuccess`      | `bool`                | Indicates if the request was successful (`true`/`false`). |

### Example: GET Request

```csharp
// Define the expected response model
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// Make the request
var response = await ApiReq.Instance.Get<User>("https://api.example.com/users/1");

if (response.IsSuccess)
{
    User user = response.Object;
    // ... use the 'user' object
}
else
{
    // ... handle the error with response.Error
}
```

### Example: POST Request

```csharp
// Define the request model
public class CreateUserInput
{
    public string Name { get; set; }
    public string Email { get; set; }
}

// Define the response model
public class CreateUserOutput
{
    public int Id { get; set; }
    public string CreatedAt { get; set; }
}

// Prepare the request
var requestData = new CreateUserInput { Name = "John Doe", Email = "john.doe@example.com" };
var request = new GenericReq<CreateUserInput>
{
    Url = "https://api.example.com/users",
    Data = requestData,
    HeaderType = HeaderType.json // or xwwwunlercoded, formdata
};

// Make the request
var response = await ApiReq.Instance.Post<CreateUserInput, CreateUserOutput>(request);

if (response.IsSuccess)
{
    CreateUserOutput newUser = response.Object;
    // ...
}
```

## License

This project is under the MIT license.