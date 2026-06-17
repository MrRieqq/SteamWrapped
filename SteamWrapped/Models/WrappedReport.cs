using System;
using System.Collections.Generic;
using System.Text;

namespace SteamWrapped.Models;

public class WrappedReport
{
    public int TotalHours { get; set; }

    public string FavoriteGame { get; set; }

    public string FavoriteGenre { get; set; }

    public string PlayerType { get; set; }

    public int GamesPlayed { get; set; }

    public double AverageHoursPerGame { get; set; }

    public string TopGame1 { get; set; }

    public string TopGame2 { get; set; }

    public string TopGame3 { get; set; }

    public int TotalAchievements { get; set; }

    public string MostPlayedGenre { get; set; }

    public string GamerRank { get; set; }
}