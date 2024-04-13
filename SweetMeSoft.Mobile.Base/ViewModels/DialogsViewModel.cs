using CommunityToolkit.Mvvm.ComponentModel;

using Controls.UserDialogs.Maui;

namespace SweetMeSoft.Mobile.Base.ViewModels;

public class DialogsViewModel : ObservableObject
{
    public async Task DisplayAlert(string title, string message, string cancel)
    {
        await UserDialogs.Instance.AlertAsync(message, title, cancel);
    }

    public async Task<bool> DisplayAlert(string title, string message, string okText, string cancelText)
    {
        return await UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText);
        //var tcs = new TaskCompletionSource<bool>();
        //Application.Current?.Dispatcher.Dispatch(async () =>
        //{
        //    try
        //    {
        //        var t = await UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText);
        //        tcs.SetResult(t);
        //    }
        //    catch (Exception e)
        //    {
        //        tcs.SetException(e);
        //    }
        //});

        //return await tcs.Task;
    }

    public async Task<string> DisplayPrompt(string title, string message, string accept = "Ok", string cancel = "Cancel", Keyboard keyboard = null, string initial = "")
    {
        var tcs = new TaskCompletionSource<string>();
        Application.Current?.Dispatcher.Dispatch(async () =>
        {
            try
            {
                var t = await Application.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, "", 50, keyboard, initial);
                tcs.SetResult(t);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });

        return await tcs.Task;
    }
}