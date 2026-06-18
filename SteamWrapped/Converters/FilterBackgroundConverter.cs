using System.Globalization;

namespace SteamWrapped.Converters;

public class FilterBackgroundConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value?.ToString() == parameter?.ToString())
            return Color.FromArgb("#355CFF");

        return Color.FromArgb("#121B2B");
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    public class ViewModeBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            bool isGridView = (bool)value;
            string mode = parameter?.ToString();

            bool isActive =
                (mode == "Grid" && isGridView) ||
                (mode == "List" && !isGridView);

            return Color.FromArgb(
                isActive ? "#355CFF" : "#121B2B");
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}