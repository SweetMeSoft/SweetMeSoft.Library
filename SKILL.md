---
name: sweetmesoft-library
description: Official developer and agent reference skill for using and consuming SweetMeSoft.Library packages (SweetMeSoft.Base, SweetMeSoft.Tools, SweetMeSoft.Connectivity, SweetMeSoft.Files, SweetMeSoft.GCP, SweetMeSoft.Captcha, SweetMeSoft.Middleware). Trigger this skill whenever writing code that integrates SweetMeSoft NuGet packages, performs PBKDF2/AES security operations, dispatches SMTP emails, executes HTTP API requests via ApiReq, generates/reads Excel, CSV, HTML, XML, or ZIP files, queries Google BigQuery or Cloud Storage, resolves captchas via 2Captcha, or registers ASP.NET Core request logging middleware.
---

# SweetMeSoft Library Integration Skill

This skill provides comprehensive, copy-paste-ready usage guides and code examples for consuming the **SweetMeSoft.Library** package suite in .NET applications.

---

## 1. Overview & Package Installation

The SweetMeSoft Library is a modular monorepo suite of .NET packages. Install individual modules based on your application's requirements:

| Package Name | Installation Command | Description |
| :--- | :--- | :--- |
| `SweetMeSoft.Base` | `dotnet add package SweetMeSoft.Base` | Core stream wrappers, content types, mapping attributes, and base models |
| `SweetMeSoft.Tools` | `dotnet add package SweetMeSoft.Tools` | Security (PBKDF2/AES), email dispatch, JSON minification, string matching, converters |
| `SweetMeSoft.Connectivity` | `dotnet add package SweetMeSoft.Connectivity` | Generic REST API HTTP execution client (`ApiReq`) with TLS 1.2/1.3 |
| `SweetMeSoft.Files` | `dotnet add package SweetMeSoft.Files` | Multi-format file engine for Excel (EPPlus/NPOI), CSV, HTML, XML, and ZIP |
| `SweetMeSoft.GCP` | `dotnet add package SweetMeSoft.GCP` | Google Cloud BigQuery and Cloud Storage connectors |
| `SweetMeSoft.Captcha` | `dotnet add package SweetMeSoft.Captcha` | Automated 2Captcha service solver for image and ReCaptcha challenges |
| `SweetMeSoft.Middleware` | `dotnet add package SweetMeSoft.Middleware` | ASP.NET Core request/response body logging & ProblemDetails error handling |

---

## 2. Module Functionalities Guide & Code Snippets

### A. SweetMeSoft.Base (Core Abstractions & Attributes)

#### `StreamFile`
A unified wrapper carrying binary file streams, file names, and MIME content types.
```csharp
using System.IO;
using SweetMeSoft.Base;

// Construct a StreamFile instance
var streamFile = new StreamFile("report.xlsx", memoryStream, Constants.ContentType.xlsx);
```

#### Data Mapping Attributes
Decorate C# model properties to control behavior across Excel generation, BigQuery mapping, and API requests:
- `[RequestAttribute("param_name")]`: Customizes property keys for `x-www-form-urlencoded` or `multipart/form-data` API bodies.
- `[ColumnExcelAttribute("Header Name", DateFormat = "yyyy-MM-dd", Type = ExcelColumnType.Currency)]`: Sets Excel column headers and cell formatting.
- `[IgnoreColumnAttribute]` / `[IgnoreAttribute]`: Excludes properties from Excel, CSV, or payload mapping.
- `[TemplateAttribute("Field Explanation")]`: Adds explanatory sub-headers when generating Excel templates.
- `[BigQueryTableAttribute("dataset.table_name")]`: Maps C# models to BigQuery table targets.
- `[BigQueryPKAttribute]`: Marks primary key properties for BigQuery entity lookups.

---

### B. SweetMeSoft.Tools (Security, Utilities & Email)

#### `Security` Class
- **Irreversible Password Hashing**: Hashes passwords using PBKDF2 with 600,000 HMAC-SHA256 iterations (`0x01` marker). Automatically verifies legacy 1,000 iteration SHA-1 hashes (`0x00` marker) for backward compatibility.
  ```csharp
  using SweetMeSoft.Tools;

  // Hash a password securely
  string hashedPassword = Security.HashPasswordIrreversible("MySecretPassword123!");

  // Verify password (returns true or false using constant-time comparison)
  bool isValid = Security.VerifyHashedPasswordIrreversible(hashedPassword, "MySecretPassword123!");
  ```

