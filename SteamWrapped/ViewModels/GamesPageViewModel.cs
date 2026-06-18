using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamWrapped.Models;
using SteamWrapped.Services;
using System.Collections.ObjectModel;

namespace SteamWrapped.ViewModels;

public partial class GamesPageViewModel : ObservableObject
{
    private readonly WrappedService _service = new();

    [ObservableProperty]
    private ObservableCollection<Game> games = new();

    [ObservableProperty]
    private bool isLoading;

    public async Task LoadGames(string steamId)
    {
        try
        {
            IsLoading = true;

            var steamGames =
                await _service.GetSteamGames(steamId);

            Games.Clear();

            foreach (var game in steamGames)
            {
                Games.Add(game);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}