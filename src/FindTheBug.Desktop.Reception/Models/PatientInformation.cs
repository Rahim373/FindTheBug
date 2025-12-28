using FindTheBug.Desktop.Reception.Validation;

namespace FindTheBug.Desktop.Reception.Models;

/// <summary>
/// Represents patient information with validation
/// </summary>
public class PatientInformation
{
    public ValidatableObject<string> PatientName { get; private set; }
    public ValidatableObject<string> PhoneNumber { get; private set; }
    public ValidatableObject<string> Age { get; private set; }
    public ValidatableObject<string> Gender { get; private set; }
    public ValidatableObject<string> Address { get; private set; }
    public ValidatableObject<string> ReferredBy { get; private set; }
    public ValidatableObject<DateTime> ReceiptDate { get; private set; }
    public string? InvoiceNumber { get; internal set; }

    public PatientInformation()
    {
        InitializeFields();
    }

    private void InitializeFields()
    {
        InitializePatientName();
        InitializePhoneNumber();
        InitializeAge();
        InitializeGender();
        InitializeAddress();
        InitializeReferredBy();
        InitializeReceiptDate();
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
        Age = new ValidatableObject<string>();
        Age.AddValidationRule(Validators.IsNotEmpty("Age", "Please enter a valid age (20y)"));
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

        Age.Value = string.Empty;
        Age.ClearErrors();

        Gender.Value = string.Empty;
        Gender.ClearErrors();

        Address.Value = string.Empty;
        Address.ClearErrors();

        ReferredBy.Value = string.Empty;
        ReferredBy.ClearErrors();

        ReceiptDate.Value = DateTime.Today;
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