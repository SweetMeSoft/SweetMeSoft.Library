# SweetMeSoft.Mobile.Base

Base library with a set of utilities, base classes and services to accelerate .NET MAUI application development.

## Description

`SweetMeSoft.Mobile.Base` is a library for .NET MAUI that provides a robust base architecture for mobile applications, following the MVVM pattern. It includes a base ViewModel with integrated functionalities, a popup service, and extensions for simple initial configuration.

## Features

-   **Simple Configuration (`AppHostBuilderExtensions`):**
    -   An extension method `UseSweetMeSoftBase` for `MauiAppBuilder` that initializes necessary dependencies like `CommunityToolkit.Maui` and `UserDialogs`.
-   **Base ViewModel (`AppBaseViewModel`):**
    -   **Navigation:** Inherits from `NavigationViewModel` (not included but implicit) for simple navigation management.
    -   **API Wrapper:** `Get` and `Post` methods that integrate with `SweetMeSoft.Connectivity` to perform HTTP requests. They automatically manage connectivity, authentication tokens and loading indicators.
    -   **Dialogs and Popups:** Integrates with `Acr.UserDialogs` for alerts and confirmations, and uses its own `PopupsService` to show a "loading" popup.
    -   **Permission Management:** Helpers to verify and request device permissions using MAUI APIs.
    -   **Hardware Access:** Functions to get GPS location.
    -   **Session Management:** A `Logout` method to clear user preferences and restart navigation.
-   **Popup Service (`PopupsService`):**
    -   A singleton service to show and hide a loading popup (`LoadingPopup`) consistently throughout the application.

## Dependencies

-   [Microsoft.Maui.Controls](https://www.nuget.org/packages/Microsoft.Maui.Controls)
-   [CommunityToolkit.Maui](https://www.nuget.org/packages/CommunityToolkit.Maui)
-   [Controls.UserDialogs.Maui](https://www.nuget.org/packages/Controls.UserDialogs.Maui)
-   [SweetMeSoft.Connectivity](https://www.nuget.org/packages/SweetMeSoft.Connectivity)

## Installation

```bash
dotnet add package SweetMeSoft.Mobile.Base
```

## Usage

### 1. Configuration in `MauiProgram.cs`

In your `MauiProgram.cs` file, use the `UseSweetMeSoftBase` method to register the library and its dependencies.

```csharp
using SweetMeSoft.Mobile.Base;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            // Call the extension method and pass your API base URL
            .UseSweetMeSoftBase("https://api.yourdomain.com") 
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ... Register your own views and viewmodels
        // builder.Services.AddSingleton<MyPage>();
        // builder.Services.AddSingleton<MyViewModel>();

        return builder.Build();
    }
}
```

### 2. Inherit from `AppBaseViewModel`

Create your ViewModels inheriting from `AppBaseViewModel` to access all the integrated functionality.

```csharp
using SweetMeSoft.Mobile.Base.ViewModels;
using CommunityToolkit.Mvvm.Input; // Required for [RelayCommand]
using System.Threading.Tasks;

public partial class MyViewModel : AppBaseViewModel
{
    // Observable properties can be defined with CommunityToolkit.Mvvm
    // [ObservableProperty]
    // private string myData;

    public MyViewModel()
    {
        // AppBaseViewModel handles navigation through Shell or NavigationPage
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        // The 'true' in showLoading is the default value, which
        // will automatically activate PopupsService.ShowLoading/HideLoading.
        var result = await Get<MyDataModel>("/my-api/endpoint", showLoading: true);
        
        if (result != null)
        {
            // ... process the result
            // MyData = result.SomeProperty;
        }
    }

    [RelayCommand]
    private async Task GoToDetailsAsync()
    {
        // Navigate to another page registered in the DI Container
        await GoToAsync<DetailsPage>();
    }
}
```

### 3. Usage in View (XAML)

Bind your ViewModel commands to your view controls.

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:MyProject.ViewModels"
             x:Class="MyProject.MyPage"
             x:DataType="vm:MyViewModel">

    <VerticalStackLayout>
        <Button Text="Load Data" Command="{Binding LoadDataCommand}" />
        <Button Text="View Details" Command="{Binding GoToDetailsCommand}" />
    </VerticalStackLayout>
    
</ContentPage>
```

## License

This project is under the MIT license.