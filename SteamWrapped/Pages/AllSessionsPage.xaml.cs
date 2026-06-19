using SteamWrapped.ViewModels;

namespace SteamWrapped.Pages;

public partial class AllSessionsPage : ContentPage
{
    private readonly AllSessionsViewModel _vm =
        new();

    public AllSessionsPage()
    {
        InitializeComponent();

        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _vm.Load();
    }
}