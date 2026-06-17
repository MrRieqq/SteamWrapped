using System;
using System.Collections.Generic;
using System.Text;

namespace SteamWrapped.Models;

public class SteamGameDto
{
    public int appid { get; set; }

    public string name { get; set; }

    public int playtime_forever { get; set; }
}
