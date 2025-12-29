using FindTheBug.Desktop.Reception.Validation;

namespace FindTheBug.Desktop.Reception.Models;

/// <summary>
/// Represents patient information with validation
/// </summary>
public class ReceiptInformation
{
    public ValidatableObject<string> PatientName { get; private set; }
    public ValidatableObject<string> PhoneNumber { get; private set; }
    public ValidatableObject<int> Age { get; private set; }
    public ValidatableObject<bool> IsAgeYear { get; private set; }
    public ValidatableObject<string> Gender { get; private set; }
    public ValidatableObject<string> Address { get; private set; }
    public ValidatableObject<string> ReferredBy { get; private set; }
    public ValidatableObject<DateTime> ReceiptDate { get; private set; }
    public string? InvoiceNumber { get; internal set; }

    // Financial properties
    public ValidatableObject<decimal> SubTotal { get; private set; }
    public ValidatableObject<decimal> Discount { get; private set; }
    public ValidatableObject<decimal> Total { get; private set; }
    public ValidatableObject<decimal> Due { get; private set; }
    public ValidatableObject<decimal> Balance { get; private set; }

    public ReceiptInformation()
    {
        InitializeFields();
    }

    private void InitializeFields()
    {
        InitializePatientName();
        InitializePhoneNumber();
        InitializeAge();
        InitializeIsAgeYear();
        InitializeGender();
        InitializeAddress();
        InitializeReferredBy();
        InitializeReceiptDate();
        InitializeFinancialProperties();
    }

    private void InitializeIsAgeYear()
    {
        IsAgeYear = new ValidatableObject<bool>();
    }

    private void InitializePatientName()
    {
        PatientName = new ValidatableObject<string>();
        PatientName.AddValidationRule(Validators.IsNotEmpty("Patient Name"));
        PatientName.AddValidationRule(Validators.MinLength(2, "Patient Name", "Patient name must be at least 2 characters"));
        PatientName.AddValidationRule(Validators.MaxLength(100, "Patient Name"));
    }

    private void InitializePhoneNumber()
    {
        PhoneNumber = new ValidatableObject<string>();
        PhoneNumber.AddValidationRule(Validators.IsNotEmpty("Phone Number"));
        PhoneNumber.AddValidationRule(Validators.IsValidPhoneNumber());
    }

    private void InitializeAge()
    {
        Age = new ValidatableObject<int>();
        Age.AddValidationRule(Validators.IsPositiveInteger("Age", "Please enter a valid age"));
    }

    private void InitializeGender()
    {
        Gender = new ValidatableObject<string>();
        Gender.AddValidationRule(Validators.IsNotEmpty("Gender", "Please select a gender"));
    }

    private void InitializeAddress()
    {
        Address = new ValidatableObject<string>();
        Address.AddValidationRule(Validators.MaxLength(500, "Address"));
    }

    private void InitializeReferredBy()
    {
        ReferredBy = new ValidatableObject<string>();
        ReferredBy.AddValidationRule(Validators.IsNotEmpty("Referred By", "Please select a doctor who referred the patient"));
    }

    private void InitializeReceiptDate()
    {
        ReceiptDate = new ValidatableObject<DateTime>();
        ReceiptDate.Value = DateTime.UtcNow;
    }

    private void InitializeFinancialProperties()
    {
        SubTotal = new ValidatableObject<decimal>();
        Discount = new ValidatableObject<decimal>();
        Total = new ValidatableObject<decimal>();
        Due = new ValidatableObject<decimal>();
        Balance = new ValidatableObject<decimal>();
    }

    public bool ValidateAll()
    {
        bool isValid = true;
        isValid &= PatientName.Validate();
        isValid &= PhoneNumber.Validate();
        isValid &= Age.Validate();
        isValid &= Gender.Validate();
        isValid &= Address.Validate();
        isValid &= ReferredBy.Validate();
        return isValid;
    }

    public void ClearAll()
    {
        InvoiceNumber = string.Empty;

        PatientName.Value = string.Empty;
        PatientName.ClearErrors();

        PhoneNumber.Value = string.Empty;
        PhoneNumber.ClearErrors();

        Age.Value = 0;
        Age.ClearErrors();

        IsAgeYear.Value = true;

        Gender.Value = string.Empty;
        Gender.ClearErrors();

        Address.Value = string.Empty;
        Address.ClearErrors();

        ReferredBy.Value = string.Empty;
        ReferredBy.ClearErrors();

        ReceiptDate.Value = DateTime.Today;

        // Clear financial properties
        SubTotal.Value = 0;
        Discount.Value = 0;
        Total.Value = 0;
        Due.Value = 0;
        Balance.Value = 0;
    }

    public void ForceValidateAll()
    {
        PatientName.ForceValidation();
        PhoneNumber.ForceValidation();
        Age.ForceValidation();
        Gender.ForceValidation();
        Address.ForceValidation();
        ReferredBy.ForceValidation();
    }

    internal bool HasValue() => !string.IsNullOrEmpty(InvoiceNumber) || PatientName.IsDirty || PhoneNumber.IsDirty
            || Age.IsDirty || Gender.IsDirty
            || Address.IsDirty || ReferredBy.IsDirty;
}