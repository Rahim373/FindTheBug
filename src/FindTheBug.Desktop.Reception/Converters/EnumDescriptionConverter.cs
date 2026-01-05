using FindTheBug.Desktop.Reception.Utils;
using System.Globalization;
using System.Windows.Data;

namespace FindTheBug.Desktop.Reception.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Enum enumValue
                ? EnumHelper.GetDescription(enumValue)
                : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => System.Windows.Data.Binding.DoNothing;
    }
}
