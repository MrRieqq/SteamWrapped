using System.Text.Json;
using SteamWrapped.Models;

namespace SteamWrapped.Services;

public class WrappedService
{
    private readonly SteamApiService _api = new();

    public async Task<WrappedReport> GenerateReport(string steamId)
    {
        var games = await GetSteamGames(steamId);

        if (!games.Any())
        {
            return new WrappedReport
            {
                FavoriteGame = "Нет данных",
                FavoriteGenre = "Нет данных",
                PlayerType = "Нет данных",
                GamerRank = "Нет данных"
            };
        }

        var totalHours = games.Sum(g => g.HoursPlayed);

        var favoriteGame = games
            .OrderByDescending(g => g.HoursPlayed)
            .FirstOrDefault()?.Name ?? "Нет данных";

        var favoriteGenre = games
            .GroupBy(g => g.Genre)
            .OrderByDescending(g => g.Sum(x => x.HoursPlayed))
            .FirstOrDefault()?.Key ?? "Unknown";

        var averageHours = games.Average(g => g.HoursPlayed);

        var gamesCount = games.Count;

        var totalAchievements = games.Sum(g => g.Achievements);

        var topGames = games
            .OrderByDescending(g => g.HoursPlayed)
            .Take(3)
            .ToList();

        var playerType = GetPlayerRank(totalHours);

        return new WrappedReport
        {
            TotalHours = totalHours,
            FavoriteGame = favoriteGame,
            FavoriteGenre = favoriteGenre,
            PlayerType = playerType,
            GamesPlayed = gamesCount,
            AverageHoursPerGame = Math.Round(averageHours, 1),
            TotalAchievements = totalAchievements,
            TopGame1 = topGames.Count > 0 ? topGames[0].Name : "",
            TopGame2 = topGames.Count > 1 ? topGames[1].Name : "",
            TopGame3 = topGames.Count > 2 ? topGames[2].Name : "",
            MostPlayedGenre = favoriteGenre,
            GamerRank = playerType
        };
    }

    private string GetPlayerRank(int totalHours)
    {
        if (totalHours >= 1000)
            return "Легенда Steam";

        if (totalHours >= 500)
            return "Хардкорный игрок";

        if (totalHours >= 250)
            return "Активный игрок";

        return "Казуальный игрок";
    }

    public async Task<List<Game>> GetSteamGames(string steamId)
    {
        var api = new SteamApiService();

        var json = await api.GetOwnedGamesJson(steamId);

        Console.WriteLine(json);

        var steamData =
            JsonSerializer.Deserialize<SteamOwnedGamesResponse>(json);

        if (steamData == null)
            return new List<Game>();

        if (steamData.response == null)
            return new List<Game>();

        if (steamData.response.games == null)
            return new List<Game>();

        return steamData.response.games
            .Select(g => new Game
            {
                Name = g.name,
                HoursPlayed = g.playtime_forever / 60,
                Genre = "Unknown",
                Achievements = 0
            })
            .ToList();
    }

    private async Task<string> GetGenre(int appId)
    {
        try
        {
            var json = await _api.GetGameDetails(appId);

            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            var property =
                root.EnumerateObject().FirstOrDefault();

            if (property.Value.TryGetProperty(
                "data",
                out var data))
            {
                if (data.TryGetProperty(
                    "genres",
                    out var genres))
                {
                    var firstGenre =
                        genres.EnumerateArray()
                        .FirstOrDefault();

                    if (firstGenre.TryGetProperty(
                        "description",
                        out var description))
                    {
                        return description.GetString()
                               ?? "Unknown";
                    }
                }
            }
        }
        catch
        {
        }

        return "Unknown";
    }
}