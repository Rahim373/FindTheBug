using FindTheBug.Desktop.Reception.Validation;

namespace FindTheBug.Desktop.Reception.Models;

/// <summary>
/// Represents test information with validation
/// </summary>
public class TestInformation
{
    public ValidatableObject<Guid> Id { get; private set; }
    public ValidatableObject<decimal> TestAmount { get; private set; }
    public ValidatableObject<decimal> TestDiscount { get; private set; }
    public ValidatableObject<string> TestName { get; private set; }

    public TestInformation()
    {
        InitializeFields();
    }

    private void InitializeFields()
    {
        InitializeId();
        InitializeTestName();
        InitializeTestAmount();
        InitializeTestDiscount();
    }

    private void InitializeId()
    {
        Id = new ValidatableObject<Guid>(); 
        Id.AddValidationRule(Validators.IsGuidNotEmpty("Test", "Please select a test"));
    }

    private void InitializeTestName() {
        TestName = new ValidatableObject<string>();
    }

    private void InitializeTestAmount()
    {
        TestAmount = new ValidatableObject<decimal>();
        TestAmount.AddValidationRule(Validators.IsPositive("Amount", "Test amount must be greater than 0"));
    }

    private void InitializeTestDiscount()
    {
        TestDiscount = new ValidatableObject<decimal>();
        TestDiscount.Value = 0;
        TestDiscount.AddValidationRule(Validators.IsInRange(0, 100, "Discount", "Discount must be between 0 and 100"));
    }

    public bool ValidateAll()
    {
        bool isValid = true;
        isValid &= Id.Validate();
        isValid &= TestAmount.Validate();
        isValid &= TestDiscount.Validate();
        return isValid;
    }

    public void ClearAll()
    {
        Id.Value = Guid.Empty;
        Id.ClearErrors();

        TestName.Value = string.Empty;

        TestAmount.Value = 0;
        TestAmount.ClearErrors();

        TestDiscount.Value = 0;
    }

    public void ForceValidateAll()
    {
        Id.ForceValidation();
        TestAmount.ForceValidation();
        TestDiscount.ForceValidation();
    }

    /// <summary>
    /// Gets the calculated total after discount
    /// </summary>
    public decimal GetTotal()
    {
        if (TestAmount.Value == 0) return 0;
        
        var discount = TestAmount.Value * (TestDiscount.Value / 100);
        return TestAmount.Value - discount;
    }
}