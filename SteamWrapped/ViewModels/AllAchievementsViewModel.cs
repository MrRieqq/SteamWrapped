using CommunityToolkit.Mvvm.ComponentModel;
using SteamWrapped.Models;
using SteamWrapped.Services;
using System.Collections.ObjectModel;
using static SteamWrapped.Models.Game;

namespace SteamWrapped.ViewModels;

public partial class AllAchievementsViewModel
    : ObservableObject
{
    private readonly WrappedService _service = new();

    [ObservableProperty]
    private ObservableCollection<RecentAchievement>
        achievements = [];

    public async Task Load()
    {
        Achievements.Clear();

        var items =
            await _service.GetRecentAchievements(
                AppData.SteamId);

        foreach (var item in items)
        {
            Achievements.Add(item);
        }
    }
}