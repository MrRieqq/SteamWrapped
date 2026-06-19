using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamWrapped.Models;
using SteamWrapped.Pages;
using SteamWrapped.Services;
using System.Collections.ObjectModel;
using static SteamWrapped.Models.Game;
using static SteamWrapped.Services.SteamApiService;
using System.Linq;

namespace SteamWrapped.ViewModels;

public partial class StatsPageViewModel : ObservableObject
{
    [RelayCommand]
    private Task OpenAllGames()
    {
        Shell.Current.CurrentItem =
            Shell.Current.Items[1];

        return Task.CompletedTask;
    }
    [RelayCommand]
    private async Task OpenAllAchievements()
    {
        await Shell.Current.GoToAsync(
            nameof(AlAchievementsPage));
    }
    private readonly WrappedService _service = new();
    [ObservableProperty]
    private ObservableCollection<RecentGame> recentGames = [];
    [ObservableProperty]
    private ObservableCollection<Game> topGames = [];

    [ObservableProperty]
    private int totalHours;

    [ObservableProperty]
    private int totalGames;

    [ObservableProperty]
    private int unlockedAchievements;

    [ObservableProperty]
    private int totalAchievements;

    [ObservableProperty]
    private double achievementProgress;

    [ObservableProperty]
    private double averageHours;

    public async Task Load()
    {
        if (string.IsNullOrWhiteSpace(AppData.SteamId))
            return;

        var games =
            await _service.GetSteamGames(AppData.SteamId);

        if (!games.Any())
            return;

        TopGames.Clear();
        RecentGames.Clear();
        RecentAchievements.Clear();

        var achievements =
    await _service.GetRecentAchievements(
        AppData.SteamId);

        foreach (var achievement in achievements.Take(5))
        {
            RecentAchievements.Add(achievement);
        }
        var recent =
            await _service.GetRecentGames(AppData.SteamId);

        int rank1 = 1;

        foreach (var game in recent.Take(5))
        {
            game.Rank = rank1++;
            RecentGames.Add(game);
        }
        var topGames = games
    .OrderByDescending(x => x.HoursPlayed)
    .Take(5)
    .ToList();

        var maxHours = topGames.Max(x => x.HoursPlayed);

        int rank = 1;

        foreach (var game in topGames)
        {
            game.Rank = rank++;

            game.PlaytimeProgress =
                maxHours == 0
                ? 0
                : (double)game.HoursPlayed / maxHours;

            TopGames.Add(game);
        }

        TotalHours = games.Sum(x => x.HoursPlayed);

        TotalGames = games.Count;

        UnlockedAchievements =
            games.Sum(x => x.AchievementsUnlocked);

        TotalAchievements =
            games.Sum(x => x.AchievementsTotal);

        AchievementProgress =
            TotalAchievements == 0
                ? 0
                : (double)UnlockedAchievements /
                  TotalAchievements;

        AverageHours =
            Math.Round(
                games.Average(x => x.HoursPlayed),
                1);
        OnPropertyChanged(nameof(TotalHoursText));
        OnPropertyChanged(nameof(AverageHoursText));
        OnPropertyChanged(nameof(AchievementText));
        OnPropertyChanged(nameof(AchievementPercentText));
    }
    public int Rank { get; set; }

    public double PlaytimeProgress { get; set; }
    public string TotalHoursText =>
    $"{TotalHours:N0} ч.";

    public string AverageHoursText =>
        $"{AverageHours:F1} ч.";

    public string AchievementText =>
        $"{UnlockedAchievements:N0} / {TotalAchievements:N0}";

    public string AchievementPercentText =>
        $"{AchievementProgress * 100:F0}% завершено";
    [ObservableProperty]
    private ObservableCollection<RecentAchievement>
    recentAchievements = [];
}