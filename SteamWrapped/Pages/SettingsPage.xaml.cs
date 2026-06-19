using SteamWrapped.Services;
using SteamWrapped.Models;
using Microsoft.Maui.ApplicationModel;

namespace SteamWrapped.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        UpdateThemeUI(ThemeService.IsDark);
        UpdateSteamProfileInfo();
        ProfileName.Text = string.IsNullOrWhiteSpace(AppData.PlayerName)
            ? "Профиль не загружен"
            : AppData.PlayerName;

        ProfileSteamId.Text =
            $"steamid64: {AppData.SteamId}";

        ProfileAvatar.Source =
            string.IsNullOrWhiteSpace(AppData.AvatarUrl)
                ? "avatar.png"
                : ImageSource.FromUri(
                    new Uri(AppData.AvatarUrl));

    }
    private void UpdateSteamProfileInfo()
    {
        ProfileSteamId.Text = $"steamid64: {AppData.SteamId}";

        // Текущая игра
        if (!string.IsNullOrWhiteSpace(AppData.CurrentGame))
        {
            CurrentGameText.IsVisible = true;
            CurrentGameText.Text = $"🎮 Играет в {AppData.CurrentGame}";
        }
        else
        {
            CurrentGameText.IsVisible = false;
        }

        switch (AppData.PersonaState)
        {
            case 0:
                StatusIndicator.BackgroundColor = Colors.Gray;
                StatusText.Text = "Не в сети";
                break;

            case 1:
                StatusIndicator.BackgroundColor = Colors.LimeGreen;
                StatusText.Text = "В сети";
                break;

            case 2:
                StatusIndicator.BackgroundColor = Colors.Red;
                StatusText.Text = "Занят";
                break;

            case 3:
                StatusIndicator.BackgroundColor = Colors.Gold;
                StatusText.Text = "Нет на месте";
                break;

            case 4:
                StatusIndicator.BackgroundColor = Colors.Orange;
                StatusText.Text = "Спит";
                break;

            case 5:
                StatusIndicator.BackgroundColor = Colors.DeepSkyBlue;
                StatusText.Text = "Ищет обмен";
                break;

            case 6:
                StatusIndicator.BackgroundColor = Colors.MediumPurple;
                StatusText.Text = "Ищет игру";
                break;

            default:
                StatusIndicator.BackgroundColor = Colors.Gray;
                StatusText.Text = "Неизвестно";
                break;
        }
    }
    private async void OpenSteamProfileTapped(object sender, TappedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(AppData.SteamId))
            return;

        await Launcher.Default.OpenAsync(
            $"https://steamcommunity.com/profiles/{AppData.SteamId}");
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
