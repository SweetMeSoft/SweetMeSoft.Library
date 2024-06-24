using SweetMeSoft.Mobile.Base.Custom;

namespace SweetMeSoft.Mobile.Base.ViewModels;

public class NavigationViewModel : DialogsViewModel
{
    public void BackToRoot()
    {
        if (isNavigating)
        {
            return;
        }

        isNavigating = true;
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync();
            isNavigating = false;
        });
    }

    public void CloseModal<T>(T result)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (Application.Current.MainPage.Navigation.ModalStack.FirstOrDefault() is BaseNavigationPage<T> newPage)
            {
                newPage.MyContentPage.result = result;
                await Application.Current.MainPage.Navigation.PopModalAsync(true);
            }
            else
            {
                await DisplayAlert("Error", "Modal closing has an error", "Ok");
            }
        });
    }

    public void GoBack()
    {
        if (isNavigating)
        {
            return;
        }

        isNavigating = true;
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Application.Current.MainPage.Navigation.PopAsync();
            isNavigating = false;
        });
    }

    public void GoTo<TClass>(TClass newPage, bool removePrevious = false) where TClass : Page
    {
        if (isNavigating)
        {
            return;
        }

        isNavigating = true;
        //TODO Check if TClass is a Modal
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Application.Current.MainPage.Navigation.PushAsync(newPage, true);
            if (removePrevious)
            {
                var count = Application.Current.MainPage.Navigation.NavigationStack.Count;
                if (count >= 1)
                {
                    Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[count - 2]);
                }
            }

            isNavigating = false;
        });
    }

    public void GoTo<TClass>(bool removePrevious = false) where TClass : Page, new()
    {
        GoTo(new TClass(), removePrevious);
    }

    public void GoToNewRoot<TClass>(TClass newPage) where TClass : Page, new()
    {
        if (isNavigating)
        {
            return;
        }

        isNavigating = true;
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            Application.Current.MainPage.Navigation.InsertPageBefore(newPage, Application.Current.MainPage.Navigation.NavigationStack[0]);
            await Application.Current.MainPage.Navigation.PopToRootAsync();
            isNavigating = false;
        });
    }

    public void GoToNewRoot<TClass>() where TClass : Page, new()
    {
        GoToNewRoot(new TClass());
    }

    public Task<T> OpenModal<TClass, T>() where TClass : BaseContentPage<T>, new()
    {
        return OpenModal<TClass, T>(new TClass());
    }

    public async Task<T> OpenModal<TClass, T>(TClass newPage) where TClass : BaseContentPage<T>
    {
        var source = new TaskCompletionSource<T>();
        newPage.PageDisappearing += (result) =>
        {
            var res = (T)Convert.ChangeType(result, typeof(T));
            source.SetResult(res);
        };

        await Application.Current.MainPage.Navigation.PushModalAsync(new BaseNavigationPage<T>(newPage), true);
        return await source.Task;
    }

    private bool isNavigating;
}