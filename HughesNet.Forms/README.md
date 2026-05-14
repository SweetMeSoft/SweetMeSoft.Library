# SweetMeSoft.Forms

Library with a set of base classes and utilities to accelerate application development with Xamarin.Forms following the MVVM pattern.

## Description

`SweetMeSoft.Forms` is a library for .NET Standard 2.1 that provides a robust `AppBaseViewModel` and a generic `BaseContentPage`. Its goal is to standardize and simplify common tasks in mobile application development, such as navigation, user interactions, API calls and the use of device functionalities.

## Features

-   **Base ViewModel (`AppBaseViewModel`):**
    -   **Simplified Navigation:** Methods to navigate between pages, open modals (with ability to return results) and handle the navigation stack (`GoTo`, `OpenModal`, `BackToRoot`, etc.).
    -   **User Dialogs:** Wrappers for `Acr.UserDialogs` that facilitate showing alerts, confirmations and loading spinners.
    -   **API Wrapper:** `Get` and `Post` methods that integrate with `SweetMeSoft.Connectivity` to perform HTTP requests, automatically handling authentication, connectivity and common errors (e.g. `401 Unauthorized`).
    -   **Permission Management:** Helpers to verify and request device permissions using `Xamarin.Essentials`.
    -   **Hardware Access:** Functions to get GPS location and to take/select photos (with support for cropping images through `Stormlion.ImageCropper`).
-   **Base Page (`BaseContentPage<T>`):**
    -   Allows modal pages to return a value of type `T` to the page that invoked them, facilitating communication between views.

## Dependencies

-   [Xamarin.Forms](https://www.nuget.org/packages/Xamarin.Forms/)
-   [Xamarin.Essentials](https://www.nuget.org/packages/Xamarin.Essentials/)
-   [Acr.UserDialogs](https://www.nuget.org/packages/Acr.UserDialogs/)
-   [Refractored.MvvmHelpers](https://www.nuget.org/packages/Refractored.MvvmHelpers/)
-   [SweetMeSoft.Base](https://www.nuget.org/packages/SweetMeSoft.Base/)
-   [SweetMeSoft.Connectivity](https://www.nuget.org/packages/SweetMeSoft.Connectivity/)
-   (Other dependencies for notifications and image cropping)

## Installation

```bash
dotnet add package SweetMeSoft.Forms
```

## Usage

### Inherit from `AppBaseViewModel`

Create your ViewModels inheriting from `AppBaseViewModel` to access all its functionality.

```csharp
using SweetMeSoft.Forms.ViewModels;
using System.Threading.Tasks;

public class MyViewModel : AppBaseViewModel
{
    public MyViewModel(INavigation navigation) : base(navigation)
    {
        // The base constructor requires the INavigation instance
    }

    public async Task LoadData()
    {
        // Performs a GET request and deserializes the response into MyDataModel
        var result = await Get<MyDataModel>("/my-api-endpoint");
        if (result != null)
        {
            // ... process the result
        }
    }

    public void NavigateToDetails()
    {
        // Navigate to another page
        GoTo<DetailsPage>();
    }
}
```

### Return a result from a modal page

1.  **Define your modal page inheriting from `BaseContentPage<T>`**, where `T` is the type of data you want to return.

```csharp
// Define the result type that the page will return
public class MyModalResult
{
    public bool IsSuccess { get; set; }
    public string Data { get; set; }
}

// Create the page
public partial class MyModalPage : BaseContentPage<MyModalResult>
{
    public MyModalPage()
    {
        InitializeComponent();
    }

    private void OnConfirmClicked(object sender, EventArgs e)
    {
        // Assign the value to the 'result' field inherited from BaseContentPage
        this.result = new MyModalResult { IsSuccess = true, Data = "Success" };
        
        // Close the modal (the OnDisappearing event will handle sending the result)
        (this.BindingContext as AppBaseViewModel)?.CloseModal();
    }
}
```

2.  **Use `OpenModal` from your ViewModel to open the page and wait for the result.**

```csharp
public async Task ShowModal()
{
    var modalResult = await OpenModal<MyModalPage, MyModalResult>();
    
    // modalResult will contain the value assigned in MyModalPage
    if (modalResult != null && modalResult.IsSuccess)
    {
        await DisplayAlert("Result", modalResult.Data, "Ok");
    }
}
```

## License

This project is under the MIT license.