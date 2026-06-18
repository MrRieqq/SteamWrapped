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

            foreach (var game in steamGames)
            {
                Console.WriteLine($"NAME=[{game.Name}]");
                Console.WriteLine($"GENRE=[{game.Genre}]");
            }

            Games.Clear();
        }
        finally
        {
            IsLoading = false;
        }

    }
}