using SteamWrapped.ViewModels;

namespace SteamWrapped.Pages;

public partial class AlAchievementsPage : ContentPage
{
    private readonly AllAchievementsViewModel _vm =
        new();

    public AlAchievementsPage()
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