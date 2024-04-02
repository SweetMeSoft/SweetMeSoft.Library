namespace SweetMeSoft.Mobile.Base.Tools;

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
        return ImageSource.FromFile(Path);
    }
}