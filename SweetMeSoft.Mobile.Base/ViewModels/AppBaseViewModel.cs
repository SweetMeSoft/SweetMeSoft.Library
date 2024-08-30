using Controls.UserDialogs.Maui;

using SweetMeSoft.Base;
using SweetMeSoft.Base.Connectivity;
using SweetMeSoft.Connectivity;

using System.Globalization;
using System.Net;

using static Microsoft.Maui.ApplicationModel.Permissions;

namespace SweetMeSoft.Mobile.Base.ViewModels;

public class AppBaseViewModel : NavigationViewModel
{
    public AppBaseViewModel()
    {
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
                await DisplayAlert("No se ha otorgado el permiso " + typeof(TPermission).Name + ". Puedes hacerlo en el menú de configuraciones.", "Error", "Ok");
            }
        }

        return status;
    }

    public Task<TRes> Get<TRes>(string url, bool useToken = true, bool showLoading = true)
    {
        return Get<string, TRes>(url, null, useToken, showLoading);
    }

    public async Task<TRes> Get<TReq, TRes>(string url, TReq data, bool useToken = true, bool showLoading = true) where TReq : class
    {
        try
        {
            if (Microsoft.Maui.Networking.Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await UserDialogs.Instance.AlertAsync("Revisa tu conexión a internet para poder continuar", "Error", "Ok");
                return default;
            }

            if (showLoading)
            {
                loadingCounter++;
                UserDialogs.Instance.ShowLoading("Espera por favor...");
            }

            var token = useToken ? Preferences.Get(Constants.KEY_JWT_TOKEN, string.Empty) : string.Empty;
            var response = await ApiRequest.Instance.Get<TReq, TRes>(new GenericRequest<TReq>
            {
                Url = url.StartsWith("http") ? url : Constants.API_URL + url,
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

            return await ManageResponse(response, url, showLoading);
        }
        catch (Exception ex)
        {
            if (showLoading && --loadingCounter == 0)
            {
                UserDialogs.Instance.HideHud();
            }

            await UserDialogs.Instance.AlertAsync(ex.Message, "Service Error", "Ok");
            throw;
        }
    }

    public Color GetColorFromResource(string colorName)
    {
        var color = Application.Current.Resources[colorName] ?? throw new Exception("Color " + colorName + " not found in resources");
        try
        {
            return (Color)color;
        }
        catch
        {
            throw new Exception(colorName + " is not a Color");
        }
    }

    public async Task<Location> GetCurrentLocation(bool showMessages = false)
    {
        try
        {
            //#if ANDROID
            //            var lm = Platform.CurrentActivity.GetSystemService(Android.Content.Context.LocationService) as Android.Locations.LocationManager;
            //            var isenabled = lm.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider);
            //            // check if GPS is enabled
            //            if (isenabled == false)
            //            {
            //                if(showMessages){
            //                    Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.StartActivity(new Android.Content.Intent(Android.Provider.Settings.ActionLocationSourceSettings));
            //                }
            //                return default;
            //            }
            //#elif IOS
            //            if (CoreLocation.CLLocationManager.Status == CoreLocation.CLAuthorizationStatus.Denied)
            //            {
            //                if (showMessages)
            //                {
            //                    UserDialogs.Instance.Alert("No se ha otorgado el permiso de ubicación. Puedes hacerlo en el menú de configuraciones.", "Error", "Ok");
            //                }
            //                return default;
            //            }
            //#endif

            var status = await CheckStatusAsync<LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await RequestAsync<LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                UserDialogs.Instance.ShowLoading("Obteniendo ubicación actual...");
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLastKnownLocationAsync() ?? await Geolocation.GetLocationAsync(request, new CancellationTokenSource().Token);
                UserDialogs.Instance.HideHud();
                return location;
            }
        }
        catch (Exception)
        {
            UserDialogs.Instance.HideHud();
            //await UserDialogs.Instance.AlertAsync(ex.Message, "Service Error", "Ok");
        }

        return default;
    }

    public async void Logout<T>(bool showConfirmation = true, Action action = null) where T : Page, new()
    {
        if (!showConfirmation || await DisplayAlert("¿Estás seguro de que deseas cerrar sesión?", "Al cerrar sesión, se eliminarán todos los datos de tu cuenta en este dispositivo.", "Si", "No"))
        {
            var token = Preferences.Get(Constants.KEY_NOTIFICATIONS_TOKEN, "");
            Preferences.Remove(Constants.KEY_CURRENT_USER);
            Preferences.Remove(Constants.KEY_CURRENT_USER_ID);
            Preferences.Remove(Constants.KEY_NOTIFICATIONS_TOKEN);
            Preferences.Remove(Constants.KEY_JWT_TOKEN);
            Preferences.Remove(Constants.KEY_IS_USER_COMPLETE);
            Preferences.Remove(Constants.KEY_CURRENT_USER_TYPE);
            action?.Invoke();
            GoToNewRoot<T>();
        }
    }

    public async Task<TRes> Post<TReq, TRes>(string url, TReq data, bool useToken = true, bool showLoading = true)
    {
        try
        {
            if (Microsoft.Maui.Networking.Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await UserDialogs.Instance.AlertAsync("Revisa tu conexión a internet para poder continuar", "Error", "Ok");
                return default;
            }

            if (showLoading)
            {
                loadingCounter++;
                UserDialogs.Instance.ShowLoading("Espera por favor...");
            }

            var token = useToken ? Preferences.Get(Constants.KEY_JWT_TOKEN, string.Empty) : string.Empty;
            var response = await ApiRequest.Instance.Post<TReq, TRes>(new GenericRequest<TReq>
            {
                Url = url.StartsWith("http") ? url : Constants.API_URL + url,
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

            return await ManageResponse(response, url, showLoading);
        }
        catch (Exception ex)
        {
            if (showLoading && --loadingCounter == 0)
            {
                UserDialogs.Instance.HideHud();
            }

            await UserDialogs.Instance.AlertAsync(ex.Message, "Service Error", "Ok");
            throw;
        }
    }

    private async Task<TRes> ManageResponse<TRes>(GenericResponse<TRes> response, string path, bool showLoading)
    {
        try
        {
            if (showLoading && --loadingCounter == 0)
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

    internal static int loadingCounter = 0;
}