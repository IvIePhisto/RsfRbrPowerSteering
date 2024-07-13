using System.Globalization;
using System.Windows.Data;

namespace RsfRbrPowerSteering.Converters;

internal class AllTrueConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        => values.Cast<bool?>().All(b => b ?? false);

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