- **Reversible AES Encryption**:
  ```csharp
  string keyBase64 = "Your32ByteBase64KeyString=";
  string ivBase64 = "Your16ByteBase64IVString=";

  // Encrypt string with AES
  string encryptedText = Security.CipherPasswordReversible("SensitiveData", keyBase64, ivBase64);

  // Decrypt string with AES
  string decryptedText = Security.DecipherPassword(encryptedText, keyBase64, ivBase64);
  ```

#### `Utils` Class
- **JSON Minification**:
  ```csharp
  string minifiedJson = Utils.MinifyJson(rawJsonString);
  ```
- **String Match Comparison**:
  Computes string similarity based on letter pairs and returns matches exceeding a decimal threshold (`0.0` to `1.0`).
  ```csharp
  List<string> catalogItems = new() { "Apple iPhone 15", "Samsung Galaxy S24", "Google Pixel 8" };
  List<StringMatch> matches = Utils.StringMatchCompare(catalogItems, "iphone 15", 0.4m);
  // Returns catalog items matching above 40% similarity
  ```
- **Random Digits Generator**:
  ```csharp
  string pinCode = Utils.GetRandomNumber(6); // Returns 6 random numeric characters
  ```
- **File Output Writer**:
  ```csharp
  Utils.WriteToAPath(streamFile, "C:/OutputDirectory");
  ```

#### `Email` Class
Dispatches HTML and text emails via SMTP.
```csharp
using SweetMeSoft.Base.Tools;
using SweetMeSoft.Tools;

var emailOptions = new EmailOptions("recipient@example.com")
{
    Subject = "System Alert",
    HtmlBody = "<p>Your account password was updated successfully.</p>",
    Host = new EmailHost
    {
        SmtpServer = "smtp.example.com",
        Port = 587,
        EmailSender = "notifications@example.com",
        PasswordSender = "smtp_password",
        EnableSsl = true
    }
};

Email.Send(emailOptions);
```

#### `Converters` Class
Culture-safe string parsing to primitive numeric and boolean types.
```csharp
double dblVal = Converters.StringToDouble("123.45");
decimal decVal = Converters.StringToDecimal("123.45");
int intVal = Converters.StringToInt("123.45"); // Truncates decimal to 123
bool boolVal = Converters.StringToBool("1");   // Returns true for "1", "true", "t"
```

---

### C. SweetMeSoft.Connectivity (HTTP REST API Client)

#### `ApiReq` Class (`ApiReq.Instance`)
Generic HTTP API engine supporting `GET`, `POST`, `PUT`, `DELETE`, and `DownloadFile` over TLS 1.2 / 1.3 with built-in authentication types (`Bearer`, `Basic`, `ApiKey`, `Cookie`).

- **HTTP GET Request**:
  ```csharp
  using SweetMeSoft.Connectivity;
  using SweetMeSoft.Base.Connectivity;

  GenericRes<MyResponseDto> response = await ApiReq.Instance.Get<MyResponseDto>("https://api.example.com/v1/users");

  if (response.HttpResponse.IsSuccessStatusCode)
  {
      MyResponseDto data = response.Object;
  }
  ```

- **HTTP POST / PUT Request**:
  ```csharp
  var request = new GenericReq<MyPayloadDto>
  {
      Url = "https://api.example.com/v1/create",
      Data = new MyPayloadDto { Name = "Sample Item" },
      HeaderType = HeaderType.json, // Options: json, xwwwunlercoded, formdata
      Authentication = new Authentication
      {
          Type = AuthenticationType.Bearer,
          Value = "YOUR_ACCESS_TOKEN"
      }
  };

  GenericRes<MyResponseDto> result = await ApiReq.Instance.Post<MyPayloadDto, MyResponseDto>(request);
  ```

- **Downloading File via HTTP**:
  ```csharp
  GenericRes<StreamFile> fileResult = await ApiReq.Instance.DownloadFile("https://example.com/files/document.pdf", "Output.pdf");
  StreamFile file = fileResult.Object;
  ```

