using System;
using System.Collections.Generic;
using System.Text;

namespace SteamWrapped.Models;

public class Game
{
    public string Name { get; set; }

    public string Genre { get; set; }

    public int HoursPlayed { get; set; }

    public int Sessions { get; set; }

    public int Achievements { get; set; }

    public int YearPlayed { get; set; }
}
