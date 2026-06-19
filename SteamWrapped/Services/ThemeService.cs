namespace SteamWrapped.Services;

public static class ThemeService
{
    private const string ThemeKey = "AppTheme";

    private static readonly Dictionary<string, Color> DarkColors = new()
    {
        ["PageBackground"] = Color.FromArgb("#0F1923"),
        ["CardBackground"] = Color.FromArgb("#111827"),
        ["CardInner"] = Color.FromArgb("#1A2535"),
        ["CardInner2"] = Color.FromArgb("#1E2D45"),
        ["CardInner3"] = Color.FromArgb("#1E293B"),
        ["CardStroke"] = Color.FromArgb("#263244"),
        ["Divider"] = Color.FromArgb("#263244"),
        ["TextPrimary"] = Colors.White,
        ["TextSecondary"] = Color.FromArgb("#8A9BB8"),
        ["ShellBackground"] = Color.FromArgb("#171A21"),
        ["MainCard"] = Color.FromArgb("#0D1726"),
        ["MainCardStroke"] = Color.FromArgb("#1D3350"),
        ["MainCardInner"] = Color.FromArgb("#132033"),
        ["MainCard2"] = Color.FromArgb("#09111D"),
        ["MainCard2Stroke"] = Color.FromArgb("#1A2A40"),
        ["MainAccent"] = Color.FromArgb("#53B9FF"),
        ["MainAccent2"] = Color.FromArgb("#B9E4FF"),
        ["MainAccent3"] = Color.FromArgb("#8B79FF"),
        ["MainSubText"] = Color.FromArgb("#7E8DA2"),
        ["MainSubText2"] = Color.FromArgb("#6F8198"),
        ["GameCard"] = Color.FromArgb("#101827"),
        ["GameCardStroke"] = Color.FromArgb("#1F2B40"),
        ["GameCardInner"] = Color.FromArgb("#1A2437"),
        ["GameSearchBg"] = Color.FromArgb("#121B2B"),
        ["GameTextSub"] = Color.FromArgb("#7E8AA4"),
        ["GameTextSub2"] = Color.FromArgb("#8D9AB4"),
        ["GameTextSub3"] = Color.FromArgb("#9FB0CC"),
    };

    private static readonly Dictionary<string, Color> MidnightColors = new()
    {
        ["PageBackground"] = Color.FromArgb("#0D0B1E"),
        ["CardBackground"] = Color.FromArgb("#13102B"),
        ["CardInner"] = Color.FromArgb("#1E1A3A"),
        ["CardInner2"] = Color.FromArgb("#221E42"),
        ["CardInner3"] = Color.FromArgb("#1A1733"),
        ["CardStroke"] = Color.FromArgb("#2E2660"),
        ["Divider"] = Color.FromArgb("#2E2660"),
        ["TextPrimary"] = Color.FromArgb("#EAE6FF"),
        ["TextSecondary"] = Color.FromArgb("#9B8FCC"),
        ["ShellBackground"] = Color.FromArgb("#0A0818"),
        ["MainCard"] = Color.FromArgb("#110E28"),
        ["MainCardStroke"] = Color.FromArgb("#2A2258"),
        ["MainCardInner"] = Color.FromArgb("#1A1640"),
        ["MainCard2"] = Color.FromArgb("#0A0820"),
        ["MainCard2Stroke"] = Color.FromArgb("#221E50"),
        ["MainAccent"] = Color.FromArgb("#A78BFA"),
        ["MainAccent2"] = Color.FromArgb("#DDD6FE"),
        ["MainAccent3"] = Color.FromArgb("#C4B5FD"),
        ["MainSubText"] = Color.FromArgb("#7C6FB0"),
        ["MainSubText2"] = Color.FromArgb("#6B5FA0"),
        ["GameCard"] = Color.FromArgb("#100E25"),
        ["GameCardStroke"] = Color.FromArgb("#2A2258"),
        ["GameCardInner"] = Color.FromArgb("#1C1940"),
        ["GameSearchBg"] = Color.FromArgb("#13102B"),
        ["GameTextSub"] = Color.FromArgb("#7B6FB5"),
        ["GameTextSub2"] = Color.FromArgb("#8B7FC0"),
        ["GameTextSub3"] = Color.FromArgb("#9B90CC"),
    };

    public static bool IsDark { get; private set; } = true;

    public static void LoadAndApply()
    {
        var saved = Preferences.Default.Get(ThemeKey, "dark");
        Apply(saved != "midnight");
    }

    public static void Apply(bool dark)
    {
        IsDark = dark;
        Preferences.Default.Set(ThemeKey, dark ? "dark" : "midnight");

        var colors = dark ? DarkColors : MidnightColors;
        var res = Application.Current?.Resources;
        if (res == null) return;

        foreach (var kv in colors)
            res[kv.Key] = kv.Value;

        Application.Current!.UserAppTheme = AppTheme.Dark;
    }
}
