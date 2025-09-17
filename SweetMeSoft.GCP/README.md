# SweetMeSoft.GCP

Library with tools to interact with Google Cloud Platform (GCP) services.

## Description

`SweetMeSoft.GCP` is a library for .NET Standard 2.1 that simplifies interaction with Google Cloud services, specifically with **BigQuery** and **Cloud Storage**. It provides two main classes, `BigQueryRepo` and `GCPStorage`, which act as repositories to facilitate common operations.

## Main Components

### BigQueryRepo

`BigQueryRepo` is a generic repository for performing CRUD (Create, Read, Update, Delete) operations on Google BigQuery.

#### Initialization

To use `BigQueryRepo`, you must first configure the credentials and project ID. You can do this in three ways:

1.  **By credentials file (JSON):**
    ```csharp
    BigQueryRepo.CredentialsFileName = "path/to/your/credentials-file.json";
    BigQueryRepo.ProjectId = "your-gcp-project-id";
    ```

2.  **By JSON content of credentials:**
    ```csharp
    BigQueryRepo.CredentialsJson = "{ \"type\": \"service_account\", ... }";
    BigQueryRepo.ProjectId = "your-gcp-project-id";
    ```

3.  **By Access Token:**
    ```csharp
    BigQueryRepo.CredentialsToken = "your-access-token";
    BigQueryRepo.ProjectId = "your-gcp-project-id";
    ```

Once configured, you can get the repository instance:

```csharp
var repo = BigQueryRepo.Instance;
```

#### Usage

The repository uses a generic class `T` that must be decorated with the `BigQueryTable` attribute.

```csharp
[BigQueryTable("my_dataset")]
public class MyEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}
```

-   **Insert an item:**
    ```csharp
    await repo.InsertItem(new MyEntity { ... });
    ```

-   **Get all items:**
    ```csharp
    var all = await repo.GetAll<MyEntity>();
    ```

-   **Query with conditions (LINQ):**
    ```csharp
    var result = await repo.GetByField<MyEntity>(e => e.Age > 18);
    ```

-   **Update:**
    ```csharp
    await repo.Update(myEditedEntity, e => e.Id == myEditedEntity.Id);
    ```

-   **Delete:**
    ```csharp
    await repo.Delete<MyEntity>(e => e.Name == "John");
    ```

### GCPStorage

`GCPStorage` is a class for managing files in Google Cloud Storage.

#### Initialization

To use `GCPStorage`, you must provide the path to the credentials file:

```csharp
GCPStorage.CredentialsFileName = "path/to/your/credentials-file.json";
var storage = GCPStorage.Instance;
```

#### Usage

-   **Upload a file:**
    ```csharp
    var streamFile = new StreamFile("myfile.txt", memoryStream, ContentType.txt);
    await storage.UploadFile("my-bucket", "path/inside/bucket", streamFile);
    ```

-   **Download a file:**
    ```csharp
    var downloadedFile = await storage.DownloadFile("my-bucket", "path/inside/bucket", "myfile.txt");
    ```

-   **Delete a file:**
    ```csharp
    await storage.DeleteFile("my-bucket", "path/inside/bucket", "myfile.txt");
    ```

-   **Move a file:**
    ```csharp
    await storage.MoveFile("source-bucket", "source/path", "destination-bucket", "destination/path", streamFile);
    ```

## Dependencies

-   Google.Cloud.BigQuery.V2
-   Google.Cloud.Storage.V1
-   SweetMeSoft.Base
-   SweetMeSoft.Tools

## License

This project is under the MIT license.