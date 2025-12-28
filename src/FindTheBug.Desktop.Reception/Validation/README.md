# Receipt Form Validation System

This document describes the validation system implemented for the Reception Desktop app's receipt form.

## Overview

The validation system provides real-time validation for all form fields with visual feedback to guide users. It includes:

- **ValidatableObject<T>**: A wrapper class that encapsulates validation logic for properties
- **ValidationRule<T>**: Defines individual validation rules with custom error messages
- **Validators**: A static class with predefined common validation rules

## Components

### 1. ValidatableObject<T>

The core component that wraps properties requiring validation.

**Properties:**
- `Value`: The actual value of the property
- `Error`: The first validation error message (if any)
- `IsValid`: Boolean indicating if all validations pass
- `IsDirty`: Boolean indicating if the value has been modified
- `Errors`: Collection of all validation error messages

**Methods:**
- `AddValidationRule(rule)`: Adds a validation rule to the property
- `Validate()`: Runs all validation rules and updates error state
- `ForceValidation()`: Forces validation even if field is not dirty
- `ClearErrors()`: Clears all errors and resets validation state
- `ClearValidationRules()`: Removes all validation rules

### 2. ValidationRule<T>

Defines a single validation rule.

**Properties:**
- `PropertyName`: Name of the property being validated
- `ErrorMessage`: Static error message
- `ErrorMessageDelegate`: Dynamic error message function
- `ValidationDelegate`: Function that performs the validation

### 3. Validators

Static helper class with predefined validation rules:

**String Validators:**
- `IsNotEmpty(propertyName, errorMessage?)`: Validates non-empty strings
- `MinLength(length, propertyName, errorMessage?)`: Validates minimum length
- `MaxLength(length, propertyName, errorMessage?)`: Validates maximum length
- `IsValidPhoneNumber(errorMessage?)`: Validates Bangladeshi phone format (11 digits, starts with 01)

**Numeric Validators (Decimal):**
- `IsPositive(propertyName, errorMessage?)`: Validates value > 0
- `IsNonNegative(propertyName, errorMessage?)`: Validates value >= 0
- `IsInRange(min, max, propertyName, errorMessage?)`: Validates value within range

**Integer Validators:**
- `IsPositiveInteger(propertyName, errorMessage?)`: Validates integer > 0
- `IsNonNegativeInteger(propertyName, errorMessage?)`: Validates integer >= 0
- `IsInIntegerRange(min, max, propertyName, errorMessage?)`: Validates integer within range

**Nullable Validators:**
- `IsRequired<T>(propertyName, errorMessage?)`: Validates nullable value has a value

## Implementation in ReceiptFormViewModel

The ReceiptFormViewModel now uses two organized model classes for better structure:

### PatientInformation Model

Located at `Models/PatientInformation.cs`, this class encapsulates all patient-related fields and their validation rules.

**Properties:**
- `PatientName`: Patient's full name
- `PhoneNumber`: Contact phone number
- `Age`: Patient's age in years
- `Gender`: Selected gender
- `Address`: Patient's address
- `ReferredBy`: Doctor who referred the patient
- `ReceiptDate`: Date of receipt creation

**Validation Rules:**

1. **Patient Name**
   - Required
   - Minimum 2 characters
   - Maximum 100 characters

2. **Phone Number**
   - Required
   - Must be 11 digits
   - Must start with "01"
   - All characters must be digits

3. **Age**
   - Must be between 0 and 120

4. **Gender**
   - Required (must select an option)

5. **Address**
   - Required
   - Minimum 5 characters
   - Maximum 500 characters

6. **Referred By**
   - Required (must select a doctor)

7. **Receipt Date**
   - Defaults to today's date
   - No validation rules (optional)

**Methods:**
- `ValidateAll()`: Validates all patient fields
- `ClearAll()`: Clears all patient fields and resets validation
- `ForceValidateAll()`: Forces validation on all fields

### TestInformation Model

Located at `Models/TestInformation.cs`, this class encapsulates test-related fields and their validation rules.

**Properties:**
- `TestName`: Name of the test
- `TestAmount`: Cost of the test
- `TestDiscount`: Discount percentage (0-100)

**Validation Rules:**

1. **Test Name**
   - Required (must select a test)

2. **Test Amount**
   - Must be greater than 0

3. **Test Discount**
   - Must be between 0 and 100 (percentage)
   - Defaults to 0

**Methods:**
- `ValidateAll()`: Validates all test fields
- `ClearAll()`: Clears all test fields and resets validation
- `ForceValidateAll()`: Forces validation on all fields
- `GetTotal()`: Calculates total after discount

### ReceiptFormViewModel Structure

