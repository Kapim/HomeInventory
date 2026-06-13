using System.Globalization;

namespace HomeInventory.Mobile.Maui.Converters;

public class DepthToMarginConverter : IValueConverter
{
    private const double IndentWidth = 16.0;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var depth = value is int d ? d : 0;
        return new Thickness(depth * IndentWidth + 4, 2, 4, 2);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
