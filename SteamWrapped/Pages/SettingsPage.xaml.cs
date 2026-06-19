using SteamWrapped.Services;

namespace SteamWrapped.Pages;

public partial class WrappedPage : ContentPage
{
    public WrappedPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateThemeUI(ThemeService.IsDark);
    }

    private void OnDarkThemeTapped(object sender, TappedEventArgs e)
    {
        ThemeService.Apply(dark: true);
        UpdateThemeUI(isDark: true);
    }

    private void OnMidnightThemeTapped(object sender, TappedEventArgs e)
    {
        ThemeService.Apply(dark: false);
        UpdateThemeUI(isDark: false);
    }

    private void UpdateThemeUI(bool isDark)
    {
        // Тёмная кнопка
        DarkBorder.Stroke = isDark ? Color.FromArgb("#4F6FFF") : Color.FromArgb("#263244");
        DarkBorder.StrokeThickness = isDark ? 2 : 1;
        DarkCheckmark.IsVisible = isDark;
        DarkLabel.TextColor = isDark ? Colors.White : Color.FromArgb("#8A9BB8");

        // Полночная кнопка
        MidnightBorder.Stroke = isDark ? Color.FromArgb("#2E2660") : Color.FromArgb("#6C5CE7");
        MidnightBorder.StrokeThickness = isDark ? 1 : 2;
        MidnightCheckmark.IsVisible = !isDark;
        MidnightLabel.TextColor = isDark ? Color.FromArgb("#9B8FCC") : Color.FromArgb("#EAE6FF");
    }
}
