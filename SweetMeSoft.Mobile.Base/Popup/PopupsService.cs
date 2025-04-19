using CommunityToolkit.Maui.Views;

namespace SweetMeSoft.Mobile.Base.Popup;

public class PopupsService
{
    private static PopupsService instance;

    public static PopupsService Instance
    {
        get { instance ??= new PopupsService(); return instance; }
    }

    private LoadingPopup _currentLoadingPopup;

    public void ShowLoading(string message = "Espera por favor...")
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_currentLoadingPopup is not null)
            {
                // Opcional: podrías actualizar el mensaje aquí si lo deseas
                // _currentLoadingPopup.UpdateMessage(message);
                return;
            }

            _currentLoadingPopup = new LoadingPopup(message);

            var currentPage = Application.Current?.MainPage;
            if (currentPage != null)
            {
                currentPage.ShowPopup(_currentLoadingPopup);
            }
            else
            {
                Console.WriteLine("[LoadingService] Error: MainPage no disponible para mostrar Popup.");
            }
        });
    }

    public void HideLoading()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _currentLoadingPopup?.Close();
            _currentLoadingPopup = null;
        });
    }
}