using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class Expense : BaseAuditableEntity
{
    public DateTime Date { get; set; }
    public string Note { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // Cash/Cheque/bKash/Nagad
    public string? ReferenceNo { get; set; }
    public string? Attachment { get; set; }
    public string Department { get; set; } = string.Empty; // Lab/Dispensary
}