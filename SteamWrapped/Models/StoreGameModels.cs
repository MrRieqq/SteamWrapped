namespace SteamWrapped.Models;

public class GenreDto
{
    public string description { get; set; }
}

public class StoreGameData
{
    public List<GenreDto> genres { get; set; }
}

public class StoreGameResponse
{
    public bool success { get; set; }

    public StoreGameData data { get; set; }
}