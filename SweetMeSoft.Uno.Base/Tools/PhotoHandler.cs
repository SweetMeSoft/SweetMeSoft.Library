using SkiaSharp;

using System.Drawing;

using Windows.Devices.Sensors;
using Windows.Media.Capture;
using Windows.Storage;

namespace SweetMeSoft.Uno.Base.Tools;

public class PhotoHandler
{
    //TODO Storage the new photo in a temp folder
    public async Task<PhotoResult> TakePhoto()
    {
        try
        {
            var photo = await capturer.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (photo == null)
            {
                return null;
            }
            else
            {
                var orientation = SimpleOrientationSensor.GetDefault()?.GetCurrentOrientation() ?? SimpleOrientation.NotRotated;
                var bytes = GetFixedPhoto(photo.Path, orientation);
                var newPath = Path.Combine(ApplicationData.Current.TemporaryFolder.Path, Guid.NewGuid().ToString());

                File.Delete(photo.Path);
                File.WriteAllBytes(newPath, bytes);

                return new PhotoResult
                {
                    OriginalPath = newPath,
                    EditedBytes = bytes,
                    Orientation = orientation
                };
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }

        return null;
    }

    public async Task<List<PhotoResult>> TakePhotos()
    {
        var results = new List<PhotoResult>();
        PhotoResult photo;
        do
        {
            photo = await TakePhoto();
            if (photo != null)
            {
                results.Add(photo);
            }

            await Task.Delay(1000);
        }
        while (photo != null);
        return results;
    }

    private byte[] GetFixedPhoto(string path, SimpleOrientation orientation)
    {
        using var sourceStream = File.OpenRead(path);
        using var memoryStream = new MemoryStream();
        sourceStream.CopyTo(memoryStream);
        var bytes = memoryStream.ToArray();

        using SKBitmap image = SKBitmap.Decode(bytes);
        if (orientation == SimpleOrientation.NotRotated && image.Width > image.Height)
        {
            bytes = RotateImage(bytes, 90);
        }

        //TODO Check other cases
        //if (orientation == SimpleOrientation.Rotated90DegreesCounterclockwise)
        //{
        //    bytes = RotateImage(bytes, 90);
        //}

        //if (orientation == SimpleOrientation.Rotated180DegreesCounterclockwise)
        //{
        //    bytes = RotateImage(bytes, 180);
        //}

        //if (orientation == SimpleOrientation.Rotated270DegreesCounterclockwise)
        //{
        //    bytes = RotateImage(bytes, 270);
        //}

        var newSize = GetSized(bytes, 500);
        return ScaleImage(bytes, newSize.Width, newSize.Height);
    }

    private Size GetSized(byte[] bytes, int maxSide)
    {
        using SKBitmap bitmap = SKBitmap.Decode(bytes);
        if (bitmap.Height > bitmap.Width)
        {
            return new Size(bitmap.Width * maxSide / bitmap.Height, maxSide);
        }
        else
        {
            if (bitmap.Height < bitmap.Width)
            {
                return new Size(maxSide, bitmap.Height * maxSide / bitmap.Width);
            }

            return new Size(maxSide, maxSide);
        }
    }

    private byte[] RotateImage(byte[] bytes, int degrees)
    {
        using SKBitmap bitmap = SKBitmap.Decode(bytes);
        using var rotated = new SKBitmap(bitmap.Height, bitmap.Width);
        using (var surface = new SKCanvas(rotated))
        {
            surface.Translate(rotated.Width, 0);
            surface.RotateDegrees(degrees);
            surface.DrawBitmap(bitmap, 0, 0);
        }

        return rotated.Encode(SKEncodedImageFormat.Jpeg, 80).ToArray();
    }

    private byte[] ScaleImage(byte[] bytes, int maxWidth, int maxHeight)
    {
        using SKBitmap bitmap = SKBitmap.Decode(bytes);
        var ratioX = (double)maxWidth / bitmap.Width;
        var ratioY = (double)maxHeight / bitmap.Height;
        var ratio = Math.Min(ratioX, ratioY);
        var newWidth = (int)(bitmap.Width * ratio);
        var newHeight = (int)(bitmap.Height * ratio);
        var info = new SKImageInfo(newWidth, newHeight);
        using var resized = bitmap.Resize(info, SKFilterQuality.High);
        return resized.Encode(SKEncodedImageFormat.Jpeg, 80).ToArray();
    }

    public static PhotoHandler Instance => instance ??= new PhotoHandler();

    private static PhotoHandler instance;

    private readonly CameraCaptureUI capturer = new CameraCaptureUI();
}