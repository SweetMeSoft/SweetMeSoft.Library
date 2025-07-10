# SweetMeSoft.Forms

Librería con un conjunto de clases base y utilidades para acelerar el desarrollo de aplicaciones con Xamarin.Forms siguiendo el patrón MVVM.

## Descripción

`SweetMeSoft.Forms` es una librería para .NET Standard 2.1 que proporciona un `AppBaseViewModel` robusto y una `BaseContentPage` genérica. Su objetivo es estandarizar y simplificar tareas comunes en el desarrollo de aplicaciones móviles, como la navegación, las interacciones con el usuario, las llamadas a API y el uso de funcionalidades del dispositivo.

## Características

-   **ViewModel Base (`AppBaseViewModel`):**
    -   **Navegación Simplificada:** Métodos para navegar entre páginas, abrir modales (con capacidad de devolver resultados) y manejar la pila de navegación (`GoTo`, `OpenModal`, `BackToRoot`, etc.).
    -   **Diálogos de Usuario:** Wrappers para `Acr.UserDialogs` que facilitan mostrar alertas, confirmaciones y spinners de carga.
    -   **Wrapper de API:** Métodos `Get` y `Post` que se integran con `SweetMeSoft.Connectivity` para realizar peticiones HTTP, manejando automáticamente la autenticación, la conectividad y los errores comunes (ej. `401 Unauthorized`).
    -   **Gestión de Permisos:** Helpers para verificar y solicitar permisos del dispositivo usando `Xamarin.Essentials`.
    -   **Acceso a Hardware:** Funciones para obtener la ubicación GPS y para tomar/seleccionar fotos (con soporte para recortar imágenes a través de `Stormlion.ImageCropper`).
-   **Página Base (`BaseContentPage<T>`):**
    -   Permite que las páginas modales devuelvan un valor de tipo `T` a la página que las invocó, facilitando la comunicación entre vistas.

## Dependencias

-   [Xamarin.Forms](https://www.nuget.org/packages/Xamarin.Forms/)
-   [Xamarin.Essentials](https://www.nuget.org/packages/Xamarin.Essentials/)
-   [Acr.UserDialogs](https://www.nuget.org/packages/Acr.UserDialogs/)
-   [Refractored.MvvmHelpers](https://www.nuget.org/packages/Refractored.MvvmHelpers/)
-   [SweetMeSoft.Base](https://www.nuget.org/packages/SweetMeSoft.Base/)
-   [SweetMeSoft.Connectivity](https://www.nuget.org/packages/SweetMeSoft.Connectivity/)
-   (Otras dependencias para notificaciones y recorte de imágenes)

## Instalación

```bash
dotnet add package SweetMeSoft.Forms
```

## Uso

### Heredar de `AppBaseViewModel`

Crea tus ViewModels heredando de `AppBaseViewModel` para acceder a toda su funcionalidad.

```csharp
using SweetMeSoft.Forms.ViewModels;
using System.Threading.Tasks;

public class MyViewModel : AppBaseViewModel
{
    public MyViewModel(INavigation navigation) : base(navigation)
    {
        // El constructor base requiere la instancia de INavigation
    }

    public async Task LoadData()
    {
        // Realiza una petición GET y deserializa la respuesta en MyDataModel
        var result = await Get<MyDataModel>("/my-api-endpoint");
        if (result != null)
        {
            // ... procesar el resultado
        }
    }

    public void NavigateToDetails()
    {
        // Navega a otra página
        GoTo<DetailsPage>();
    }
}
```

### Devolver un resultado desde una página modal

1.  **Define tu página modal heredando de `BaseContentPage<T>`**, donde `T` es el tipo de dato que quieres devolver.

```csharp
// Define el tipo de resultado que devolverá la página
public class MyModalResult
{
    public bool IsSuccess { get; set; }
    public string Data { get; set; }
}

// Crea la página
public partial class MyModalPage : BaseContentPage<MyModalResult>
{
    public MyModalPage()
    {
        InitializeComponent();
    }

    private void OnConfirmClicked(object sender, EventArgs e)
    {
        // Asigna el valor al campo 'result' heredado de BaseContentPage
        this.result = new MyModalResult { IsSuccess = true, Data = "Éxito" };
        
        // Cierra el modal (el evento OnDisappearing se encargará de enviar el resultado)
        (this.BindingContext as AppBaseViewModel)?.CloseModal();
    }
}
```

2.  **Usa `OpenModal` desde tu ViewModel para abrir la página y esperar el resultado.**

```csharp
public async Task ShowModal()
{
    var modalResult = await OpenModal<MyModalPage, MyModalResult>();
    
    // modalResult contendrá el valor asignado en MyModalPage
    if (modalResult != null && modalResult.IsSuccess)
    {
        await DisplayAlert("Resultado", modalResult.Data, "Ok");
    }
}
```

## Licencia

Este proyecto está bajo la licencia MIT. 