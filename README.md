[![.NET Standard 2.1](https://img.shields.io/badge/.NET%20Standard-2.1-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-1)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C# 12](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![NuGet](https://img.shields.io/badge/NuGet-Package%20Suite-004880?logo=nuget&logoColor=white)](https://www.nuget.org/)
[![GitHub Actions](https://img.shields.io/badge/GitHub%20Actions-CI%2FCD-2088FF?logo=githubactions&logoColor=white)](https://docs.github.com/en/actions)

# SweetMeSoft Library

## Table of Contents
- [Project Summary](#project-summary)
- [Functionalities](#functionalities)
- [Third-Party Libraries and Dependencies](#third-party-libraries-and-dependencies)
- [Core Implementation](#core-implementation)
- [Target Versions](#target-versions)
- [Folder Structure](#folder-structure)
- [Design Patterns](#design-patterns)
- [Configurations](#configurations)
- [Integrations](#integrations)

---

## Project Summary

SweetMeSoft Library is an enterprise-grade monorepo suite of reusable .NET libraries distributed as individual NuGet packages. It provides foundational abstractions, security utilities, HTTP communication abstractions, file format processing engines, Google Cloud Platform service connectors, automated captcha resolution handlers, and ASP.NET Core request handling middleware for application ecosystems.

---

## Functionalities

- Cryptographic Security: High-iteration PBKDF2 password hashing with HMAC-SHA256, AES symmetric encryption and decryption, and constant-time byte array equality verification.
- HTTP Request Orchestration: Abstractions for RESTful API consumption supporting Bearer, Basic, ApiKey, and Cookie authentication mechanisms with TLS 1.2 and TLS 1.3 protocol enforcement.
- Multi-Format File Processing: Data generation, parsing, and transformation for Excel spreadsheets, CSV data streams, HTML DOM trees, XML structures, and ZIP archives.
- Cloud Service Connectivity: Native wrappers for Google Cloud Platform services including BigQuery analytical queries and Cloud Storage object management.
- Automated Captcha Resolution: Integration with external 2Captcha services for processing standard image, ReCaptcha v2/v3, and Enterprise captchas.
- Middleware Infrastructure: ASP.NET Core HTTP request/response logging, standardized problem details error formatting, and service factory abstractions.

---

## Third-Party Libraries and Dependencies

| Package Name | Purpose | Target Projects |
| :--- | :--- | :--- |
| `System.Text.Json` | High-performance JSON serialization and parsing | `SweetMeSoft.Base` |
| `Microsoft.CSharp` | Dynamic language execution support | `SweetMeSoft.Tools` |
| `Microsoft.AspNet.WebApi.Client` | Legacy HTTP message formatting abstractions | `SweetMeSoft.Connectivity` |
| `EPPlus` | OpenXML Excel spreadsheet read/write engine | `SweetMeSoft.Files` |
| `NPOI` | Office OpenXML document manipulation engine | `SweetMeSoft.Files` |
| `CsvHelper` | Delimited CSV file parsing and serialization | `SweetMeSoft.Files` |
| `HtmlAgilityPack` | HTML DOM tree parsing and XPath querying | `SweetMeSoft.Files` |
| `System.Linq.Async` | Asynchronous LINQ query extensions | `SweetMeSoft.Files`, `SweetMeSoft.GCP` |
| `Google.Cloud.BigQuery.V2` | Google Cloud BigQuery API client | `SweetMeSoft.GCP` |
| `Google.Cloud.Storage.V1` | Google Cloud Storage API client | `SweetMeSoft.GCP` |
| `Microsoft.AspNetCore.Http.Abstractions` | ASP.NET Core HTTP context pipeline abstractions | `SweetMeSoft.Middleware` |
| `Microsoft.AspNetCore.Mvc.Core` | ASP.NET Core MVC core contracts | `SweetMeSoft.Middleware` |
| `xunit` | Unit testing framework | `SweetMeSoft.Tests` |
| `Microsoft.NET.Test.Sdk` | Test execution platform SDK | `SweetMeSoft.Tests` |

---

## Core Implementation

The solution is structured as a multi-targeted .NET library suite configured via Central Directory Properties (`Directory.Build.props`). Compile-time checks enforce C# 12 language rules, implicit global usings, and strict Nullable Reference Types evaluation.

Automated continuous integration and continuous deployment pipelines are defined in GitHub Actions (`.github/workflows/release.yml`). Upon publishing git version tags, the pipeline automatically restores dependencies, builds Release configurations across multi-target runtimes, executes xUnit test suites, packs NuGet packages, and publishes artifacts to the NuGet registry.

---

## Target Versions

- .NET SDK: 10.0 (supporting 8.0, 9.0, and 10.0 runtime builds)
- Target Frameworks: `.NET Standard 2.1`, `.NET 10.0`
- C# Language Version: 12.0 (`latest`)

---

## Folder Structure

```text
SweetMeSoft.Library/
├── Directory.Build.props
├── SweetMeSoft.Library.sln
├── README.md
├── .github/
│   └── workflows/
│       └── release.yml
├── SweetMeSoft.Base/
│   ├── Attributes/
│   ├── Captcha/
│   ├── Connectivity/
│   ├── Files/
│   ├── GCP/
│   ├── Interfaces/
│   └── Tools/
├── SweetMeSoft.Tools/
├── SweetMeSoft.Connectivity/
├── SweetMeSoft.Files/
├── SweetMeSoft.GCP/
├── SweetMeSoft.Captcha/
├── SweetMeSoft.Middleware/
│   ├── Interface/
│   └── Service/
└── SweetMeSoft.Tests/
```

- `Directory.Build.props`: Centralized MSBuild properties for common metadata, C# language rules, and compiler flags.
- `.github/workflows/`: CI/CD automation workflow definitions for building, testing, and NuGet publishing.
- `SweetMeSoft.Base`: Core interfaces, base data structures, attributes, and shared data transfer models.
- `SweetMeSoft.Tools`: Cryptographic security algorithms, string match utilities, reflection helpers, and converters.
- `SweetMeSoft.Connectivity`: HTTP client wrappers and API request execution engines.
- `SweetMeSoft.Files`: Multi-format file reader and writer modules for Excel, CSV, HTML, XML, and ZIP files.
- `SweetMeSoft.GCP`: Integration wrappers for Google Cloud BigQuery and Cloud Storage.
- `SweetMeSoft.Captcha`: Automated captcha solver service client wrappers.
- `SweetMeSoft.Middleware`: ASP.NET Core request logging and error handling middleware.
- `SweetMeSoft.Tests`: Automated unit test project utilizing xUnit test framework.

---

## Design Patterns

- Shared Kernel Pattern: Centralized domain models, contracts, and attributes in `SweetMeSoft.Base` shared across all downstream libraries.
- Factory Method Pattern: Instantiation abstractions for service Resolution (`ServiceFactory`).
- Singleton Pattern: Global access points for thread-safe utility service instances (`ApiReq.Instance`, `BigQueryRepo.Instance`).
- Strategy Pattern: Strategy selection for handling different captcha types (Standard, ReCaptcha v2/v3, Enterprise).
- Repository Pattern: Abstraction of data layer operations via `IRepository` contracts.

---

## Configurations

- MSBuild Properties (`Directory.Build.props`): Central configuration defining package authoring, copyright, license expressions, repository metadata, target framework defaults, and compiler nullability checks.
- GitHub Actions Secrets: `NUGET_API_KEY` environment secret configured in repository settings for authenticating NuGet package deployments.

---

## Integrations

- Google Cloud Platform Services: BigQuery API and Cloud Storage API.
- 2Captcha External API: Remote captcha solving web services.
- NuGet Package Registry: Package hosting and dependency resolution platform.
