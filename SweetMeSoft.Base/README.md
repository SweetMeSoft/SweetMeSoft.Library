# SweetMeSoft.Base

Librería base con un conjunto de utilidades, helpers y clases fundamentales para los proyectos de SweetMeSoft.

## Descripción

`SweetMeSoft.Base` es una librería para .NET Standard 2.1 que sirve como la piedra angular para otras librerías y aplicaciones de SweetMeSoft. Proporciona un conjunto de componentes reutilizables que incluyen atributos personalizados, conectividad genérica, manejo de captchas, constantes de aplicación, helpers para archivos, utilidades para Google Cloud Platform (GCP) y herramientas para el manejo de correos electrónicos.

## Componentes Principales

### Attributes

Un conjunto de atributos personalizados para decorar modelos y propiedades:
-   `BigQueryPKAttribute`: Marca una propiedad como clave primaria para tablas de BigQuery.
-   `BigQueryTableAttribute`: Especifica el nombre de la tabla y el dataset en BigQuery.
-   `ColumnExcelAttribute`: Define el comportamiento de una propiedad al ser exportada a Excel (nombre de columna, formato, etc.).
-   `IgnoreColumnAttribute`: Excluye una propiedad de ciertos procesos automáticos.
-   `RequestAttribute`: Utilizado para mapear propiedades en solicitudes.
-   `TemplateAttribute`: Provee texto de ayuda o explicaciones para plantillas.

### Captcha

Clases para interactuar con servicios de Captcha:
-   `CaptchaType`: Enumeración con diferentes tipos de captchas soportados (ReCaptchaV2, HCaptcha, FunCaptcha, etc.).
-   `CaptchaOptions`: Clase para configurar los parámetros necesarios para resolver un captcha.

### Connectivity

Clases para realizar solicitudes HTTP genéricas:
-   `GenericReq<T>`: Define una solicitud genérica con URL, autenticación, cabeceras y cuerpo.
-   `GenericRes<T>`: Encapsula la respuesta de una solicitud, incluyendo el `HttpResponseMessage`, cookies y el objeto deserializado.
-   `Authentication`: Soporte para diferentes tipos de autenticación como `Bearer`, `ApiKey`, `Cookie` y `Basic`.
-   `ErrorDetails`: Modelo estándar para deserializar respuestas de error.

### Constants

Una clase estática que contiene valores constantes para la aplicación:
-   `ContentTypesDict`: Un diccionario que mapea extensiones de archivo a sus correspondientes MIME types.
-   `ConfirmationTypes`: Enumeración para tipos de confirmación (Email, ResetPassword).
-   Claves para la gestión de estado y configuración (`KEY_JWT_TOKEN`, `API_URL`, etc.).

### Files

Clases y enums base para la manipulación de archivos, utilizados por `SweetMeSoft.Files`:
-   `ExcelSheet`: Representa una hoja de cálculo en un archivo Excel.
-   `ExcelOptions`: Opciones para la lectura de archivos Excel.
-   `MyXmlTextWriter`: Un `XmlTextWriter` personalizado para formateo específico.

### GCP (Google Cloud Platform)

Un conjunto de herramientas para traducir expresiones LINQ a consultas SQL, especialmente útil para BigQuery.
-   `QueryTranslator`: Un `ExpressionVisitor` que convierte un árbol de expresión LINQ a una consulta SQL `WHERE`.
-   Soporta `Where`, `Take`, `Skip`, `OrderBy`, y `OrderByDescending`.

### StreamFile

Una clase de utilidad que encapsula un `Stream` junto con metadatos importantes como el nombre del archivo y el tipo de contenido, facilitando el manejo de archivos en toda la aplicación.

### Tools

Utilidades para tareas comunes:
-   `EmailOptions`: Configura y define todos los parámetros para enviar un correo electrónico (destinatario, asunto, cuerpo, adjuntos, etc.).
-   `EmailHost`: Enumeración de proveedores de correo comunes.
-   `EmailAttachment`: Representa un archivo adjunto en un correo.
-   `StringMatch`: Una clase simple para almacenar un texto y su porcentaje de coincidencia.

## Instalación

Puedes instalar el paquete a través de la consola de NuGet Package Manager:

```powershell
Install-Package SweetMeSoft.Base
```

O mediante la CLI de .NET:

```bash
dotnet add package SweetMeSoft.Base
```

## Dependencias

-   System.Text.Json

## Licencia

Este proyecto está distribuido bajo la licencia MIT. 