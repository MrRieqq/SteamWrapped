using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamWrapped.Services;

namespace SteamWrapped.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string steamId;

    [ObservableProperty]
    private int totalHours;

    [ObservableProperty]
    private string favoriteGame;

    [ObservableProperty]
    private string favoriteGenre;

    [ObservableProperty]
    private string playerType;

    [ObservableProperty]
    private int gamesPlayed;

    [ObservableProperty]
    private double averageHoursPerGame;

    [ObservableProperty]
    private int totalAchievements;

    [ObservableProperty]
    private string topGame1;

    [ObservableProperty]
    private string topGame2;

    [ObservableProperty]
    private string topGame3;

    public MainPageViewModel()
    {
        SteamId = string.Empty;
    }

    [RelayCommand]
    private async Task LoadSteamData()
    {
        if (string.IsNullOrWhiteSpace(SteamId))
            return;

        try
        {
            var service = new WrappedService();

            var report = await service.GenerateReport(SteamId);

            TotalHours = report.TotalHours;
            FavoriteGame = report.FavoriteGame;
            FavoriteGenre = report.FavoriteGenre;
            PlayerType = report.PlayerType;

            GamesPlayed = report.GamesPlayed;
            AverageHoursPerGame = report.AverageHoursPerGame;
            TotalAchievements = report.TotalAchievements;

            TopGame1 = report.TopGame1;
            TopGame2 = report.TopGame2;
            TopGame3 = report.TopGame3;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Ошибка",
                ex.Message,
                "OK");
        }
    }

}