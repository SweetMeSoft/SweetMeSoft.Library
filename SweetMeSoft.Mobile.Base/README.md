# SweetMeSoft.Mobile.Base

Librería base con un conjunto de utilidades, clases base y servicios para acelerar el desarrollo de aplicaciones .NET MAUI.

## Descripción

`SweetMeSoft.Mobile.Base` es una librería para .NET MAUI que proporciona una arquitectura base robusta para aplicaciones móviles, siguiendo el patrón MVVM. Incluye un ViewModel base con funcionalidades integradas, un servicio de popups, y extensiones para una configuración inicial sencilla.

## Características

-   **Configuración Sencilla (`AppHostBuilderExtensions`):**
    -   Un método de extensión `UseSweetMeSoftBase` para `MauiAppBuilder` que inicializa las dependencias necesarias como `CommunityToolkit.Maui` y `UserDialogs`.
-   **ViewModel Base (`AppBaseViewModel`):**
    -   **Navegación:** Hereda de `NavigationViewModel` (no incluido pero implícito) para una gestión de navegación simple.
    -   **Wrapper de API:** Métodos `Get` y `Post` que se integran con `SweetMeSoft.Connectivity` para realizar peticiones HTTP. Gestionan automáticamente la conectividad, los tokens de autenticación y los indicadores de carga.
    -   **Diálogos y Popups:** Se integra con `Acr.UserDialogs` para alertas y confirmaciones, y utiliza un `PopupsService` propio para mostrar un popup de "cargando".
    -   **Gestión de Permisos:** Helpers para verificar y solicitar permisos del dispositivo usando las APIs de MAUI.
    -   **Acceso a Hardware:** Funciones para obtener la ubicación GPS.
    -   **Gestión de Sesión:** Un método `Logout` para limpiar las preferencias de usuario y reiniciar la navegación.
-   **Servicio de Popups (`PopupsService`):**
    -   Un servicio singleton para mostrar y ocultar un popup de carga (`LoadingPopup`) de forma consistente en toda la aplicación.

## Dependencias

-   [Microsoft.Maui.Controls](https://www.nuget.org/packages/Microsoft.Maui.Controls)
-   [CommunityToolkit.Maui](https://www.nuget.org/packages/CommunityToolkit.Maui)
-   [Controls.UserDialogs.Maui](https://www.nuget.org/packages/Controls.UserDialogs.Maui)
-   [SweetMeSoft.Connectivity](https://www.nuget.org/packages/SweetMeSoft.Connectivity)

## Instalación

```bash
dotnet add package SweetMeSoft.Mobile.Base
```

## Uso

### 1. Configuración en `MauiProgram.cs`

En tu archivo `MauiProgram.cs`, utiliza el método `UseSweetMeSoftBase` para registrar la librería y sus dependencias.

```csharp
using SweetMeSoft.Mobile.Base;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            // Llama al método de extensión y pasa la URL base de tu API
            .UseSweetMeSoftBase("https://api.tudominio.com") 
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ... Registra tus propias vistas y viewmodels
        // builder.Services.AddSingleton<MyPage>();
        // builder.Services.AddSingleton<MyViewModel>();

        return builder.Build();
    }
}
```

### 2. Heredar de `AppBaseViewModel`

Crea tus ViewModels heredando de `AppBaseViewModel` para acceder a toda la funcionalidad integrada.

```csharp
using SweetMeSoft.Mobile.Base.ViewModels;
using CommunityToolkit.Mvvm.Input; // Necesario para [RelayCommand]
using System.Threading.Tasks;

public partial class MyViewModel : AppBaseViewModel
{
    // Las propiedades observables se pueden definir con el CommunityToolkit.Mvvm
    // [ObservableProperty]
    // private string myData;

    public MyViewModel()
    {
        // El AppBaseViewModel se encarga de la navegación a través del Shell o NavigationPage
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        // El 'true' en showLoading es el valor por defecto, lo que
        // activará automáticamente el PopupsService.ShowLoading/HideLoading.
        var result = await Get<MyDataModel>("/my-api/endpoint", showLoading: true);
        
        if (result != null)
        {
            // ... procesar el resultado
            // MyData = result.SomeProperty;
        }
    }

    [RelayCommand]
    private async Task GoToDetailsAsync()
    {
        // Navega a otra página registrada en el DI Container
        await GoToAsync<DetailsPage>();
    }
}
```

### 3. Uso en la Vista (XAML)

Enlaza los comandos de tu ViewModel a los controles de tu vista.

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MyProject.ViewModels"
             x:Class="MyProject.MyPage"
             x:DataType="vm:MyViewModel">

    <VerticalStackLayout>
        <Button Text="Cargar Datos" Command="{Binding LoadDataCommand}" />
        <Button Text="Ver Detalles" Command="{Binding GoToDetailsCommand}" />
    </VerticalStackLayout>
    
</ContentPage>
```

## Licencia

Este proyecto está bajo la licencia MIT. 