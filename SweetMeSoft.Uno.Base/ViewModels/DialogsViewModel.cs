using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SweetMeSoft.Uno.Base.ViewModels;

public class DialogsViewModel() : ObservableObject
{
    private ContentDialog loading;

    public async Task DisplayAlert(string title, string message, string okText)
    {
        try
        {
            var dialog = new ContentDialog()
            {
                Title = title,
                Content = message,
                CloseButtonText = okText,
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = Window.Current.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public async Task<bool> DisplayAlert(string title, string message, string okText, string cancelText)
    {
        var dialog = new ContentDialog()
        {
            Title = title,
            Content = message,
            PrimaryButtonText = okText,
            CloseButtonText = cancelText,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = Window.Current.Content.XamlRoot
        };
        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    public async void DisplayLoading(string message)
    {
        if (loading == null || !loading.IsLoaded)
        {
            loading = new ContentDialog()
            {
                Content = new ProgressRing() { IsActive = true },
                Title = message,
                AllowFocusOnInteraction = false,
                XamlRoot = Window.Current.Content.XamlRoot
            };

            await loading.ShowAsync();
        }
    }

    public void HideLoading()
    {
        if (loading.IsLoaded)
        {
            loading.Hide();
        }
    }

    public async Task<string> DisplayPrompt(string title, string message, string accept = "Ok", string cancel = "Cancel", string initial = "")
    {
        var dialog = new ContentDialog()
        {
            Title = title,
            Content = new TextBox() { PlaceholderText = message, Text = initial },
            PrimaryButtonText = accept,
            CloseButtonText = cancel,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = Window.Current.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary ? (dialog.Content as TextBox).Text : string.Empty;
    }

    public async Task<string> DisplayList(string title, string message, string cancel, params string[] items)
    {
        //TODO Add Ok text
        var dialog = new ContentDialog()
        {
            Title = title,
            Content = new ComboBox() { PlaceholderText = message, ItemsSource = items },
            PrimaryButtonText = "Ok",
            CloseButtonText = cancel,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = Window.Current.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary ? (dialog.Content as ComboBox).SelectedItem.ToString() : string.Empty;
    }

    //public async Task<string> DisplayPrompt(string title, string message, string accept = "Ok", string cancel = "Cancel", Keyboard keyboard = null, string initial = "")
    //{
    //    var tcs = new TaskCompletionSource<string>();
    //    Application.Current?.Dispatcher.Dispatch(async () =>
    //    {
    //        try
    //        {
    //            var t = await Application.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, "", 50, keyboard, initial);
    //            tcs.SetResult(t);
    //        }
    //        catch (Exception e)
    //        {
    //            tcs.SetException(e);
    //        }
    //    });

    //    return await tcs.Task;
    //}
}
