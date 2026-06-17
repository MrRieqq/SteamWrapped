using SteamWrapped.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SteamWrapped.Services;

public class WrappedService
{
    public List<Game> GetGames()
    {
        return new()
        {
            new Game
            {
                Name = "Dota 2",
                Genre = "MOBA",
                HoursPlayed = 542,
                Achievements = 120,
                Sessions = 250,
                YearPlayed = 2025
            },

            new Game
            {
                Name = "Cyberpunk 2077",
                Genre = "RPG",
                HoursPlayed = 180,
                Achievements = 55,
                Sessions = 60,
                YearPlayed = 2025
            },

            new Game
            {
                Name = "Civilization VI",
                Genre = "Strategy",
                HoursPlayed = 120,
                Achievements = 35,
                Sessions = 40,
                YearPlayed = 2025
            },

            new Game
            {
                Name = "Counter Strike 2",
                Genre = "Shooter",
                HoursPlayed = 210,
                Achievements = 70,
                Sessions = 100,
                YearPlayed = 2025
            }
        };
    }

    public async Task<WrappedReport> GenerateReport(string steamId)
    {
        var games = await GetSteamGames(steamId);

        var totalHours = games.Sum(g => g.HoursPlayed);

        var favoriteGame = games
            .OrderByDescending(g => g.HoursPlayed)
            .First()
            .Name;

        var favoriteGenre = games
            .GroupBy(g => g.Genre)
            .OrderByDescending(g => g.Sum(x => x.HoursPlayed))
            .First()
            .Key;

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

        var steamData =
            JsonSerializer.Deserialize<SteamOwnedGamesResponse>(json);

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
}