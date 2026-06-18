using System.Text.Json.Serialization;

namespace SteamWrapped.Models;

public class Game
{
    public int AppId { get; set; }

    public string Name { get; set; } = "";

    public string Genre { get; set; } = "Unknown";

    public int HoursPlayed { get; set; }

    public int Sessions { get; set; }

    public int Achievements { get; set; }

    public int YearPlayed { get; set; }
    public string ImageUrl =>
    $"https://cdn.cloudflare.steamstatic.com/steam/apps/{AppId}/library_600x900.jpg";
}
public class SteamResponse
{
    public int game_count { get; set; }

    public List<SteamGameDto> games { get; set; }
}
public class ResolveVanityResponse
{
    [JsonPropertyName("response")]
    public VanityResponse Response { get; set; }
}

public class VanityResponse
{
    [JsonPropertyName("success")]
    public int Success { get; set; }

    [JsonPropertyName("steamid")]
    public string SteamId { get; set; }
}