using SweetMeSoft.Base;
using SweetMeSoft.Uno.Base.i18n.Resources;
using SweetMeSoft.Uno.Base.ViewModels;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace SweetMeSoft.Uno.Base.Tools;

public class FileHandler
{
    public async Task<StreamFile> PickFile(int maxFileSize = int.MaxValue)
    {
        var fileOpenPicker = new FileOpenPicker();
        fileOpenPicker.FileTypeFilter.Add("*");
        fileOpenPicker.ViewMode = PickerViewMode.List;
        var file = await fileOpenPicker.PickSingleFileAsync();
        if (file == null)
        {
            return null;
        }

        using var stream = await file.OpenAsync(FileAccessMode.Read);
        var bytesFile = await stream.ReadBytesAsync(new CancellationToken());

        if (bytesFile.Length / 1024 > maxFileSize)
        {
            await dialogsViewModel.DisplayAlert(Resources.Error, Resources.FileTooBig.Replace("{fileSize}", maxFileSize + "KB"), Resources.Ok);
            return null;
        }

        var contentType = Constants.GetContentType(file.FileType.Replace(".", ""));
        return new StreamFile(file.DisplayName, new MemoryStream(bytesFile), contentType);
    }

    public async Task<List<StreamFile>> PickFiles(int maxFileSize = int.MaxValue)
    {
        var fileList = new List<StreamFile>();
        var ignoredFiles = false;
        var fileOpenPicker = new FileOpenPicker();
        fileOpenPicker.FileTypeFilter.Add("*");
        fileOpenPicker.ViewMode = PickerViewMode.List;
        var files = await fileOpenPicker.PickMultipleFilesAsync();
        if (files == null)
        {
            return null;
        }

        foreach (var file in files)
        {
            using var stream = await file.OpenAsync(FileAccessMode.Read);
            var bytesFile = await stream.ReadBytesAsync(new CancellationToken());

            if (bytesFile.Length / 1024 > maxFileSize)
            {
                ignoredFiles = true;
            }
            else
            {
                var contentType = Constants.GetContentType(file.FileType.Replace(".", ""));
                fileList.Add(new StreamFile(file.DisplayName, new MemoryStream(bytesFile), contentType));
            }
        }

        if (ignoredFiles)
        {
            await dialogsViewModel.DisplayAlert(Resources.Error, Resources.FileTooBig.Replace("{fileSize}", maxFileSize + "KB"), Resources.Ok);
        }

        return fileList;
    }

    public static FileHandler Instance => instance ??= new FileHandler();

    private static FileHandler instance;

    private readonly DialogsViewModel dialogsViewModel = new DialogsViewModel();
}