using System.Globalization;

namespace SteamWrapped.Converters;

public class FilterTextConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        bool isActive = value?.ToString() == parameter?.ToString();
        return GetThemeColor(isActive ? "FilterActiveText" : "FilterInactiveText",
                             isActive ? "#FFFFFF" : "#9FB0CC");
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) => throw new NotImplementedException();

    // ── helper ──────────────────────────────────────────────────────────────
    private static Color GetThemeColor(string key, string fallback)
    {
        if (Application.Current?.Resources.TryGetValue(key, out var raw) == true
            && raw is Color c)
            return c;

        return Color.FromArgb(fallback);
    }

    // ── nested: view-mode (Grid / List) buttons ──────────────────────────────
    public class ViewModeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            bool isGridView = (bool)value;
            string? mode = parameter?.ToString();

            bool isActive =
                (mode == "Grid" && isGridView) ||
                (mode == "List" && !isGridView);

            return GetThemeColor(isActive ? "FilterActiveText" : "FilterInactiveText",
                                 isActive ? "#FFFFFF" : "#8D9AB4");
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
