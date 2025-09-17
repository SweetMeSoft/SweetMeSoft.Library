# SweetMeSoft.Base

Base library with a set of utilities, helpers and fundamental classes for SweetMeSoft projects.

## Description

`SweetMeSoft.Base` is a library for .NET Standard 2.1 that serves as the cornerstone for other SweetMeSoft libraries and applications. It provides a set of reusable components that include custom attributes, generic connectivity, captcha handling, application constants, file helpers, Google Cloud Platform (GCP) utilities and email handling tools.

## Main Components

### Attributes

A set of custom attributes to decorate models and properties:
-   `BigQueryPKAttribute`: Marks a property as primary key for BigQuery tables.
-   `BigQueryTableAttribute`: Specifies the table name and dataset in BigQuery.
-   `ColumnExcelAttribute`: Defines the behavior of a property when exported to Excel (column name, format, etc.).
-   `IgnoreColumnAttribute`: Excludes a property from certain automatic processes.
-   `RequestAttribute`: Used to map properties in requests.
-   `TemplateAttribute`: Provides help text or explanations for templates.

### Captcha

Classes to interact with Captcha services:
-   `CaptchaType`: Enumeration with different types of supported captchas (ReCaptchaV2, HCaptcha, FunCaptcha, etc.).
-   `CaptchaOptions`: Class to configure the necessary parameters to solve a captcha.

### Connectivity

Classes to perform generic HTTP requests:
-   `GenericReq<T>`: Defines a generic request with URL, authentication, headers and body.
-   `GenericRes<T>`: Encapsulates the response of a request, including the `HttpResponseMessage`, cookies and the deserialized object.
-   `Authentication`: Support for different types of authentication like `Bearer`, `ApiKey`, `Cookie` and `Basic`.
-   `ErrorDetails`: Standard model to deserialize error responses.

### Constants

A static class that contains constant values for the application:
-   `ContentTypesDict`: A dictionary that maps file extensions to their corresponding MIME types.
-   `ConfirmationTypes`: Enumeration for confirmation types (Email, ResetPassword).
-   Keys for state management and configuration (`KEY_JWT_TOKEN`, `API_URL`, etc.).

### Files

Base classes and enums for file manipulation, used by `SweetMeSoft.Files`:
-   `ExcelSheet`: Represents a spreadsheet in an Excel file.
-   `ExcelOptions`: Options for reading Excel files.
-   `MyXmlTextWriter`: A custom `XmlTextWriter` for specific formatting.

### GCP (Google Cloud Platform)

A set of tools to translate LINQ expressions to SQL queries, especially useful for BigQuery.
-   `QueryTranslator`: An `ExpressionVisitor` that converts a LINQ expression tree to a SQL `WHERE` query.
-   Supports `Where`, `Take`, `Skip`, `OrderBy`, and `OrderByDescending`.

### StreamFile

A utility class that encapsulates a `Stream` along with important metadata such as file name and content type, facilitating file handling throughout the application.

### Tools

Utilities for common tasks:
-   `EmailOptions`: Configures and defines all parameters to send an email (recipient, subject, body, attachments, etc.).
-   `EmailHost`: Enumeration of common email providers.
-   `EmailAttachment`: Represents an attachment in an email.
-   `StringMatch`: A simple class to store text and its match percentage.

## Installation

You can install the package through the NuGet Package Manager console:

```powershell
Install-Package SweetMeSoft.Base
```

Or via the .NET CLI:

```bash
dotnet add package SweetMeSoft.Base
```

## Dependencies

-   System.Text.Json

## License

This project is distributed under the MIT license.