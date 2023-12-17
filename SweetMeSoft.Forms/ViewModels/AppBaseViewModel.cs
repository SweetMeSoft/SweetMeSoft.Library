using Acr.UserDialogs;
using CTCNetwork.Base;
using CTCNetwork.Base.Objects.Tools;
using CTCNetwork.Employee.Interfaces;
using CTCNetwork.Employee.Pages.Common;
using Microsoft.AppCenter.Crashes;

using MvvmHelpers;

using Plugin.FirebasePushNotification;

using Stormlion.ImageCropper;

using SweetMeSoft.Base;
using SweetMeSoft.Base.Connectivity;
using SweetMeSoft.Connectivity;
using SweetMeSoft.Forms.Pages;
using System.Globalization;
using System.Net;
using System.Text.Json;

using Xamarin.Essentials;
using Xamarin.Forms;

using static Stormlion.ImageCropper.ImageCropper;
using static Xamarin.Essentials.Permissions;

namespace SweetMeSoft.Forms.ViewModels
{
    public class AppBaseViewModel : BaseViewModel
    {
        private int loadingQuantity = 0;
        public INavigation Navigation { get; set; }

        public AppBaseViewModel()
        {
        }

        public AppBaseViewModel(INavigation navigation)
        {
            Navigation = navigation;
        }

        public async Task DisplayAlert(string title, string message, string cancel)
        {
            await UserDialogs.Instance.AlertAsync(message, title, cancel);
        }

        public async Task<bool> DisplayAlert(string title, string message, string okText, string cancelText)
        {
            return await UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText);
        }

