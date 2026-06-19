using SteamWrapped.ViewModels;

namespace SteamWrapped.Pages;

public partial class Statspage : ContentPage
{
    private readonly StatsPageViewModel _vm;

    public Statspage()
    {
        InitializeComponent();

        _vm = new StatsPageViewModel();

        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _vm.Load();
    }
}