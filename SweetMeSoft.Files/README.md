# SweetMeSoft.Files

Library for file manipulation.

## Description

`SweetMeSoft.Files` is a library for .NET Standard 2.1 that facilitates reading and writing various file formats, such as CSV, Excel, TXT, XML, HTML and ZIP. It abstracts the complexity of underlying libraries and provides a simple and unified API for working with files.

## Features

-   **CSV:** Reading and writing CSV files using `CsvHelper`.
-   **Excel:** Reading and writing Excel files (.xlsx) with `EPPlus` and 2003 version (.xls) with `NPOI`.
-   **HTML:** Reading tables from HTML files using `HtmlAgilityPack`.
-   **TXT:** Reading and writing plain text files.
-   **XML:** Serialization and deserialization of objects to and from XML format.
-   **ZIP:** File compression and decompression.

## Installation

You can install the package through the NuGet Package Manager console:

```powershell
Install-Package SweetMeSoft.Files
```

Or via the .NET CLI:

```bash
dotnet add package SweetMeSoft.Files
```

## Usage

Below are usage examples for each supported file type.

### CSV

**Read a CSV file to a list of objects**

```csharp
using SweetMeSoft.Files;
using SweetMeSoft.Base;

// Assumes you have a StreamFile, which is a wrapper with the stream and file metadata.
var streamFile = new StreamFile("path/to/file.csv");
IEnumerable<MyModel> records = await Csv.Read<MyModel>(streamFile);

// You can also use a Stream directly.
using (var stream = File.OpenRead("path/to/file.csv"))
{
    IEnumerable<MyModel> records = await Csv.Read<MyModel>(stream);
}
```

**Create a CSV file from a list of objects**

```csharp
var myList = new List<MyModel> { /* ... your data ... */ };
StreamFile csvFile = await Csv.Create(myList, "new_file.csv");
```

### Excel

**Read an Excel file (.xlsx)**

```csharp
var streamFile = new StreamFile("path/to/file.xlsx");
IEnumerable<MyModel> records = Excel.Read<MyModel>(streamFile);
```

**Read an Excel 2003 file (.xls)**
```csharp
var streamFile = new StreamFile("path/to/file.xls");
IEnumerable<MyModel> records = Excel.Read2003File<MyModel>(streamFile);
```

**Generate an Excel file**

```csharp
var myList = new List<MyModel> { /* ... your data ... */ };
StreamFile excelFile = Excel.Generate(myList, "Sheet1", "report.xlsx");
```

### HTML

**Read a table from an HTML file**

```csharp
var streamFile = new StreamFile("path/to/file.html");
List<MyModel> records = await Html.ReadTable<MyModel>(streamFile);
```

### TXT

**Read lines from a text file**

```csharp
using (var stream = File.OpenRead("path/to/file.txt"))
{
    List<string> lines = await Txt.ReadLines(stream);
}
```

**Generate a text file**
```csharp
var lines = new List<string> { "Line 1", "Line 2" };
StreamFile txtFile = await Txt.Generate(lines, "my_file.txt");
```

### XML

**Read (deserialize) from XML**
```csharp
using (var stream = File.OpenRead("path/to/file.xml"))
{
    MyModel myObject = await Xml.Read<MyModel>(stream);
}
```

**Create (serialize) to XML**
```csharp
var myObject = new MyModel { /* ... your data ... */ };
StreamFile xmlFile = Xml.Create(myObject, "my_object.xml");
```

### ZIP

**Decompress a ZIP file**
```csharp
using (var stream = File.OpenRead("path/to/file.zip"))
{
    List<StreamFile> uncompressedFiles = await Zip.Uncompress(stream);
}
```

**Compress files into a ZIP**
```csharp
var file1 = new StreamFile(/* ... */);
var file2 = new StreamFile(/* ... */);
StreamFile zipFile = await Zip.Compress("my_file.zip", file1, file2);
```

## Dependencies

-   CsvHelper
-   EPPlus
-   HtmlAgilityPack
-   NPOI
-   System.Linq.Async
-   SweetMeSoft.Base
-   SweetMeSoft.Tools

## License

This project is distributed under the MIT license.