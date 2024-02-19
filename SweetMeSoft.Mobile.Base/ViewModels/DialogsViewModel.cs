using Controls.UserDialogs.Maui;

using MvvmHelpers;

namespace SweetMeSoft.Mobile.Base.ViewModels;

public class DialogsViewModel : BaseViewModel
{

    public void DisplayAlert(string title, string message, string cancel)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await UserDialogs.Instance.AlertAsync(message, title, cancel);
        });
    }

    public async Task<bool> DisplayAlert(string title, string message, string okText, string cancelText)
    {
        return await UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText);
    }

    public async Task<string> DisplayPrompt(string title, string message, string accept = "Ok", string cancel = "Cancel", Keyboard keyboard = null, string initial = "")
    {
        return await Application.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, "", 50, keyboard, initial);
    }
}
