using System.Text.Json.Serialization;

namespace SteamWrapped.Models;

public class SteamProfileResponse
{
    [JsonPropertyName("response")]
    public SteamProfileContainer Response { get; set; }
}

public class SteamProfileContainer
{
    [JsonPropertyName("players")]
    public List<SteamPlayer> Players { get; set; }
}

public class SteamPlayer
{
    [JsonPropertyName("personaname")]
    public string PersonaName { get; set; }

    [JsonPropertyName("avatarfull")]
    public string AvatarFull { get; set; }

    [JsonPropertyName("personastate")]
    public int PersonaState { get; set; }
}
public class SteamLevelResponse
{
    [JsonPropertyName("response")]
    public SteamLevelData Response { get; set; }
}

public class SteamLevelData
{
    [JsonPropertyName("player_level")]
    public int PlayerLevel { get; set; }
}