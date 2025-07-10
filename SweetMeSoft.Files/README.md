# SweetMeSoft.Files

Librería para la manipulación de archivos.

## Descripción

`SweetMeSoft.Files` es una librería para .NET Standard 2.1 que facilita la lectura y escritura de diversos formatos de archivo, como CSV, Excel, TXT, XML, HTML y ZIP. Abstrae la complejidad de las librerías subyacentes y proporciona una API simple y unificada para trabajar con archivos.

## Características

-   **CSV:** Lectura y escritura de archivos CSV utilizando `CsvHelper`.
-   **Excel:** Lectura y escritura de archivos Excel (.xlsx) con `EPPlus` y la versión 2003 (.xls) con `NPOI`.
-   **HTML:** Lectura de tablas desde archivos HTML mediante `HtmlAgilityPack`.
-   **TXT:** Lectura y escritura de archivos de texto plano.
-   **XML:** Serialización y deserialización de objetos a y desde formato XML.
-   **ZIP:** Compresión y descompresión de archivos.

## Instalación

Puedes instalar el paquete a través de la consola de NuGet Package Manager:

```powershell
Install-Package SweetMeSoft.Files
```

O mediante la CLI de .NET:

```bash
dotnet add package SweetMeSoft.Files
```

## Uso

A continuación se muestran ejemplos de uso para cada tipo de archivo soportado.

### CSV

**Leer un archivo CSV a una lista de objetos**

```csharp
using SweetMeSoft.Files;
using SweetMeSoft.Base;

// Se asume que tienes un StreamFile, que es un wrapper con el stream y metadatos del archivo.
var streamFile = new StreamFile("ruta/al/archivo.csv");
IEnumerable<MyModel> records = await Csv.Read<MyModel>(streamFile);

// También puedes usar un Stream directamente.
using (var stream = File.OpenRead("ruta/al/archivo.csv"))
{
    IEnumerable<MyModel> records = await Csv.Read<MyModel>(stream);
}
```

**Crear un archivo CSV desde una lista de objetos**

```csharp
var myList = new List<MyModel> { /* ... tus datos ... */ };
StreamFile csvFile = await Csv.Create(myList, "nuevo_archivo.csv");
```

### Excel

**Leer un archivo Excel (.xlsx)**

```csharp
var streamFile = new StreamFile("ruta/al/archivo.xlsx");
IEnumerable<MyModel> records = Excel.Read<MyModel>(streamFile);
```

**Leer un archivo Excel 2003 (.xls)**
```csharp
var streamFile = new StreamFile("ruta/al/archivo.xls");
IEnumerable<MyModel> records = Excel.Read2003File<MyModel>(streamFile);
```

**Generar un archivo Excel**

```csharp
var myList = new List<MyModel> { /* ... tus datos ... */ };
StreamFile excelFile = Excel.Generate(myList, "Hoja1", "reporte.xlsx");
```

### HTML

**Leer una tabla de un archivo HTML**

```csharp
var streamFile = new StreamFile("ruta/al/archivo.html");
List<MyModel> records = await Html.ReadTable<MyModel>(streamFile);
```

### TXT

**Leer líneas de un archivo de texto**

```csharp
using (var stream = File.OpenRead("ruta/al/archivo.txt"))
{
    List<string> lines = await Txt.ReadLines(stream);
}
```

**Generar un archivo de texto**
```csharp
var lines = new List<string> { "Línea 1", "Línea 2" };
StreamFile txtFile = await Txt.Generate(lines, "mi_archivo.txt");
```

### XML

**Leer (deserializar) desde XML**
```csharp
using (var stream = File.OpenRead("ruta/al/archivo.xml"))
{
    MyModel myObject = await Xml.Read<MyModel>(stream);
}
```

**Crear (serializar) a XML**
```csharp
var myObject = new MyModel { /* ... tus datos ... */ };
StreamFile xmlFile = Xml.Create(myObject, "mi_objeto.xml");
```

### ZIP

**Descomprimir un archivo ZIP**
```csharp
using (var stream = File.OpenRead("ruta/al/archivo.zip"))
{
    List<StreamFile> uncompressedFiles = await Zip.Uncompress(stream);
}
```

**Comprimir archivos en un ZIP**
```csharp
var file1 = new StreamFile(/* ... */);
var file2 = new StreamFile(/* ... */);
StreamFile zipFile = await Zip.Compress("mi_archivo.zip", file1, file2);
```

## Dependencias

-   CsvHelper
-   EPPlus
-   HtmlAgilityPack
-   NPOI
-   System.Linq.Async
-   SweetMeSoft.Base
-   SweetMeSoft.Tools

## Licencia

Este proyecto está distribuido bajo la licencia MIT. 