---

### D. SweetMeSoft.Files (Excel, CSV, HTML, XML, ZIP)

#### `Excel` Class
- **Generate Excel File from Object Collection**:
  ```csharp
  using SweetMeSoft.Files;
  using SweetMeSoft.Base;

  List<MyReportModel> reportData = GetReportData();
  StreamFile excelFile = Excel.Generate(reportData, "Report Sheet", "SalesReport.xlsx");
  ```
- **Read Excel File into Strongly-Typed Collection**:
  ```csharp
  using System.IO;

  using var stream = File.OpenRead("data.xlsx");
  IEnumerable<MyModel> records = Excel.Read<MyModel>(stream, headerRow: 1);
  ```
- **Validate Excel Headers Format**:
  ```csharp
  string validationError = Excel.ValidateFormat<MyModel>(streamFile, headerRow: 1);
  ```

#### `Csv` Class
- **Generate CSV StreamFile**:
  ```csharp
  StreamFile csvFile = Csv.CreateCsv(dataList, "DataExport.csv");
  ```
- **Read CSV Stream into Collection**:
  ```csharp
  List<MyModel> records = Csv.ReadCsv<MyModel>(fileStream);
  ```

#### `Xml` Class
- **Serialize & Deserialize XML**:
  ```csharp
  string xmlContent = Xml.SerializeToXml(myObject);
  MyObject obj = Xml.DeserializeFromXml<MyObject>(xmlContent);
  ```

#### `Zip` Class
- **Compress Files into a ZIP Archive**:
  ```csharp
  List<StreamFile> filesToCompress = new() { file1, file2 };
  StreamFile zipArchive = Zip.CreateZip(filesToCompress, "Archive.zip");
  ```
- **Decompress ZIP Archive**:
  ```csharp
  List<StreamFile> extractedFiles = Zip.ExtractZip(zipStream);
  ```

---

### E. SweetMeSoft.GCP (Google Cloud BigQuery & Storage)

#### `BigQueryRepo` Class (`BigQueryRepo.Instance`)
Executes BigQuery SQL queries and inserts models mapped with `[BigQueryTable]`.
```csharp
using SweetMeSoft.GCP;

// Execute SQL Query
List<MyBigQueryModel> rows = await BigQueryRepo.Instance.Query<MyBigQueryModel>("SELECT * FROM `my_project.my_dataset.my_table` WHERE status = 'ACTIVE'");

// Insert Records
await BigQueryRepo.Instance.Insert(rows);
```

#### `GCPStorage` Class
Uploads and downloads blob files from Google Cloud Storage buckets.
```csharp
using SweetMeSoft.GCP;

// Upload File Stream to GCS Bucket
string publicUrl = await GCPStorage.UploadFile(fileStream, "image_name.png", "my-gcs-bucket-name");

// Download File Stream from GCS Bucket
StreamFile downloadedFile = await GCPStorage.DownloadFile("image_name.png", "my-gcs-bucket-name");
```

---

### F. SweetMeSoft.Captcha (2Captcha Resolver)

#### `Solver` Class
Solves captchas asynchronously using 2Captcha API integration.
```csharp
using SweetMeSoft.Captcha;
using SweetMeSoft.Base.Captcha;

var captchaOptions = new CaptchaOptions
{
    TwoCaptchaKey = "YOUR_2CAPTCHA_API_KEY",
    CaptchaType = CaptchaType.ReCaptchaV2,
    SiteKey = "6Le-wbbSAAAA...",
    SiteUrl = "https://example.com/login"
};

string token = await Solver.SolveAsync(captchaOptions);
```

---

### G. SweetMeSoft.Middleware (ASP.NET Core Request Logging)

#### `RequestLoggingMiddleware`
Asynchronously logs request/response bodies and handles non-200 status codes by outputting standard RFC 7807 `ProblemDetails` JSON responses.

```csharp
using SweetMeSoft.Middleware;

// Register in Startup.cs / Program.cs
app.UseMiddleware<RequestLoggingMiddleware>(async (httpContext, responseBody, statusCode, requestBody) =>
{
    // Custom logging delegate
    Console.WriteLine($"[HTTP {statusCode}] Request: {requestBody} | Response: {responseBody}");
});
```
