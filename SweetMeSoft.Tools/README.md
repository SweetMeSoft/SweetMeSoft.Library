# SweetMeSoft.Tools

Librería con un conjunto de herramientas y utilidades reutilizables para tareas comunes.

## Descripción

`SweetMeSoft.Tools` es una librería para .NET Standard 2.1 que agrupa una colección de clases de ayuda estáticas para realizar una variedad de tareas, desde conversiones de tipos y validaciones hasta envío de correos y cifrado de datos.

## Componentes Principales

### `Converters`
Métodos para conversiones de tipos de datos, manejando la cultura local para separadores decimales.
-   `StringToDouble(string)`
-   `StringToFloat(string)`
-   `StringToDecimal(string)`
-   `StringToInt(string)`
-   `IntToString(int)`
-   `DecimalToString(decimal)`
-   `StringToBool(string)`

### `Email`
Una clase para enviar correos electrónicos a través de SMTP.
-   `Send(EmailOptions)`: Envía un correo electrónico configurado a través de un objeto `EmailOptions`.
    -   Soporta hosts preconfigurados como Gmail, Outlook y Webmail.
    -   Permite adjuntos normales e incrustados (linked).
    -   Requiere que `EmailOptions.Sender` y `EmailOptions.Password` se establezcan estáticamente antes de su uso.

### `Reflections`
Utilidades que usan reflexión.
-   `CleanVirtualProperties<T>(T entity)`: Elimina referencias circulares en objetos (comúnmente entidades de base de datos) serializando y deserializando el objeto con `ReferenceLoopHandling.Ignore`. Muy útil para preparar datos para ser enviados a través de una API.

### `Security`
Métodos para hashing de contraseñas y cifrado reversible.
-   `HashPasswordIrreversible(string)`: Crea un hash de una contraseña usando PBKDF2 con un salt.
-   `VerifyHashedPasswordIrreversible(string, string)`: Compara una contraseña en texto plano con su hash.
-   `CipherPasswordReversible(string, key, iv)`: Cifra un texto usando AES.
-   `DecipherPassword(string, key, iv)`: Descifra un texto cifrado con AES.

### `Utils`
Un conjunto de utilidades varias.
-   `GetException(Exception)`: Obtiene el mensaje de la excepción más interna.
-   `GetRandomNumber(int)`: Genera una cadena de N dígitos aleatorios.
-   `WriteToAPath(StreamFile, path)`: Guarda un `StreamFile` en una ruta del disco.
-   `StringMatchCompare(list, chain, threshold)`: Realiza una comparación de cadenas "fuzzy" para encontrar similitudes.
-   `MinifyJson(string)`: Elimina los espacios en blanco de un string JSON.

### `Validators`
Métodos de validación comunes.
-   `IsValidEmail(string)`: Verifica si un string tiene un formato de correo electrónico válido.
-   `IsValidPassword(string)`: Verifica si una contraseña cumple con requisitos de complejidad (mayúsculas, minúsculas, números, símbolos y longitud mínima).

## Dependencias

-   [Microsoft.CSharp](https://www.nuget.org/packages/Microsoft.CSharp)
-   [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
-   [SweetMeSoft.Base](https://www.nuget.org/packages/SweetMeSoft.Base/)

## Instalación

```bash
dotnet add package SweetMeSoft.Tools
```

## Ejemplo de Uso

```csharp
using SweetMeSoft.Tools;

// --- Validación ---
bool isValid = Validators.IsValidEmail("test@example.com");

// --- Seguridad ---
string password = "MySecurePassword123!";
string hashedPassword = Security.HashPasswordIrreversible(password);
bool isPasswordCorrect = Security.VerifyHashedPasswordIrreversible(hashedPassword, password);

// --- Conversores ---
int number = Converters.StringToInt("123.45"); // Resulta en 123
```

## Licencia

Este proyecto está bajo la licencia MIT. 