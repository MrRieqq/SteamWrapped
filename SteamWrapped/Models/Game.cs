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
    public int AchievementsUnlocked { get; set; }

    public int AchievementsTotal { get; set; }

    public string AchievementText =>
        AchievementsTotal == 0
            ? "Нет достижений"
            : $"🏆{AchievementsUnlocked} / {AchievementsTotal}";

    public double AchievementPercent =>
        AchievementsTotal == 0
            ? 0
            : (double)AchievementsUnlocked / AchievementsTotal;
    public int YearPlayed { get; set; }
    public string ImageUrl =>
    $"https://cdn.cloudflare.steamstatic.com/steam/apps/{AppId}/header.jpg";
    public string AchievementPercentText =>
        AchievementsTotal == 0
            ? ""
            : $"{AchievementPercent * 100:F0}%"; 
    public bool HasAchievements =>
    AchievementsTotal > 0;
    public int Rank { get; set; }

    public double PlaytimeProgress { get; set; }
    public string ListImageUrl =>
    $"https://cdn.cloudflare.steamstatic.com/steam/apps/{AppId}/library_600x900.jpg";
    public class RecentAchievement
    {
        public string GameName { get; set; } = "";

        public string AchievementName { get; set; } = "";

        public string Description { get; set; } = "";

        public int AppId { get; set; }

        public long UnlockTime { get; set; }

        public string ImageUrl =>
            $"https://cdn.cloudflare.steamstatic.com/steam/apps/{AppId}/header.jpg";
        public string ListImageUrl =>
   $"https://cdn.cloudflare.steamstatic.com/steam/apps/{AppId}/library_600x900.jpg";
        public DateTime UnlockDate =>
            DateTimeOffset
                .FromUnixTimeSeconds(UnlockTime)
                .LocalDateTime;
    }
}
public class AchievementInfo
{
    public int Unlocked { get; set; }
    public int Total { get; set; }
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