```csharp
public class ReceiptFormViewModel : BaseViewModel
{
    // Organized information models
    public PatientInformation PatientInfo { get; private set; }
    public TestInformation TestInfo { get; private set; }
    
    // Commands
    public ICommand AddTestCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand SaveCommand { get; }
    
    // Test collection
    public ObservableCollection<LabTestDto> Tests { get; set; }
}
```

**Benefits of This Structure:**

1. **Separation of Concerns**: Patient and test information are logically separated
2. **Reusability**: Models can be used in other ViewModels or contexts
3. **Maintainability**: Related validation logic is grouped together
4. **Testability**: Individual models can be unit tested independently
5. **Cleaner ViewModel**: ReceiptFormViewModel is simpler and focuses on coordination

## XAML Integration

### Data Binding

Fields are bound using the `Value` property through their parent model:

```xml
<!-- Patient Information Fields -->
<TextBox Text="{Binding PatientInfo.PatientName.Value, UpdateSourceTrigger=PropertyChanged}"
         Background="{Binding PatientInfo.PatientName.IsValid, Converter={StaticResource ErrorToColorConverter}}"
         BorderBrush="{Binding PatientInfo.PatientName.Error, Converter={StaticResource ErrorToBorderColorConverter}}">

<TextBox Text="{Binding PatientInfo.PhoneNumber.Value, UpdateSourceTrigger=PropertyChanged}"
         Background="{Binding PatientInfo.PhoneNumber.IsValid, Converter={StaticResource ErrorToColorConverter}}"
         BorderBrush="{Binding PatientInfo.PhoneNumber.Error, Converter={StaticResource ErrorToBorderColorConverter}}">

<!-- Test Information Fields -->
<ComboBox SelectedItem="{Binding TestInfo.TestName.Value}">
    <ComboBoxItem Content="X-Ray"></ComboBoxItem>
    <ComboBoxItem Content="MRI Scan"></ComboBoxItem>
</ComboBox>

<TextBox Text="{Binding TestInfo.TestAmount.Value, UpdateSourceTrigger=PropertyChanged}"
         Background="{Binding TestInfo.TestAmount.IsValid, Converter={StaticResource ErrorToColorConverter}}"
         BorderBrush="{Binding TestInfo.TestAmount.Error, Converter={StaticResource ErrorToBorderColorConverter}}">
```

### Visual Feedback

**Background Color:**
- Valid: White
- Invalid: LightPink

