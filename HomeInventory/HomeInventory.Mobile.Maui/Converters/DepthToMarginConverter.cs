using System.Globalization;

namespace HomeInventory.Mobile.Maui.Converters;

public class DepthToMarginConverter : IValueConverter
{
    private const double IndentWidth = 14.0;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var depth = value is int d ? d : 0;
        return new Thickness(depth * IndentWidth, 0, 0, 0);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
