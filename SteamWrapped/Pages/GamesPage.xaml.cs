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
    private async void FilterButtonTapped(object sender, TappedEventArgs e)
    {
        if (sender is TapGestureRecognizer tap &&
            tap.Parent is Border border)
        {
            await border.ScaleTo(0.92, 80, Easing.CubicOut);
            await border.ScaleTo(1.0, 120, Easing.CubicIn);
        }
    }

}