using System;
using System.Collections.Generic;
using System.Text;

namespace SteamWrapped.Models
{
    public static class AppData
    {
        public static string SteamId { get; set; } = "";

        public static string PlayerName { get; set; } = "";

        public static string AvatarUrl { get; set; } = "";

        public static int PersonaState { get; set; }
        public static string CurrentGame { get; set; } = "";
    }
}
