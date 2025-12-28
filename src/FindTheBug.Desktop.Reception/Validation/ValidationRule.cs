namespace FindTheBug.Desktop.Reception.Validation;

/// <summary>
/// Defines a validation rule for a property
/// </summary>
public class ValidationRule<T>
{
    public string PropertyName { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public Func<T?, bool>? ValidationDelegate { get; set; }
    public Func<T?, string?>? ErrorMessageDelegate { get; set; }

    public bool Validate(T? value, out string? errorMessage)
    {
        errorMessage = null;

        if (ValidationDelegate == null)
        {
            errorMessage = "No validation rule defined";
            return false;
        }

        bool isValid = ValidationDelegate(value);

        if (!isValid)
        {
            errorMessage = ErrorMessageDelegate?.Invoke(value) ?? ErrorMessage ?? "Invalid value";
        }

        return isValid;
    }
}