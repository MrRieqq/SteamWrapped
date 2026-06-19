using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;
using Microsoft.Maui.ApplicationModel;
using System.Reflection;

public partial class AboutViewModel : ObservableObject
{
    public string BuildDate =>
        GetBuildDate().ToString("dd MMMM yyyy", new CultureInfo("ru-RU"));

    private static DateTime GetBuildDate()
    {
        return File.GetLastWriteTime(
            Assembly.GetExecutingAssembly().Location);
    }
}