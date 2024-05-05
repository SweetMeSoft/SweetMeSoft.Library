using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

using Windows.Devices.Sensors;

namespace SweetMeSoft.Uno.Base.Tools;

public class PhotoResult
{
    public string OriginalPath { get; set; }

    public byte[] EditedBytes { get; set; }

    public SimpleOrientation Orientation { get; set; }

    public string GetBase64()
    {
        var bytes = GetBytes();
        return Convert.ToBase64String(bytes);
    }

    public byte[] GetBytes()
    {
        using Stream sourceStream = File.OpenRead(OriginalPath);
        using var memoryStream = new MemoryStream();
        sourceStream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public ImageSource GetImageSource()
    {
        return new BitmapImage(new Uri(OriginalPath));
    }

    public Stream GetStream()
    {
        return File.OpenRead(OriginalPath);
    }
}