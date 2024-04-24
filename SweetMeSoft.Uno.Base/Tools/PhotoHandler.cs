using Windows.Foundation;
using Windows.Media.Capture;

namespace SweetMeSoft.Uno.Base.Tools;

public class PhotoHandler
{
    public static async Task<PhotoResult> TakePhoto()
    {
        try
        {
            var captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(400, 400);
            captureUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.SmallVga;
            captureUI.PhotoSettings.AllowCropping = true;
            captureUI.PhotoSettings.CroppedAspectRatio = new Size(1, 1);

            var photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                return null;
            }
            else
            {
                return new PhotoResult
                {
                    Path = photo.Path
                };
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
        //TODO Fix
        return null;
        //if (MediaPicker.Default.IsCaptureSupported)
        //{
        //    try
        //    {
        //        UserDialogs.Instance.ShowLoading("Tomando foto...");
        //        var result = new PhotoResult();
        //        FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
        //        await Task.Delay(1000); //Delay is needed to wait back to the previous page
        //        if (photo != null)
        //        {
        //            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        //            {
        //                result.Path = photo.FullPath;
        //            }
        //            else
        //            {
        //                var fullPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
        //                using var stream = await photo.OpenReadAsync();
        //                using var newStream = File.Create(fullPath);
        //                await stream.CopyToAsync(newStream);
        //                result.Path = fullPath;
        //            }
        //            UserDialogs.Instance.HideHud();
        //            return result;
        //        }

        //        UserDialogs.Instance.HideHud();
        //        return null;
        //    }
        //    catch (Exception e)
        //    {
        //        UserDialogs.Instance.HideHud();
        //        if (e.GetType() == typeof(PermissionException))
        //        {
        //            await DisplayAlert("No se tienen los permisos necesarios para tomar fotos. Habilita los permisos en los ajustes de la app para poder continuar.", "Error", "Aceptar");
        //        }
        //        else
        //        {
        //            await DisplayAlert("Error al tomar la foto", "Error", "Aceptar");
        //        }
        //        return null;
        //    }
        //}
        //else
        //{
        //    await DisplayAlert("No se puede tomar fotos en este dispositivo", "Error", "Aceptar");
        //    return null;
        //}
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
}
