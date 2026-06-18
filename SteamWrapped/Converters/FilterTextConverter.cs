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
        if (value?.ToString() == parameter?.ToString())
            return Colors.White;

        return Color.FromArgb("#9FB0CC");
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    public class ViewModeTextConverter : IValueConverter
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
                isActive ? "#FFFFFF" : "#8D9AB4");
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}