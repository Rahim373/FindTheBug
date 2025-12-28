namespace FindTheBug.Desktop.Reception.Validation;

/// <summary>
/// Predefined validation rules for common scenarios
/// </summary>
public static class Validators
{
    /// <summary>
    /// Validates that a string is not null or whitespace
    /// </summary>
    public static ValidationRule<string> IsNotEmpty(string propertyName, string? errorMessage = null)
    {
        return new ValidationRule<string>
        {
            PropertyName = propertyName,
            ErrorMessage = errorMessage ?? $"{propertyName} is required",
            ValidationDelegate = value => !string.IsNullOrWhiteSpace(value)
        };
    }

    /// <summary>
    /// Validates that a string has a minimum length
    /// </summary>
    public static ValidationRule<string> MinLength(int minLength, string propertyName, string? errorMessage = null)
    {
        return new ValidationRule<string>
        {
            PropertyName = propertyName,
            ErrorMessageDelegate = value => errorMessage ?? $"{propertyName} must be at least {minLength} characters",
            ValidationDelegate = value => !string.IsNullOrWhiteSpace(value) && value.Length >= minLength
        };
    }

    /// <summary>
    /// Validates that a string has a maximum length
    /// </summary>
    public static ValidationRule<string> MaxLength(int maxLength, string propertyName, string? errorMessage = null)
    {
        return new ValidationRule<string>
        {
            PropertyName = propertyName,
            ErrorMessageDelegate = value => errorMessage ?? $"{propertyName} cannot exceed {maxLength} characters",
            ValidationDelegate = value => string.IsNullOrWhiteSpace(value) || value.Length <= maxLength
        };
    }

    /// <summary>
    /// Validates phone number format (Bangladeshi format: 01XXXXXXXXX)
    /// </summary>
    public static ValidationRule<string> IsValidPhoneNumber(string? errorMessage = null)
    {
        return new ValidationRule<string>
        {
            PropertyName = "Phone Number",
            ErrorMessage = errorMessage ?? "Please enter a valid phone number (e.g., 01XXXXXXXXX)",
            ValidationDelegate = value => !string.IsNullOrWhiteSpace(value) && 
                                         value.Length == 11 && 
                                         value.StartsWith("01") && 
                                         value.All(char.IsDigit)
        };
    }

    /// <summary>
    /// Validates that a numeric value is positive
    /// </summary>
    public static ValidationRule<decimal> IsPositive(string propertyName, string? errorMessage = null)
    {
        return new ValidationRule<decimal>
        {
            PropertyName = propertyName,
            ErrorMessage = errorMessage ?? $"{propertyName} must be greater than 0",
            ValidationDelegate = value => value > 0
        };
    }

    /// <summary>
    /// Validates that a numeric value is non-negative
    /// </summary>
    public static ValidationRule<decimal> IsNonNegative(string propertyName, string? errorMessage = null)
    {
        return new ValidationRule<decimal>
        {
            PropertyName = propertyName,
            ErrorMessage = errorMessage ?? $"{propertyName} cannot be negative",
            ValidationDelegate = value => value >= 0
        };
    }

    /// <summary>
    /// Validates that a numeric value is within a range
    /// </summary>
    public static ValidationRule<decimal> IsInRange(decimal min, decimal max, string propertyName, string? errorMessage = null)
    {
        return new ValidationRule<decimal>
        {
            PropertyName = propertyName,
            ErrorMessageDelegate = value => errorMessage ?? $"{propertyName} must be between {min} and {max}",
            ValidationDelegate = value => value >= min && value <= max
        };
    }

    /// <summary>
    /// Validates that an integer is positive
    /// </summary>
    public static ValidationRule<int> IsPositiveInteger(string propertyName, string? errorMessage = null)
    {
        return new ValidationRule<int>
        {
            PropertyName = propertyName,
            ErrorMessage = errorMessage ?? $"{propertyName} must be greater than 0",
            ValidationDelegate = value => value > 0
        };
    }

    /// <summary>
    /// Validates that an integer is non-negative
    /// </summary>
    public static ValidationRule<int> IsNonNegativeInteger(string propertyName, string? errorMessage = null)
    {
        return new ValidationRule<int>
        {
            PropertyName = propertyName,
            ErrorMessage = errorMessage ?? $"{propertyName} cannot be negative",
            ValidationDelegate = value => value >= 0
        };
    }

    /// <summary>
    /// Validates that an integer is within a range
    /// </summary>
    public static ValidationRule<int> IsInIntegerRange(int min, int max, string propertyName, string? errorMessage = null)
    {
        return new ValidationRule<int>
        {
            PropertyName = propertyName,
            ErrorMessageDelegate = value => errorMessage ?? $"{propertyName} must be between {min} and {max}",
            ValidationDelegate = value => value >= min && value <= max
        };
    }

    /// <summary>
    /// Validates that a nullable value has a value
    /// </summary>
    public static ValidationRule<T> IsRequired<T>(string propertyName, string? errorMessage = null) where T : struct
    {
        return new ValidationRule<T>
        {
            PropertyName = propertyName,
            ErrorMessage = errorMessage ?? $"{propertyName} is required",
            ValidationDelegate = value => value.ToString() != ""
        };
    }
}