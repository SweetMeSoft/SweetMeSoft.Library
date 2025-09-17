# SweetMeSoft.Tools

Library with a set of reusable tools and utilities for common tasks.

## Description

`SweetMeSoft.Tools` is a library for .NET Standard 2.1 that groups a collection of static helper classes to perform a variety of tasks, from type conversions and validations to email sending and data encryption.

## Main Components

### `Converters`
Methods for data type conversions, handling local culture for decimal separators.
-   `StringToDouble(string)`
-   `StringToFloat(string)`
-   `StringToDecimal(string)`
-   `StringToInt(string)`
-   `IntToString(int)`
-   `DecimalToString(decimal)`
-   `StringToBool(string)`

### `Email`
A class for sending emails via SMTP.
-   `Send(EmailOptions)`: Sends an email configured through an `EmailOptions` object.
    -   Supports preconfigured hosts like Gmail, Outlook and Webmail.
    -   Allows normal and embedded (linked) attachments.
    -   Requires `EmailOptions.Sender` and `EmailOptions.Password` to be set statically before use.

### `Reflections`
Utilities that use reflection.
-   `CleanVirtualProperties<T>(T entity)`: Removes circular references in objects (commonly database entities) by serializing and deserializing the object with `ReferenceLoopHandling.Ignore`. Very useful for preparing data to be sent through an API.

### `Security`
Methods for password hashing and reversible encryption.
-   `HashPasswordIrreversible(string)`: Creates a hash of a password using PBKDF2 with a salt.
-   `VerifyHashedPasswordIrreversible(string, string)`: Compares a plain text password with its hash.
-   `CipherPasswordReversible(string, key, iv)`: Encrypts text using AES.
-   `DecipherPassword(string, key, iv)`: Decrypts encrypted text with AES.

### `Utils`
A set of miscellaneous utilities.
-   `GetException(Exception)`: Gets the innermost exception message.
-   `GetRandomNumber(int)`: Generates a string of N random digits.
-   `WriteToAPath(StreamFile, path)`: Saves a `StreamFile` to a disk path.
-   `StringMatchCompare(list, chain, threshold)`: Performs a "fuzzy" string comparison to find similarities.
-   `MinifyJson(string)`: Removes whitespace from a JSON string.

### `Validators`
Common validation methods.
-   `IsValidEmail(string)`: Verifies if a string has a valid email format.
-   `IsValidPassword(string)`: Verifies if a password meets complexity requirements (uppercase, lowercase, numbers, symbols and minimum length).

## Dependencies

-   [Microsoft.CSharp](https://www.nuget.org/packages/Microsoft.CSharp)
-   [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
-   [SweetMeSoft.Base](https://www.nuget.org/packages/SweetMeSoft.Base/)

## Installation

```bash
dotnet add package SweetMeSoft.Tools
```

## Usage Example

```csharp
using SweetMeSoft.Tools;

// --- Validation ---
bool isValid = Validators.IsValidEmail("test@example.com");

// --- Security ---
string password = "MySecurePassword123!";
string hashedPassword = Security.HashPasswordIrreversible(password);
bool isPasswordCorrect = Security.VerifyHashedPasswordIrreversible(hashedPassword, password);

// --- Converters ---
int number = Converters.StringToInt("123.45"); // Results in 123
```

## License

This project is under the MIT license.