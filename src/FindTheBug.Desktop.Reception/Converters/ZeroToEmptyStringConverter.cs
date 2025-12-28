using System.Globalization;
using System.Windows.Data;

namespace FindTheBug.Desktop.Reception.Converters;

public class ZeroToEmptyStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Check if the value is a number and is 0
        if (value is int intValue && intValue == 0)
        {
            return string.Empty; // Return empty string
        }
        if (value is double doubleValue && doubleValue == 0.0)
        {
            return string.Empty;
        }
        if (value is decimal decimalValue && decimalValue == 0.0m)
            return string.Empty;

        // For any other value, return the value as a string
        return value?.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // If the textbox is empty or null, convert back to 0 (or a default value) for the source property
        if (string.IsNullOrEmpty(value as string))
        {
            // Return 0 if the target type expects an int, double, etc.
            if (targetType == typeof(int) || targetType == typeof(int?)) return 0;
            if (targetType == typeof(double) || targetType == typeof(double?)) return 0.0;
            if (targetType == typeof(decimal) || targetType == typeof(decimal?)) return 0.0m;
            // Add other numeric types as needed
        }

        // Attempt to parse the non-empty string back to the target type
        // This usually requires more robust validation/TryParse logic for real applications
        try
        {
            return System.Convert.ChangeType(value, targetType, culture);
        }
        catch
        {
            return System.Windows.DependencyProperty.UnsetValue; // Indicates conversion failure
        }
    }
}
