using Controls.UserDialogs.Maui;
using MvvmHelpers;
using System.Net;
using static Microsoft.Maui.ApplicationModel.Permissions;
using SweetMeSoft.Connectivity;
using SweetMeSoft.Base.Connectivity;
using SweetMeSoft.Base;

namespace SweetMeSoft.Mobile.Base.ViewModels;

public class AppBaseViewModel : BaseViewModel
{
    public AppBaseViewModel()
    {
    }

    public void DisplayAlert(string title, string message, string cancel)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await UserDialogs.Instance.AlertAsync(message, title, cancel);
        });
    }

    public async Task<bool> DisplayAlert(string title, string message, string okText, string cancelText)
    {
        return await UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText);
    }

    public async Task<string> DisplayPrompt(string title, string message, string accept = "Ok", string cancel = "Cancel", Keyboard keyboard = null, string initial = "")
    {
        return await Application.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, "", 50, keyboard, initial);
    }

    public void GoTo<TClass>(TClass newPage, bool removePrevious = false) where TClass : Page
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Application.Current.MainPage.Navigation.PushAsync(newPage, true);
            if (removePrevious)
            {
                var count = Application.Current.MainPage.Navigation.NavigationStack.Count;
                if (count >= 1)
                {
                    Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[count - 2]);
                }
            }
        });
    }

    public void GoTo<TClass>(bool removePrevious = false) where TClass : Page, new()
    {
        GoTo(new TClass(), removePrevious);
    }

    public void GoToNewRoot<TClass>(TClass newPage) where TClass : Page, new()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            Application.Current.MainPage.Navigation.InsertPageBefore(newPage, Application.Current.MainPage.Navigation.NavigationStack[0]);
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        });
    }

    public void GoToNewRoot<TClass>() where TClass : Page, new()
    {
        GoToNewRoot(new TClass());
    }

    public void BackToRoot()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        });
    }

    public void GoBack()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        });
    }

    public async Task<TRes> Get<TReq, TRes>(string url, TReq data, bool useToken = true, bool showLoading = true) where TReq : class
    {
        if (Microsoft.Maui.Networking.Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await UserDialogs.Instance.AlertAsync("Revisa tu conexión a internet para poder continuar", "Error", "Ok");
            return default;
        }

        if (showLoading)
        {
            UserDialogs.Instance.ShowLoading("Espera por favor...");
        }

        var token = useToken ? Preferences.Get(Constants.KEY_JWT_TOKEN, string.Empty) : string.Empty;
        var response = await ApiRequest.Instance.Get<TReq, TRes>(new GenericRequest<TReq>
        {
            Url = url,
            Data = data,
            Authentication = string.IsNullOrEmpty(token) ? null : new Authentication
            {
                Type = AuthenticationType.Bearer,
                Value = token
            }
        });

        return await ManageResponse(response, url, showLoading);
    }

    public async Task<TRes> Post<TReq, TRes>(string url, TReq data, bool useToken = true, bool showLoading = true)
    {
        if (Microsoft.Maui.Networking.Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await UserDialogs.Instance.AlertAsync("Revisa tu conexión a internet para poder continuar", "Error", "Ok");
            return default;
        }

        if (showLoading)
        {
            UserDialogs.Instance.ShowLoading("Espera por favor...");
        }

        var token = useToken ? Preferences.Get(Constants.KEY_JWT_TOKEN, string.Empty) : string.Empty;
        var response = await ApiRequest.Instance.Post<TReq, TRes>(new GenericRequest<TReq>
        {
            Url = url,
            Data = data,
            Authentication = string.IsNullOrEmpty(token) ? null : new Authentication
            {
                Type = AuthenticationType.Bearer,
                Value = token
            }
        });

        return await ManageResponse(response, url, showLoading);
    }

    public async Task<Location> GetCurrentLocation()
    {
        try
        {
            var status = await CheckStatusAsync<LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await RequestAsync<LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                UserDialogs.Instance.ShowLoading("Obteniendo ubicación actual...");
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request, new CancellationTokenSource().Token);
                UserDialogs.Instance.HideHud();
                return location;
            }
        }
        catch (Exception ex)
        {
            UserDialogs.Instance.HideHud();
            await UserDialogs.Instance.AlertAsync(ex.Message, "Service Error", "Ok");
        }

        return default;
    }

    public async Task<PermissionStatus> CheckAndRequestPermissionAsync<TPermission>() where TPermission : BasePermission, new()
    {
        TPermission permission = new();
        var status = await permission.CheckStatusAsync();
        if (status != PermissionStatus.Granted)
        {
            status = await permission.RequestAsync();
            if (status != PermissionStatus.Granted)
            {
                await UserDialogs.Instance.AlertAsync("Ubicación solicitada", "Permiso requerido", "Ok");
            }
        }

        return status;
    }

    //public async Task TakePhoto(CameraOptions options)
    //{
    //    if (options.CropperOptions != null)
    //    {
    //        UserDialogs.Instance.ShowLoading(Languages.TakingPhoto);
    //        var cropper = new ImageCropper()
    //        {
    //            AspectRatioX = options.CropperOptions.IsSquare ? 1 : 0,
    //            AspectRatioY = options.CropperOptions.IsSquare ? 1 : 0,
    //            CropShape = options.CropperOptions.IsOval ? ImageCropper.CropShapeType.Oval : ImageCropper.CropShapeType.Rectangle,
    //            PageTitle = Languages.PhotoProfile,
    //            CancelButtonTitle = Languages.Cancel,
    //            CropButtonTitle = Languages.Crop,
    //            PhotoLibraryTitle = Languages.Gallery,
    //            SelectSourceTitle = Languages.SelectSource,
    //            TakePhotoTitle = Languages.TakePhoto,
    //            MediaPickerOptions = new MediaPickerOptions()
    //            {
    //                Title = Languages.PhotoProfile,
    //            },
    //            Faiure = (error) =>
    //            {
    //                UserDialogs.Instance.HideHud();
    //                options.FailureAction?.Invoke(error.ToString());
    //            },
    //            Success = (imageFile) =>
    //            {
    //                UserDialogs.Instance.HideHud();
    //                Device.BeginInvokeOnMainThread(async () =>
    //                {
    //                    var fs = new FileStream(imageFile, FileMode.Open, FileAccess.Read);
    //                    var ms = new MemoryStream();
    //                    await fs.CopyToAsync(ms);
    //                    fs.Position = 0;
    //                    ms.Position = 0;

    //                    options.SuccessAction?.Invoke(new PhotoResult
    //                    {
    //                        ImagePath = imageFile,
    //                        StreamFile = new StreamFile(DateTime.Now.ToString("yyyy-MM-dd HHmmss"), ms, Constants.ContentType.png),
    //                        ImageSource = ImageSource.FromStream(() => fs)
    //                    });
    //                });
    //            }
    //        };

    //        cropper.Show(options.CropperOptions.Page);
    //    }
    //    else
    //    {
    //        var result = await MediaPicker.CapturePhotoAsync();
    //        if (result != null)
    //        {
    //            if (Device.RuntimePlatform == "Android")
    //            {
    //                await Task.Delay(TimeSpan.FromMilliseconds(1000.0));
    //            }

    //            Device.BeginInvokeOnMainThread(async () =>
    //            {
    //                try
    //                {
    //                    var filepath = Device.RuntimePlatform == "Android" ? result.FullPath : Path.Combine(FileSystem.CacheDirectory, result.FileName);

    //                    var fs = new FileStream(result.FullPath, FileMode.Open, FileAccess.Read);
    //                    var ms = new MemoryStream();
    //                    await fs.CopyToAsync(ms);
    //                    fs.Position = 0;
    //                    ms.Position = 0;

    //                    options.SuccessAction?.Invoke(new PhotoResult
    //                    {
    //                        ImagePath = filepath,
    //                        StreamFile = new StreamFile(DateTime.Now.ToString("yyyy-MM-dd HHmmss"), ms, Constants.ContentType.png),
    //                        ImageSource = ImageSource.FromStream(() => fs)
    //                    });
    //                }
    //                catch (Exception ex)
    //                {
    //                    options.FailureAction?.Invoke(ex.Message);
    //                }
    //            });
    //        }
    //        else
    //        {
    //            options.FailureAction?.Invoke(Languages.NoFileSelected);
    //        }
    //    }
    //}

    private async Task<TRes> ManageResponse<TRes>(GenericResponse<TRes> response, string path, bool showLoading)
    {
        try
        {
            if (showLoading)
            {
                UserDialogs.Instance.HideHud();
            }

            if (response.HttpResponse.IsSuccessStatusCode)
            {
                if (response.HttpResponse.Headers.TryGetValues("jwt", out var token))
                {
                    Preferences.Set(Constants.KEY_JWT_TOKEN, token.First());
                }
                return response.Object;
            }

            switch (response.HttpResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    await UserDialogs.Instance.AlertAsync(path + ":\n\r\n\r", "The service " + path + " does not exist.", "Ok");
                    return default;
            }

            await UserDialogs.Instance.AlertAsync(path + ":\n\r\n\r" + response.Error.Title, "Service Error", "Ok");
        }
        catch (Exception ex)
        {
            await UserDialogs.Instance.AlertAsync(path + ":\n\r\n\r" + ex.Message, "Service Error", "Ok");
        }

        return default;
    }
}
