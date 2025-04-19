namespace SweetMeSoft.Mobile.Base.Popup;

public partial class LoadingPopup : CommunityToolkit.Maui.Views.Popup
{
    public LoadingPopup(string message = "Cargando...")
    {
        InitializeComponent();
        LoadingLabel.Text = message;

        // Opcional: Establece el tamaño del Popup
        // Si no lo estableces, se ajustará al contenido (Frame)
        // Size = new Size(250, 200);
    }

    public void UpdateMessage(string newMessage)
    {
        if (!string.IsNullOrWhiteSpace(newMessage))
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingLabel.Text = newMessage;
            });
        }
    }
}