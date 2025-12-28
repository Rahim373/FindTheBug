using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FindTheBug.Desktop.Reception.Converters;

/// <summary>
/// Converts a validation error state to a brush color
/// </summary>
public class ValidationErrorToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool hasError)
        {
            return hasError 
                ? new SolidColorBrush(Colors.LightPink) 
                : new SolidColorBrush(Colors.White);
        }

        if (value is string errorMessage)
        {
            return !string.IsNullOrEmpty(errorMessage)
                ? new SolidColorBrush(Colors.LightPink)
                : new SolidColorBrush(Colors.White);
        }

        return new SolidColorBrush(Colors.White);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a validation error state to a border brush color
/// </summary>
public class ValidationErrorToBorderColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool hasError)
        {
            return hasError
                ? new SolidColorBrush(Colors.Red)
                : new SolidColorBrush(Color.FromRgb(171, 171, 171));
        }

        if (value is string errorMessage)
        {
            return !string.IsNullOrEmpty(errorMessage)
                ? new SolidColorBrush(Colors.Red)
                : new SolidColorBrush(Color.FromRgb(171, 171, 171));
        }

        return new SolidColorBrush(Color.FromRgb(171, 171, 171));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}