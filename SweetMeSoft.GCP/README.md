# SweetMeSoft.GCP

Librería con herramientas para interactuar con servicios de Google Cloud Platform (GCP).

## Descripción

`SweetMeSoft.GCP` es una librería para .NET Standard 2.1 que simplifica la interacción con los servicios de Google Cloud, específicamente con **BigQuery** y **Cloud Storage**. Proporciona dos clases principales, `BigQueryRepo` y `GCPStorage`, que actúan como repositorios para facilitar las operaciones comunes.

## Componentes Principales

### BigQueryRepo

`BigQueryRepo` es un repositorio genérico para realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) en Google BigQuery.

#### Inicialización

Para usar `BigQueryRepo`, primero debes configurar las credenciales y el ID del proyecto. Puedes hacerlo de tres maneras:

1.  **Por archivo de credenciales (JSON):**
    ```csharp
    BigQueryRepo.CredentialsFileName = "ruta/a/tu/archivo-credenciales.json";
    BigQueryRepo.ProjectId = "tu-gcp-project-id";
    ```

2.  **Por contenido JSON de las credenciales:**
    ```csharp
    BigQueryRepo.CredentialsJson = "{ \"type\": \"service_account\", ... }";
    BigQueryRepo.ProjectId = "tu-gcp-project-id";
    ```

3.  **Por Token de Acceso:**
    ```csharp
    BigQueryRepo.CredentialsToken = "tu-token-de-acceso";
    BigQueryRepo.ProjectId = "tu-gcp-project-id";
    ```

Una vez configurado, puedes obtener la instancia del repositorio:

```csharp
var repo = BigQueryRepo.Instance;
```

#### Uso

El repositorio utiliza una clase genérica `T` que debe estar decorada con el atributo `BigQueryTable`.

```csharp
[BigQueryTable("mi_dataset")]
public class MiEntidad
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public int Edad { get; set; }
}
```

-   **Insertar un item:**
    ```csharp
    await repo.InsertItem(new MiEntidad { ... });
    ```

-   **Obtener todos los items:**
    ```csharp
    var todos = await repo.GetAll<MiEntidad>();
    ```

-   **Consultar con condiciones (LINQ):**
    ```csharp
    var resultado = await repo.GetByField<MiEntidad>(e => e.Edad > 18);
    ```

-   **Actualizar:**
    ```csharp
    await repo.Update(miEntidadEditada, e => e.Id == miEntidadEditada.Id);
    ```

-   **Eliminar:**
    ```csharp
    await repo.Delete<MiEntidad>(e => e.Nombre == "Juan");
    ```

### GCPStorage

`GCPStorage` es una clase para gestionar archivos en Google Cloud Storage.

#### Inicialización

Para usar `GCPStorage`, debes proporcionar la ruta al archivo de credenciales:

```csharp
GCPStorage.CredentialsFileName = "ruta/a/tu/archivo-credenciales.json";
var storage = GCPStorage.Instance;
```

#### Uso

-   **Subir un archivo:**
    ```csharp
    var streamFile = new StreamFile("miarchivo.txt", memoryStream, ContentType.txt);
    await storage.UploadFile("mi-bucket", "ruta/dentro/del/bucket", streamFile);
    ```

-   **Descargar un archivo:**
    ```csharp
    var archivoDescargado = await storage.DownloadFile("mi-bucket", "ruta/dentro/del/bucket", "miarchivo.txt");
    ```

-   **Eliminar un archivo:**
    ```csharp
    await storage.DeleteFile("mi-bucket", "ruta/dentro/del/bucket", "miarchivo.txt");
    ```

-   **Mover un archivo:**
    ```csharp
    await storage.MoveFile("bucket-origen", "ruta/origen", "bucket-destino", "ruta/destino", streamFile);
    ```

## Dependencias

-   Google.Cloud.BigQuery.V2
-   Google.Cloud.Storage.V1
-   SweetMeSoft.Base
-   SweetMeSoft.Tools

## Licencia

Este proyecto está bajo la licencia MIT. 