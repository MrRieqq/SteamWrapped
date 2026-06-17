using SteamWrapped.Models;
using SteamWrapped.ViewModels;

namespace SteamWrapped.Pages;

public partial class GamesPage : ContentPage
{
    private readonly GamesPageViewModel _vm;

    public GamesPage()
    {
        InitializeComponent();

        _vm = new GamesPageViewModel();

        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrWhiteSpace(AppData.SteamId))
        {
            await _vm.LoadGames(AppData.SteamId);
        }
    }
}