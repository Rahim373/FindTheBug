using System.Windows.Markup;

namespace FindTheBug.Desktop.Reception.Extensions;

public class EnumBindingSourceExtension : MarkupExtension
{
    public Type EnumType { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (EnumType == null || !EnumType.IsEnum)
            throw new InvalidOperationException("EnumType must be an enum.");

        return Enum.GetValues(EnumType);
    }
}
