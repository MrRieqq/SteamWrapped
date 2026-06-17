using System.Text.Json;

namespace SteamWrapped.Services;

public class SteamApiService
{
    private const string ApiKey = "FE3B59EFEB6371A8A3BF392A48AE3613";

    public async Task<string> GetOwnedGamesJson(string steamId)
    {
        using var client = new HttpClient();

        var url =
            $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/" +
            $"?key={ApiKey}" +
            $"&steamid={steamId}" +
            $"&include_appinfo=true" +
            $"&format=json";

        return await client.GetStringAsync(url);
    }
}