**Border Color:**
- Valid: Gray (#ABABAB)
- Invalid: Red

**Error Messages:**
- Displayed in red text below each field
- Uses `StringToVisibilityConverter` to show/hide

```xml
<TextBlock Text="{Binding PatientName.Error}" 
           Foreground="Red" 
           FontSize="10" 
           Margin="2,2,0,0"
           Visibility="{Binding PatientName.Error, Converter={StaticResource StringToVisibilityConverter}}"/>
```

### Converters

Two converters handle visual feedback:

1. **ValidationErrorToColorConverter**: Converts validation state to background color
2. **ValidationErrorToBorderColorConverter**: Converts validation state to border color

## Commands

### AddTestCommand

Validates test fields before adding to the list:
```csharp
private void AddTest(object? parameter)
{
    // Validate test fields before adding
    if (!TestInfo.ValidateAll())
    {
        return;
    }

    Tests.Add(new LabTestDto
    {
        Name = TestInfo.TestName.Value ?? string.Empty,
        Amount = TestInfo.TestAmount.Value,
        Discount = TestInfo.TestDiscount.Value
    });

    OnPropertyChanged(nameof(Tests));
    
    // Reset test fields
    TestInfo.ClearAll();
}
```

### ResetCommand

Clears all form fields and resets validation state:
```csharp
private void ResetForm(object? parameter)
{
    // Reset all information
    PatientInfo.ClearAll();
    TestInfo.ClearAll();
    
    // Clear tests collection
    Tests.Clear();
    
    OnPropertyChanged(nameof(Tests));
}
```

### SaveCommand

Validates all patient fields before saving:
```csharp
private void SaveReceipt(object? parameter)
{
    // Validate all patient information
    if (!PatientInfo.ValidateAll())
    {
        return;
    }

    // Validate that at least one test is added
    if (!Tests.Any())
    {
        // TODO: Show message to user
        return;
    }

    // TODO: Implement save logic
}
```

## Usage Examples

### Adding a New Field to PatientInformation

1. **Add property to PatientInformation.cs:**
```csharp
public class PatientInformation
{
    // ... existing properties
    public ValidatableObject<string> Email { get; private set; }
}
```

2. **Initialize the field in constructor:**
```csharp
private void InitializeEmail()
{
    Email = new ValidatableObject<string>();
    Email.AddValidationRule(Validators.IsNotEmpty("Email"));
    Email.AddValidationRule(new ValidationRule<string>
    {
        PropertyName = "Email",
        ErrorMessage = "Invalid email format",
        ValidationDelegate = value => Regex.IsMatch(value ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$")
    });
}

// Call this in InitializeFields()
```

3. **Add to ClearAll() method:**
```csharp
public void ClearAll()
{
    // ... existing clears
    Email.Value = string.Empty;
    Email.ClearErrors();
}
```

4. **Bind in XAML:**
```xml
<TextBox Text="{Binding PatientInfo.Email.Value, UpdateSourceTrigger=PropertyChanged}"
         Background="{Binding PatientInfo.Email.IsValid, Converter={StaticResource ErrorToColorConverter}}"
         BorderBrush="{Binding PatientInfo.Email.Error, Converter={StaticResource ErrorToBorderColorConverter}}">
</TextBox>
<TextBlock Text="{Binding PatientInfo.Email.Error}" 
           Foreground="Red" 
           FontSize="10" 
           Margin="2,2,0,0"
           Visibility="{Binding PatientInfo.Email.Error, Converter={StaticResource StringToVisibilityConverter}}"/>
```

### Creating a New Information Model

1. **Create the model class:**
```csharp
public class BillingInformation
{
    public ValidatableObject<decimal> Subtotal { get; private set; }
    public ValidatableObject<decimal> Discount { get; private set; }
    public ValidatableObject<decimal> Total { get; private set; }

    public BillingInformation()
    {
        InitializeFields();
    }

    private void InitializeFields()
    {
        Subtotal = new ValidatableObject<decimal>();
        Subtotal.AddValidationRule(Validators.IsNonNegative("Subtotal"));

        Discount = new ValidatableObject<decimal>();
        Discount.AddValidationRule(Validators.IsInRange(0, 100, "Discount"));

        Total = new ValidatableObject<decimal>();
        Total.AddValidationRule(Validators.IsPositive("Total"));
    }

    public bool ValidateAll()
    {
        return Subtotal.Validate() && Discount.Validate() && Total.Validate();
    }

    public void ClearAll()
    {
        Subtotal.Value = 0;
        Subtotal.ClearErrors();
        Discount.Value = 0;
        Discount.ClearErrors();
        Total.Value = 0;
        Total.ClearErrors();
    }

    public void ForceValidateAll()
    {
        Subtotal.ForceValidation();
        Discount.ForceValidation();
        Total.ForceValidation();
    }
}
```

2. **Add to ViewModel:**
```csharp
public BillingInformation BillingInfo { get; private set; }

public ReceiptFormViewModel(Action<object?>? onLogout = null, Action<object?>? onSyncData = null)
{
    // ... existing initialization
    BillingInfo = new BillingInformation();
}
```

3. **Use in XAML:**
```xml
<TextBox Text="{Binding BillingInfo.Subtotal.Value, UpdateSourceTrigger=PropertyChanged}"
         Background="{Binding BillingInfo.Subtotal.IsValid, Converter={StaticResource ErrorToColorConverter}}"
         BorderBrush="{Binding BillingInfo.Subtotal.Error, Converter={StaticResource ErrorToBorderColorConverter}}">
</TextBox>
```

### Creating Custom Validation Rules

```csharp
var customRule = new ValidationRule<string>
{
    PropertyName = "CustomField",
    ErrorMessageDelegate = value => $"Custom validation failed for: {value}",
    ValidationDelegate = value => 
    {
        // Your validation logic here
        return value != null && value.StartsWith("PREFIX");
    }
};

yourValidatableObject.AddValidationRule(customRule);
```

## Best Practices

1. **Use `UpdateSourceTrigger=PropertyChanged`** for real-time validation feedback
2. **Call `Validate()`** before performing actions (save, add test, etc.)
3. **Use `ForceValidation()`** when validating on form submission (shows all errors)
4. **Clear errors** after successful operations to reset form state
5. **Provide clear error messages** that tell users exactly what's wrong
6. **Test all validation scenarios** including edge cases

## Future Enhancements

Potential improvements:

1. Add email validation to Validators class
2. Implement multi-field validation (e.g., start date < end date)
3. Add async validation (e.g., check if phone number already exists)
4. Create validation summary component
5. Add support for cross-property validation
6. Implement validation groups for complex forms
7. Add localization support for error messages

## Troubleshooting

**Validation not firing:**
- Ensure `UpdateSourceTrigger=PropertyChanged` is set in binding
- Check that validation rules are added in initialization methods
- Verify that `Value` property is being set, not the object itself

**Error messages not showing:**
- Check that `StringToVisibilityConverter` is properly registered
- Ensure the Error property has a value (not empty string)
- Verify that the TextBlock is properly bound to the Error property

**Colors not changing:**
- Check that converters are registered in UserControl.Resources
- Ensure IsValid/Error properties are being updated correctly
- Verify converter implementation handles the binding type correctly