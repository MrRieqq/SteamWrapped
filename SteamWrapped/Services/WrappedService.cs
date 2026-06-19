using SteamWrapped.Models;
using System.Text.Json;
using static SteamWrapped.Models.Game;
using static SteamWrapped.Services.SteamApiService;

namespace SteamWrapped.Services;

public class WrappedService
{
    private static readonly Dictionary<int, string> GenreCache = new();
    private static readonly Dictionary<int, AchievementInfo>
    AchievementCache = new();
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
            .ToList();

        var tasks = topGames.Select(async g =>
        {
            var genreTask = GetGenre(g.appid);
            var achievementTask = GetAchievements(
                steamId,
                g.appid);

            await Task.WhenAll(
                genreTask,
                achievementTask);

            var achievementInfo =
                await achievementTask;

            return new Game
            {
                AppId = g.appid,
                Name = g.name,
                HoursPlayed = g.playtime_forever / 60,
                Genre = await genreTask,
                AchievementsUnlocked = achievementInfo.Unlocked,
                AchievementsTotal = achievementInfo.Total
            };
        });

        return (await Task.WhenAll(tasks)).ToList();
    }

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

    private async Task<AchievementInfo> GetAchievements(
    string steamId,
    int appId)
    {
        if (AchievementCache.TryGetValue(appId, out var cached))
            return cached;

        try
        {
            var json =
                await _api.GetAchievementsJson(
                    steamId,
                    appId);

            if (string.IsNullOrWhiteSpace(json))
            {
                var empty = new AchievementInfo();

                AchievementCache[appId] = empty;

                return empty;
            }

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty(
                "playerstats",
                out var playerstats))
            {
                if (playerstats.TryGetProperty(
                    "achievements",
                    out var achievements))
                {
                    var list =
                        achievements
                        .EnumerateArray()
                        .ToList();

                    var achievementInfo =
                        new AchievementInfo
                        {
                            Total = list.Count,
                            Unlocked = list.Count(a =>
                                a.GetProperty("achieved")
                                 .GetInt32() == 1)
                        };

                    AchievementCache[appId] = achievementInfo;

                    return achievementInfo;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"APPID {appId} ERROR: {ex.Message}");
        }

        var result = new AchievementInfo();

        AchievementCache[appId] = result;

        return result;
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
    public async Task<List<RecentGame>> GetRecentGames(string steamId)
    {
        var json =
            await _api.GetRecentlyPlayedGames(steamId);

        if (string.IsNullOrWhiteSpace(json))
            return [];

        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement
            .GetProperty("response")
            .TryGetProperty("games", out var games))
            return [];

        return games
            .EnumerateArray()
            .Select(x => new RecentGame
            {
                AppId = x.GetProperty("appid").GetInt32(),
                Name = x.GetProperty("name").GetString() ?? "",
                MinutesLast2Weeks =
                    x.GetProperty("playtime_2weeks").GetInt32()
            })
            .ToList();
    }
    public async Task<List<RecentAchievement>>
    GetRecentAchievements(string steamId)
    {
        var games = await GetSteamGames(steamId);

        var result = new List<RecentAchievement>();

        foreach (var game in games.Take(20))
        {
            var schema =
    await GetAchievementSchema(game.AppId);
            var achievements =
                await _api.GetAchievementsJson(
                    steamId,
                    game.AppId);

            if (string.IsNullOrWhiteSpace(achievements))
                continue;

            using var doc =
                JsonDocument.Parse(achievements);

            if (!doc.RootElement.TryGetProperty(
                    "playerstats",
                    out var playerstats))
                continue;

            if (!playerstats.TryGetProperty(
                    "achievements",
                    out var achs))
                continue;

            foreach (var a in achs.EnumerateArray())
            {
                if (a.GetProperty("achieved")
                    .GetInt32() != 1)
                    continue;

                if (!a.TryGetProperty(
                        "unlocktime",
                        out var unlock))
                    continue;
                var apiName =
    a.GetProperty("apiname").GetString() ?? "";

                string achievementName =
                    schema.TryGetValue(apiName, out var info)
                        ? info.Name
                        : apiName;

                string description =
                    schema.TryGetValue(apiName, out info)
                        ? info.Description
                        : "";

                result.Add(new RecentAchievement
                {
                    AppId = game.AppId,
                    GameName = game.Name,
                    AchievementName = achievementName,
                    Description = description,
                    UnlockTime = unlock.GetInt64()
                });
            }
        }

        return result
            .OrderByDescending(x => x.UnlockTime)
            .ToList();
    }
    private async Task<Dictionary<string, (string Name, string Description)>>
GetAchievementSchema(int appId)
    {
        var json = await _api.GetGameSchema(appId);

        var result =
            new Dictionary<string, (string, string)>();

        if (string.IsNullOrWhiteSpace(json))
            return result;

        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement
            .GetProperty("game")
            .TryGetProperty("availableGameStats", out var stats))
            return result;

        if (!stats.TryGetProperty("achievements", out var achs))
            return result;

        foreach (var ach in achs.EnumerateArray())
        {
            var apiName =
                ach.GetProperty("name").GetString() ?? "";

            var displayName =
                ach.TryGetProperty("displayName", out var dn)
                ? dn.GetString() ?? ""
                : "";

            var description =
                ach.TryGetProperty("description", out var desc)
                ? desc.GetString() ?? ""
                : "";

            result[apiName] =
                (displayName, description);
        }

        return result;
    }
}