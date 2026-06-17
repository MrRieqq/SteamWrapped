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