using Windows.Storage;

namespace SweetMeSoft.Uno.Base.Tools;

public class Preferences
{
    public static string Get(string key, string defaultValue)
    {
        try
        {
            var value = ApplicationData.Current.LocalSettings.Values[key] as string;
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public static int Get(string key, int defaultValue)
    {
        try
        {
            return (int)ApplicationData.Current.LocalSettings.Values[key];
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public static void Set(string key, string value)
    {
        ApplicationData.Current.LocalSettings.Values[key] = value;
    }

    public static void Set(string key, int value)
    {
        ApplicationData.Current.LocalSettings.Values[key] = value;
    }

    public static void Remove(string key)
    {
        ApplicationData.Current.LocalSettings.Values.Remove(key);
    }

    public static void Clear()
    {
        ApplicationData.Current.LocalSettings.Values.Clear();
    }
}
