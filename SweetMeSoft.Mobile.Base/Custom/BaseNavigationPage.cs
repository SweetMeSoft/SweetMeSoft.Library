namespace SweetMeSoft.Mobile.Base.Custom;

public class BaseNavigationPage<T> : NavigationPage
{
    public BaseContentPage<T> MyContentPage;
    public BaseNavigationPage(BaseContentPage<T> contentPage) : base(contentPage)
    {
        //BarBackgroundColor = Color.FromHex("#011155");
        //BarTextColor = Microsoft.Maui.Graphics.Color.FromHex("#FFFFFF");
        MyContentPage = contentPage;
    }
}

