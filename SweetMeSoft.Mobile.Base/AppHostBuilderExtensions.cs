using Controls.UserDialogs.Maui;

using SweetMeSoft.Base;

namespace SweetMeSoft.Mobile.Base;

public static class AppHostBuilderExtensions
{
    public static MauiAppBuilder UseSweetMeSoftBase(this MauiAppBuilder builder, string apiUrl)
    {
        builder.UseUserDialogs();
        Constants.API_URL = apiUrl;

        return builder;
    }
}
