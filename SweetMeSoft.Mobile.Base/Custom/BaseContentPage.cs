namespace SweetMeSoft.Mobile.Base.Custom;

public class BaseContentPage<T> : ContentPage
{
    public event Action<T> PageDisappearing;
    public T result;

    public BaseContentPage()
    {
    }

    protected override void OnDisappearing()
    {
        PageDisappearing?.Invoke(result);
        if (PageDisappearing != null)
        {
            foreach (var @delegate in PageDisappearing.GetInvocationList())
            {
                PageDisappearing -= @delegate as Action<T>;
            }
        }
        base.OnDisappearing();
    }
}
