using Microsoft.UI.Xaml.Controls;

using SweetMeSoft.Uno.Base.ViewModels;

namespace SweetMeSoft.Uno.Base.Pages;

public class BasePage : Page
{
    private readonly AppBaseViewModel viewModel;

    public BasePage(AppBaseViewModel viewModel)
    {
        this.viewModel = viewModel;
        this.DataContext = viewModel;
        //viewModel.UpdateView = Bindings.Update;
    }
}