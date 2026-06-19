using Microsoft.Extensions.DependencyInjection;
using SteamWrapped.Services;

namespace SteamWrapped
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            ThemeService.LoadAndApply();

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}