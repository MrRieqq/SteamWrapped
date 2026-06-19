using CommunityToolkit.Mvvm.ComponentModel;
using SteamWrapped.Models;
using SteamWrapped.Services;
using System.Collections.ObjectModel;
using static SteamWrapped.Services.SteamApiService;

public partial class AllSessionsViewModel : ObservableObject
{
    private readonly WrappedService _service = new();

    [ObservableProperty]
    private ObservableCollection<RecentGame> sessions = [];

    public async Task Load()
    {
        Sessions.Clear();

        var items =
            await _service.GetRecentGames(
                AppData.SteamId);

        foreach (var item in items)
        {
            Sessions.Add(item);
        }
    }
}