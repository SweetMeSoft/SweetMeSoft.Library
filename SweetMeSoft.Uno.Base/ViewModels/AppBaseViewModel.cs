using Microsoft.UI.Xaml.Controls;

using SweetMeSoft.Base;
using SweetMeSoft.Base.Connectivity;
using SweetMeSoft.Connectivity;
using SweetMeSoft.Uno.Base.i18n.Resources;
using SweetMeSoft.Uno.Base.Models;
using SweetMeSoft.Uno.Base.Tools;

using System.Globalization;
using System.Net;

using Windows.Networking.Connectivity;

namespace SweetMeSoft.Uno.Base.ViewModels;

public class AppBaseViewModel() : NavigationViewModel
{
    public static int loadingCounter = 0;

    public Action UpdateView = () => { };

    //public async Task<PermissionStatus> CheckAndRequestPermissionAsync<TPermission>() where TPermission : BasePermission, new()
    //{
    //    TPermission permission = new();
    //    var status = await permission.CheckStatusAsync();
    //    if (status != PermissionStatus.Granted)
    //    {
    //        status = await permission.RequestAsync();
    //        if (status != PermissionStatus.Granted)
    //        {
    //            await DisplayAlert("No se ha otorgado el permiso " + typeof(TPermission).Name + ". Puedes hacerlo en el menú de configuraciones.", "Error", "Ok");
    //        }
    //    }

    //    return status;
    //}

    //public void UpdateInLoop(int ticks = 10, int threshold = 1000)
    //{
    //    var timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
    //    timer.Interval = TimeSpan.FromMilliseconds(threshold);
    //    timer.Tick += (sender, args) =>
    //    {
    //        if (ticks == 0)
    //        {
    //            timer.Stop();
    //            return;
    //        }

    //        UpdateView();
    //        ticks--;
    //    };
    //    timer.Start();
    //}

    public async Task<TRes> Get<TRes>(string url, bool useToken = true, bool showLoading = true)
    {
        return await Get<string, TRes>(url, null, useToken, showLoading);
    }

    public async Task<TRes> Get<TReq, TRes>(string url, TReq data, bool useToken = true, bool showLoading = true) where TReq : class
    {
        try
        {
            if (NetworkInformation.GetInternetConnectionProfile() == null)
            {
                await DisplayAlert(Resources.NoInternet, Resources.CheckInternetConnection, Resources.Ok);
                return default;
            }

            if (showLoading)
            {
                loadingCounter++;
                DisplayLoading(Resources.Loading);
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
                HideLoading();
            }

            await DisplayAlert(Resources.Error, ex.Message, Resources.Ok);
            throw;
        }
    }

    //public Color GetColorFromResource(string colorName)
    //{
    //    var color = Application.Current.Resources[colorName] ?? throw new Exception("Color " + colorName + " not found in resources");
    //    try
    //    {
    //        return (Color)color;
    //    }
    //    catch
    //    {
    //        throw new Exception(colorName + " is not a Color");
    //    }
    //}

    public async Task<Location> GetCurrentLocation()
    {
        try
        {
            var access = await Windows.Devices.Geolocation.Geolocator.RequestAccessAsync();
            if (access == Windows.Devices.Geolocation.GeolocationAccessStatus.Allowed)
            {
                var geolocator = new Windows.Devices.Geolocation.Geolocator();
                var position = await geolocator.GetGeopositionAsync();
                return new Location(position.Coordinate.Point.Position.Latitude, position.Coordinate.Point.Position.Longitude);
            }

            return null;
        }
        catch (Exception ex)
        {
            HideLoading();
            await DisplayAlert(Resources.Error, ex.Message, Resources.Ok);
        }

        return default;
    }

    public async void Logout<T>(bool showConfirmation = true, Action action = null) where T : Page, new()
    {
        if (!showConfirmation || await DisplayAlert(Resources.CloseSession, Resources.AreYouSure, Resources.Yes, Resources.No))
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
            if (NetworkInformation.GetInternetConnectionProfile() == null)
            {
                await DisplayAlert(Resources.NoInternet, Resources.CheckInternetConnection, Resources.Ok);
                return default;
            }

            if (showLoading)
            {
                loadingCounter++;
                DisplayLoading(Resources.Loading);
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
                HideLoading();
            }

            await DisplayAlert(Resources.Error, ex.Message, Resources.Ok);
            throw;
        }
    }

    private async Task<TRes> ManageResponse<TRes>(GenericResponse<TRes> response, string path, bool showLoading)
    {
        try
        {
            if (showLoading && --loadingCounter == 0)
            {
                HideLoading();
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
                    await DisplayAlert(path + ":\n\r\n\r", "The service " + path + " does not exist.", Resources.Ok);
                    return default;
            }

            await DisplayAlert(Resources.Error, path + ":\n\r\n\r" + response.Error.Title, Resources.Ok);
        }
        catch (Exception ex)
        {
            await DisplayAlert(Resources.Error, path + ":\n\r\n\r" + ex.Message, Resources.Ok);
        }

        return default;
    }
}