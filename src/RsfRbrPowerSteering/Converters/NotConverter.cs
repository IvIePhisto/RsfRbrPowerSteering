using System.Globalization;
using System.Windows.Data;

namespace RsfRbrPowerSteering.Converters
{
    internal class NotConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value == null
                ? null
                : value is bool boolValue
                    ? !boolValue
                    : null;

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Convert(value, targetType, parameter, culture);
    }
}
