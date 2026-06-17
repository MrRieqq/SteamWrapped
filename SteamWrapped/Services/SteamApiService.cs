using System.Net;
using SteamWrapped.Models;

namespace SteamWrapped.Services;

public class SteamApiService
{
    private const string ApiKey = "FE3B59EFEB6371A8A3BF392A48AE3613";

    private static readonly HttpClient _client;

    static SteamApiService()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression =
                DecompressionMethods.GZip |
                DecompressionMethods.Deflate
        };

        _client = new HttpClient(handler);

        _client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "SteamWrapped/1.0");

        _client.Timeout = TimeSpan.FromSeconds(30);
    }

    private async Task<string?> SafeGetStringAsync(string url)
    {
        try
        {
            return await _client.GetStringAsync(url);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP ERROR: {ex}");
            return null;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"TIMEOUT: {ex}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UNKNOWN ERROR: {ex}");
            return null;
        }
    }

    public async Task<string?> GetOwnedGamesJson(string steamId)
    {
        var url =
            $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/" +
            $"?key={ApiKey}" +
            $"&steamid={steamId}" +
            $"&include_appinfo=true" +
            $"&format=json";

        return await SafeGetStringAsync(url);
    }

    public async Task<string?> GetGameDetails(int appId)
    {
        var url =
            $"https://store.steampowered.com/api/appdetails?appids={appId}";

        return await SafeGetStringAsync(url);
    }

    public async Task<string?> GetAchievementsJson(
        string steamId,
        int appId)
    {
        var url =
            $"https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v0001/" +
            $"?appid={appId}" +
            $"&key={ApiKey}" +
            $"&steamid={steamId}";

        return await SafeGetStringAsync(url);
    }
}