namespace SteamWrapped.Pages;

public partial class AboutPage : ContentPage
{
    public Command OpenSteamApiCommand { get; }
    public Command OpenMauiCommand { get; }
    public Command OpenFaqCommand { get; }
    public Command OpenGitCommand { get; }

    public AboutPage()
    {
        InitializeComponent();

        OpenSteamApiCommand = new Command(async () =>
            await Launcher.OpenAsync("https://steamcommunity.com/dev"));

        OpenMauiCommand = new Command(async () =>
            await Launcher.OpenAsync("https://dotnet.microsoft.com/apps/maui"));

        OpenFaqCommand = new Command(async () =>
            await Launcher.OpenAsync("https://telegra.ph/SteamWrapped--FAQ-06-19"));

        OpenGitCommand = new Command(async () =>
            await Launcher.OpenAsync("https://github.com/MrRieqq/SteamWrapped"));

        BindingContext = this;
        BindingContext = new AboutViewModel();
    }
}