using System.Text.Json;
using SteamWrapped.Models;

namespace SteamWrapped.Services;

public class WrappedService
{
    private static readonly Dictionary<int, string> GenreCache = new();
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

        var totalAchievements =
            games.Sum(g => g.AchievementsUnlocked);

        var favoriteGame = games
            .OrderByDescending(g => g.HoursPlayed)
            .FirstOrDefault()?.Name ?? "Нет данных";

        var favoriteGenre = games
            .Where(g => !string.IsNullOrWhiteSpace(g.Genre))
            .GroupBy(g => g.Genre)
            .OrderByDescending(g => g.Sum(x => x.HoursPlayed))
            .FirstOrDefault()?.Key ?? "Unknown";

        var averageHours = games.Average(g => g.HoursPlayed);

        var gamesCount = games.Count;

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
        var json = await _api.GetOwnedGamesJson(steamId);

        if (string.IsNullOrWhiteSpace(json))
            return new List<Game>();

        var steamData =
            JsonSerializer.Deserialize<SteamOwnedGamesResponse>(json);

        if (steamData?.response?.games == null)
            return new List<Game>();

        var topGames = steamData.response.games
            .OrderByDescending(x => x.playtime_forever)
            .Take(50)
            .ToList();

        var tasks = topGames.Select(async g =>
        {
            var genreTask = GetGenre(g.appid);
            var achievementTask = GetAchievements(steamId, g.appid);
            var achievementTotalTask = GetAchievementsTotal(g.appid);

            await Task.WhenAll(
                genreTask,
                achievementTask,
                achievementTotalTask);

            return new Game
            {
                AppId = g.appid,
                Name = g.name,
                HoursPlayed = g.playtime_forever / 60,
                Genre = await genreTask,
                AchievementsUnlocked = await achievementTask,
                AchievementsTotal = await achievementTotalTask
            };
        });

        return (await Task.WhenAll(tasks)).ToList();
    }
    private static readonly Dictionary<int, int> AchievementTotalCache = new();
    private async Task<string> GetGenre(int appId)
    {
        if (GenreCache.TryGetValue(appId, out var cached))
            return cached;
        try
        {
            var json = await _api.GetGameDetails(appId);

            if (string.IsNullOrWhiteSpace(json))
                return "Unknown";

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
                        var genre =
                            description.GetString() ?? "Unknown";

                        GenreCache[appId] = genre;

                        return genre;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"GENRE ERROR {appId}: {ex.Message}");
        }
        GenreCache[appId] = "Unknown";
        return "Unknown";
    }

    private async Task<int> GetAchievements(
        string steamId,
        int appId)
    {
        try
        {
            var json =
                    await _api.GetAchievementsJson(
                    steamId,
                    appId);

            if (string.IsNullOrWhiteSpace(json))
                return 0;

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty(
                "playerstats",
                out var playerstats))
            {
                if (playerstats.TryGetProperty(
                    "achievements",
                    out var achievements))
                {
                    int count = achievements
                        .EnumerateArray()
                        .Count(a =>
                            a.GetProperty("achieved")
                            .GetInt32() == 1);

                    Console.WriteLine(
                        $"APPID {appId}: {count} achievements");

                    return count;
                }
            }

            Console.WriteLine(
                $"APPID {appId}: achievements not found");
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"APPID {appId} ERROR: {ex.Message}");
        }

        return 0;
    }
    public async Task<SteamPlayer?> GetPlayerProfile(string steamId)
    {
        var json = await _api.GetPlayerSummary(steamId);

        if (string.IsNullOrWhiteSpace(json))
            return null;

        var profile =
            JsonSerializer.Deserialize<SteamProfileResponse>(json);

        return profile?
            .Response?
            .Players?
            .FirstOrDefault();
    }
    public async Task<int> GetSteamLevel(string steamId)
    {
        try
        {
            var json = await _api.GetSteamLevel(steamId);

            if (string.IsNullOrWhiteSpace(json))
                return 0;

            var level =
                JsonSerializer.Deserialize<SteamLevelResponse>(json);

            return level?.Response?.PlayerLevel ?? 0;
        }
        catch
        {
            return 0;
        }
    }
    public async Task<string> ResolveSteamId(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";

        input = input.Trim();

        // Уже SteamID64
        if (input.All(char.IsDigit))
            return input;

        // profiles/7656119...
        if (input.Contains("/profiles/"))
        {
            var parts = input.Split('/');
            return parts.Last(x => !string.IsNullOrWhiteSpace(x));
        }

        // id/username
        if (input.Contains("/id/"))
        {
            var parts = input.Split('/');

            var vanityName =
                parts.SkipWhile(x => x != "id")
                     .Skip(1)
                     .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(vanityName))
                throw new Exception("Неверная ссылка Steam");

            var json =
                await _api.ResolveVanityUrl(vanityName);

            if (string.IsNullOrWhiteSpace(json))
                throw new Exception("Не удалось получить SteamID");

            var result =
                JsonSerializer.Deserialize<ResolveVanityResponse>(json);

            if (result?.Response?.Success != 1)
                throw new Exception("Профиль не найден");

            return result.Response.SteamId;
        }

        throw new Exception("Неверный Steam ID");
    }
    private async Task<int> GetAchievementsTotal(int appId)
    {
        if (AchievementTotalCache.TryGetValue(appId, out var cached))
            return cached;

        try
        {
            var json = await _api.GetSchemaForGame(appId);

            if (string.IsNullOrWhiteSpace(json))
                return 0;

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("game", out var game) &&
                game.TryGetProperty("availableGameStats", out var stats) &&
                stats.TryGetProperty("achievements", out var achievements))
            {
                var total = achievements.GetArrayLength();

                AchievementTotalCache[appId] = total;

                return total;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TOTAL ACH ERROR {appId}: {ex.Message}");
        }

        return 0;
    }
}