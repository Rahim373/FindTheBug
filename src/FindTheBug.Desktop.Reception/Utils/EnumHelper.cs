using System.ComponentModel;
using System.Reflection;

namespace FindTheBug.Desktop.Reception.Utils;

public static class EnumHelper
{
    public static IEnumerable<T> GetValues<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static string GetDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        return field?.GetCustomAttribute<DescriptionAttribute>()?.Description
               ?? value.ToString();
    }
}
