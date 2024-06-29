using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using SweetMeSoft.Uno.Base.i18n.Resources;

namespace SweetMeSoft.Uno.Base.ViewModels;

public class DialogsViewModel() : ObservableObject
{
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

    public async Task<bool> DisplayAlert(string title, string message, string okText = "", string cancelText = "")
    {
        okText = string.IsNullOrEmpty(okText) ? Resources.Ok : okText;
        cancelText = string.IsNullOrEmpty(cancelText) ? Resources.Cancel : cancelText;
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

    public async Task<string> DisplayList(string title, string message, string cancel, params string[] items)
    {
        var dialog = new ContentDialog()
        {
            Title = title,
            Content = new ComboBox() { PlaceholderText = message, ItemsSource = items },
            PrimaryButtonText = Resources.Ok,
            CloseButtonText = cancel,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = Window.Current.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        var combobox = dialog.Content as ComboBox;
        return result == ContentDialogResult.Primary && combobox.SelectedItem != null ? combobox.SelectedItem.ToString() : string.Empty;
    }

    public async void DisplayLoading(string message)
    {
        loadingCount++;
        if ((loading == null || !loading.IsLoaded) && loadingCount == 1)
        {
            loading.Title = message;
            await loading.ShowAsync();
        }
    }

    public async Task<string> DisplayPrompt(string title, string message, string accept = "", string cancel = "", string initial = "")
    {
        accept = string.IsNullOrEmpty(accept) ? Resources.Ok : accept;
        cancel = string.IsNullOrEmpty(cancel) ? Resources.Cancel : cancel;
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

    public void HideLoading()
    {
        loadingCount--;
        if (loading.IsLoaded && loadingCount == 0)
        {
            loading.Hide();
        }
    }

    private readonly ContentDialog loading = new ContentDialog()
    {
        Content = new ProgressRing() { IsActive = true },
        AllowFocusOnInteraction = false,
        XamlRoot = Window.Current.Content.XamlRoot
    };

    private int loadingCount = 0;
}