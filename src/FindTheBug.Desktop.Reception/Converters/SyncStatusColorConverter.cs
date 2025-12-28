using FindTheBug.Desktop.Reception.Services.CloudSync;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FindTheBug.Desktop.Reception.Converters;

/// <summary>
/// Converts SyncState properties to colors for the sync status indicator
/// </summary>
public class SyncStatusColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var state = (SyncStateEnum?)value;

        return state switch
        {
            SyncStateEnum.InProgress => Colors.Yellow,
            SyncStateEnum.Success => Colors.Green,
            SyncStateEnum.Fail => Colors.Red,
            _ => Colors.Gray
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}