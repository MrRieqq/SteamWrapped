namespace SteamWrapped.Services;

public static class ThemeService
{
    private const string ThemeKey = "AppTheme";

    // Тёмная тема (оригинальная)
    private static readonly Dictionary<string, Color> DarkColors = new()
    {
        ["PageBackground"] = Color.FromArgb("#0F1923"),
        ["CardBackground"] = Color.FromArgb("#111827"),
        ["CardStroke"] = Color.FromArgb("#263244"),
        ["TextPrimary"] = Colors.White,
        ["TextSecondary"] = Color.FromArgb("#8A9BB8"),
        ["ShellBackground"] = Color.FromArgb("#171A21"),
    };

    // Полночная тема (глубокий тёмно-фиолетовый)
    private static readonly Dictionary<string, Color> MidnightColors = new()
    {
        ["PageBackground"] = Color.FromArgb("#0D0B1E"),
        ["CardBackground"] = Color.FromArgb("#13102B"),
        ["CardStroke"] = Color.FromArgb("#2E2660"),
        ["TextPrimary"] = Color.FromArgb("#EAE6FF"),
        ["TextSecondary"] = Color.FromArgb("#9B8FCC"),
        ["ShellBackground"] = Color.FromArgb("#0A0818"),
    };

    public static bool IsDark { get; private set; } = true;

    public static void LoadAndApply()
    {
        var saved = Preferences.Default.Get(ThemeKey, "dark");
        Apply(saved == "midnight" ? false : true);
    }

    /// <summary>dark=true → тёмная, dark=false → полночная</summary>
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
