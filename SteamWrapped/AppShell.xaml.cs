using SteamWrapped.Pages;

namespace SteamWrapped;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(
            nameof(AlAchievementsPage),
            typeof(AlAchievementsPage));
    }
}