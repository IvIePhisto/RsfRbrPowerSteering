using System.Globalization;
using System.Windows.Data;

namespace RsfRbrPowerSteering.Converters;

internal class UpperTextConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is null
            ? null
            : targetType == typeof(string)
                ? value.ToString()?.ToUpperInvariant()
                : value;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
