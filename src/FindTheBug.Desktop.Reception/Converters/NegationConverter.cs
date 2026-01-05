using System.Globalization;
using System.Windows.Data;

namespace FindTheBug.Desktop.Reception.Converters;

internal class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool d)
        {
            return !d;
        }
        // Return original value or handle other types as needed
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // This is used for TwoWay binding; return the negative of the input
        if (value is bool d)
        {
            return !d;
        }
        return value;
    }
}
