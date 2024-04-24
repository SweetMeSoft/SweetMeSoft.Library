using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace SweetMeSoft.Uno.Base.Tools;

public class PhotoResult
{
    public string Path { get; set; }

    public string GetBase64()
    {
        var bytes = GetBytes();
        return Convert.ToBase64String(bytes);
    }

    public byte[] GetBytes()
    {
        using Stream sourceStream = File.OpenRead(Path);
        using var memoryStream = new MemoryStream();
        sourceStream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public ImageSource GetImageSource()
    {
        return new BitmapImage(new Uri(Path));
    }
}
