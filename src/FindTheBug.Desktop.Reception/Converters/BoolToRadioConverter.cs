using System.Globalization;
using System.Windows.Data;

namespace FindTheBug.Desktop.Reception.Converters;

public class BooleanToRadioButtonConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue && boolValue.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? parameter : System.Windows.Data.Binding.DoNothing;
    }
}
