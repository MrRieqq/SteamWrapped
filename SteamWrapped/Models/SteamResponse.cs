using System;
using System.Collections.Generic;
using System.Text;

namespace SteamWrapped.Models;

public class SteamResponse
{
    public int game_count { get; set; }

    public List<SteamGameDto> games { get; set; }
}