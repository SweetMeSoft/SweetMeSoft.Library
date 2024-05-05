using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SweetMeSoft.Uno.Base.ViewModels;

public class NavigationViewModel : DialogsViewModel
{
    public NavigationViewModel()
    {
    }

    public void BackToRoot()
    {
        var frame = (Frame)Window.Current.Content;
        while (frame.CanGoBack)
        {
            frame.BackStack.Clear();
        }
    }

    public void GoBack()
    {
        var frame = (Frame)Window.Current.Content;
        if (frame.CanGoBack)
        {
            frame.GoBack();
        }
    }

    public void GoTo<TClass>(object parameter, bool removePrevious = false) where TClass : Page
    {
        var frame = (Frame)Window.Current.Content;
        frame.Navigate(typeof(TClass), parameter);
        if (removePrevious)
        {
            int count = frame.BackStack.Count;
            if (count >= 1)
            {
                frame.BackStack.RemoveAt(frame.BackStack.Count - 1);
            }
        }
    }

    public void GoTo<TClass>(bool removePrevious = false) where TClass : Page, new()
    {
        GoTo<TClass>(null, removePrevious);
    }

    public void GoToNewRoot<TClass>() where TClass : Page, new()
    {
        var frame = (Frame)Window.Current.Content;
        frame.Navigate(typeof(TClass));
        frame.BackStack.Clear();
    }

    //public async Task<T> OpenModal<TClass, T>() where TClass : BaseContentPage<T>, new()
    //{
    //    return await OpenModal<TClass, T>(new TClass());
    //}

    //public async Task<T> OpenModal<TClass, T>(TClass newPage) where TClass : BaseContentPage<T>
    //{
    //    var source = new TaskCompletionSource<T>();
    //    newPage.PageDisappearing += (result) =>
    //    {
    //        var res = (T)Convert.ChangeType(result, typeof(T));
    //        source.SetResult(res);
    //    };

    //    await Application.Current.MainPage.Navigation.PushModalAsync(new BaseNavigationPage<T>(newPage), true);
    //    return await source.Task;
    //}

    //public void CloseModal<T>(T result)
    //{
    //    MainThread.BeginInvokeOnMainThread(async () =>
    //    {
    //        if (Application.Current.MainPage.Navigation.ModalStack.FirstOrDefault() is BaseNavigationPage<T> newPage)
    //        {
    //            newPage.MyContentPage.result = result;
    //            await Application.Current.MainPage.Navigation.PopModalAsync(true);
    //        }
    //        else
    //        {
    //            DisplayAlert("Error", "Modal closing has an error", "Ok");
    //        }
    //    });
    //}
}