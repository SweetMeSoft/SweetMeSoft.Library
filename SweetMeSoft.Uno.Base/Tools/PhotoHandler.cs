using SkiaSharp;

using System.Drawing;

using Windows.Devices.Sensors;
using Windows.Media.Capture;

namespace SweetMeSoft.Uno.Base.Tools;

public class PhotoHandler
{
    //TODO Storage the new photo in a temp folder
    public static async Task<PhotoResult> TakePhoto()
    {
        try
        {
            var captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            //captureUI.PhotoSettings.CroppedSizeInPixels = new Size(400, 400);
            //captureUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.SmallVga;
            //captureUI.PhotoSettings.AllowCropping = true;
            //captureUI.PhotoSettings.CroppedAspectRatio = new Size(1, 1);

            var photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (photo == null)
            {
                return null;
            }
            else
            {
                var orientation = SimpleOrientationSensor.GetDefault()?.GetCurrentOrientation() ?? SimpleOrientation.NotRotated;
                return new PhotoResult
                {
                    OriginalPath = photo.Path,
                    EditedBytes = GetFixedPhoto(photo.Path, orientation),
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

    public static async Task<List<PhotoResult>> TakePhotos()
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
        }
        while (photo != null);
        return results;
    }

    private static byte[] GetFixedPhoto(string path, SimpleOrientation orientation)
    {
        using var sourceStream = File.OpenRead(path);
        using var memoryStream = new MemoryStream();
        sourceStream.CopyTo(memoryStream);
        var bytes = memoryStream.ToArray();

        SKBitmap image = SKBitmap.Decode(bytes);
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

    private static Size GetSized(byte[] bytes, int maxSide)
    {
        SKBitmap originalImage = SKBitmap.Decode(bytes);
        if (originalImage.Height > originalImage.Width)
        {
            return new Size(originalImage.Width * maxSide / originalImage.Height, maxSide);
        }
        else
        {
            if (originalImage.Height < originalImage.Width)
            {
                return new Size(maxSide, originalImage.Height * maxSide / originalImage.Width);
            }

            return new Size(maxSide, maxSide);
        }
    }

    private static byte[] RotateImage(byte[] bytes, int degrees)
    {
        using var bitmap = SKBitmap.Decode(bytes);
        var rotated = new SKBitmap(bitmap.Height, bitmap.Width);

        using (var surface = new SKCanvas(rotated))
        {
            surface.Translate(rotated.Width, 0);
            surface.RotateDegrees(degrees);
            surface.DrawBitmap(bitmap, 0, 0);
        }

        return rotated.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray();
    }

    private static byte[] ScaleImage(byte[] bytes, int maxWidth, int maxHeight)
    {
        SKBitmap image = SKBitmap.Decode(bytes);

        var ratioX = (double)maxWidth / image.Width;
        var ratioY = (double)maxHeight / image.Height;
        var ratio = Math.Min(ratioX, ratioY);

        var newWidth = (int)(image.Width * ratio);
        var newHeight = (int)(image.Height * ratio);

        var info = new SKImageInfo(newWidth, newHeight);
        image = image.Resize(info, SKFilterQuality.High);
        return image.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray();
    }
}