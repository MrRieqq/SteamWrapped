
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamWrapped.Models;
using SteamWrapped.Services;

namespace SteamWrapped.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string steamId = string.Empty;

    [ObservableProperty]
    private int totalHours;

    [ObservableProperty]
    private string favoriteGame = string.Empty;

    [ObservableProperty]
    private string favoriteGenre = string.Empty;

    [ObservableProperty]
    private string playerType = string.Empty;

    [ObservableProperty]
    private string gamerRank = string.Empty;

    [ObservableProperty]
    private int gamesPlayed;

    [ObservableProperty]
    private double averageHoursPerGame;

    [ObservableProperty]
    private int totalAchievements;

    [ObservableProperty]
    private string topGame1 = string.Empty;

    [ObservableProperty]
    private string topGame2 = string.Empty;

    [ObservableProperty]
    private string topGame3 = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [RelayCommand]
    private async Task LoadSteamData()
    {
        if (string.IsNullOrWhiteSpace(SteamId))
        {
            ErrorMessage = "Введите SteamID64";
            HasError = true;
            return;
        }

        try
        {
            HasError = false;
            ErrorMessage = "";
            IsLoading = true;

            var service = new WrappedService();

            var report = await service.GenerateReport(SteamId);

            AppData.SteamId = SteamId;
            TotalHours = report.TotalHours;
            FavoriteGame = report.FavoriteGame;
            FavoriteGenre = report.FavoriteGenre;
            PlayerType = report.PlayerType;
            GamerRank = report.GamerRank;

            GamesPlayed = report.GamesPlayed;
            AverageHoursPerGame = report.AverageHoursPerGame;
            TotalAchievements = report.TotalAchievements;

            TopGame1 = report.TopGame1;
            TopGame2 = report.TopGame2;
            TopGame3 = report.TopGame3;
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;

            await Shell.Current.DisplayAlert(
                "Ошибка",
                ex.Message,
                "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }
}