        public void GoTo<TClass>(TClass newPage, bool removePrevious = false) where TClass : Page
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushAsync(newPage, true);
                if (removePrevious)
                {
                    var count = Navigation.NavigationStack.Count;
                    if (count >= 1)
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[count - 2]);
                    }
                }
            });
        }

        public void GoTo<TClass>(bool removePrevious = false) where TClass : Page, new()
        {
            GoTo(new TClass(), removePrevious);
        }

        public async Task<T> OpenModal<TClass, T>() where TClass : BaseContentPage<T>, new()
        {
            return await OpenModal<TClass, T>(new TClass());
        }

        public async Task<T> OpenModal<TClass, T>(TClass newPage) where TClass : BaseContentPage<T>
        {
            var source = new TaskCompletionSource<T>();
            newPage.PageDisappearing += (result) =>
            {
                var res = (T)Convert.ChangeType(result, typeof(T));
                source.SetResult(res);
            };

            await Navigation.PushModalAsync(new NavigationPage(newPage), true);
            return await source.Task;
        }

        public void CloseModal()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PopModalAsync(true);
            });
        }


        public void GoToNewRoot<TClass>(TClass newPage) where TClass : Page, new()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Navigation.InsertPageBefore(newPage, Navigation.NavigationStack[0]);
                await Navigation.PopToRootAsync();
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
                await Navigation.PopToRootAsync();
            });
        }

        public void GoBack()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PopAsync();
            });
        }

        public async Task<TRes> Get<TRes>(string url, bool useToken = true)
        {
            return await Get<string, TRes>(url, null, useToken);
        }

        public async Task<TRes> Get<TReq, TRes>(string path, TReq data, bool useToken = true) where TReq : class
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await UserDialogs.Instance.AlertAsync(Languages.NoInternet, Languages.Error, Languages.Ok);
                return default;
            }

            HandleLoading(true);
            var token = useToken ? Preferences.Get(Constants.KEY_JWT_TOKEN, string.Empty) : string.Empty;
            var response = await ApiRequest.Instance.GetRequest<TReq, TRes>(new GenericRequest<TReq>
            {
                Url = Constants.API_URL + path,
                Data = data,
                Authentication = string.IsNullOrEmpty(token) ? null : new Authentication
                {
                    Type = AuthenticationType.Bearer,
                    Value = token
                },
                Headers = new Dictionary<string, string>
                {
                    { "CurrentDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "Language", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName }
                }
            });

            return await ManageResponse(response, path);
        }

        public async Task<TRes> Post<TReq, TRes>(string path, TReq data, bool useToken = true)
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await UserDialogs.Instance.AlertAsync(Languages.NoInternet, Languages.Error, Languages.Ok);
                return default;
            }

            HandleLoading(true);
            var token = useToken ? Preferences.Get(Constants.KEY_JWT_TOKEN, string.Empty) : string.Empty;
            var response = await ApiRequest.Instance.PostRequest<TReq, TRes>(new GenericRequest<TReq>
            {
                Url = Constants.API_URL + path,
                Data = data,
                Authentication = string.IsNullOrEmpty(token) ? null : new Authentication
                {
                    Type = AuthenticationType.Bearer,
                    Value = token
                },
                Headers = new Dictionary<string, string>
                {
                    { "CurrentDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "Language", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName }
                }
            });

            return await ManageResponse(response, path);
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
                    UserDialogs.Instance.ShowLoading(Languages.GettingCurrentLocation);
                    var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                    var location = await Geolocation.GetLocationAsync(request, new CancellationTokenSource().Token);
                    UserDialogs.Instance.HideLoading();
                    return location;
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                await UserDialogs.Instance.AlertAsync(ex.Message, "Service Error", Languages.Ok);
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
                    await UserDialogs.Instance.AlertAsync(Languages.EnableLocation, Languages.PermissionRequired, Languages.Ok);
                }
            }

            return status;
        }

        public async Task TakePhoto(CameraOptions options)
        {
            if (options.CropperOptions != null)
            {
                UserDialogs.Instance.ShowLoading(Languages.TakingPhoto);
                var cropper = new ImageCropper()
                {
                    AspectRatioX = options.CropperOptions.IsSquare ? 1 : 0,
                    AspectRatioY = options.CropperOptions.IsSquare ? 1 : 0,
                    CropShape = options.CropperOptions.IsOval ? CropShapeType.Oval : CropShapeType.Rectangle,
                    PageTitle = Languages.PhotoProfile,
                    CancelButtonTitle = Languages.Cancel,
                    CropButtonTitle = Languages.Crop,
                    PhotoLibraryTitle = Languages.Gallery,
                    SelectSourceTitle = Languages.SelectSource,
                    TakePhotoTitle = Languages.TakePhoto,
                    MediaPickerOptions = new MediaPickerOptions()
                    {
                        Title = Languages.PhotoProfile,
                    },
                    Faiure = (error) =>
                    {
                        if (error != ResultErrorType.None && error != ResultErrorType.CroppingCancelled)
                        {
                            Crashes.TrackError(new Exception(error.ToString()));
                        }

                        UserDialogs.Instance.HideLoading();
                        options.FailureAction?.Invoke(new Exception(error.ToString()));
                    },
                    Success = (imageFile) =>
                    {
                        UserDialogs.Instance.HideLoading();
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ManagePhotoResult(imageFile, options);
                        });
                    }
                };

                cropper.Show(options.CropperOptions.Page);
            }
            else
            {
                var result = await MediaPicker.CapturePhotoAsync();
                if (result != null)
                {
                    if (Device.RuntimePlatform == "Android")
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1000.0));
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var filepath = Device.RuntimePlatform == "Android" ? result.FullPath : Path.Combine(FileSystem.CacheDirectory, result.FileName);
                        ManagePhotoResult(filepath, options);
                    });
                }
                else
                {
                    options.FailureAction?.Invoke(new Exception(Languages.NoFileSelected));
                }
            }
        }

        private void ManagePhotoResult(string imageFile, CameraOptions options)
        {
            try
            {
                var fs = new FileStream(imageFile, FileMode.Open, FileAccess.Read);
                var dep = DependencyService.Get<IImageTools>();
                var resized = dep.Reduce(fs);

                options.SuccessAction?.Invoke(new PhotoResult
                {
                    ImagePath = imageFile,
                    StreamFile = new StreamFile(DateTime.Now.ToString("yyyy-MM-dd HHmmss"), new MemoryStream(resized), Constants.ContentType.png),
                    ImageSource = ImageSource.FromStream(() => new MemoryStream(resized))
                });
            }
            catch (Exception ex)
            {
                options.FailureAction?.Invoke(ex);
            }
        }

        private void HandleLoading(bool show)
        {
            if (show)
            {
                if (loadingQuantity == 0)
                {
                    UserDialogs.Instance.ShowLoading(Languages.PleaseWait);
                }

                loadingQuantity++;
            }
            else
            {
                loadingQuantity--;
                if (loadingQuantity == 0)
                {
                    UserDialogs.Instance.HideLoading();
                }
            }
        }

        private async Task<TRes> ManageResponse<TRes>(GenericResponse<TRes> response, string path)
        {
            try
            {
                HandleLoading(false);
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
                        await UserDialogs.Instance.AlertAsync(path + ":\n\r\n\r", "The service " + path + " does not exist.", Languages.Ok);
                        return default;
                    case HttpStatusCode.Unauthorized:
                        Logout(false);
                        return default;
                }

                Crashes.TrackError(new Exception(JsonSerializer.Serialize(response.Error)));
                await UserDialogs.Instance.AlertAsync(path + ":\n\r\n\r" + response.Error.Title, "Service Error", Languages.Ok);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await UserDialogs.Instance.AlertAsync(path + ":\n\r\n\r" + ex.Message, "Service Error", Languages.Ok);
            }

            return default;
        }

        public async void Logout(bool showConfirmation = true)
        {
            if (showConfirmation && await DisplayAlert(Languages.CloseSession, Languages.EndThisSession, Languages.EndSession, Languages.GoBack))
            {
                var token = Preferences.Get(CTCNetworkConstants.KEY_NOTIFICATIONS_TOKEN, "");
                await Post<string, bool>("Notification/DeleteToken?token=" + token, null);
                Preferences.Remove(CTCNetworkConstants.KEY_CURRENT_USER);
                Preferences.Remove(CTCNetworkConstants.KEY_CURRENT_USER_ID);
                Preferences.Remove(CTCNetworkConstants.KEY_NOTIFICATIONS_TOKEN);
                Preferences.Remove(CTCNetworkConstants.KEY_JWT_TOKEN);
                Preferences.Remove(CTCNetworkConstants.KEY_IS_USER_COMPLETE);
                Preferences.Remove(CTCNetworkConstants.KEY_CURRENT_USER_TYPE);
                CrossFirebasePushNotification.Current.UnregisterForPushNotifications();

                GoToNewRoot<LoginPage>();
            }
        }
    }
}
