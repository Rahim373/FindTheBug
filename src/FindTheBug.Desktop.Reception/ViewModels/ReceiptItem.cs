namespace FindTheBug.Desktop.Reception.ViewModels
{
    public class ReceiptItem
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime BilledOn { get; set; }
        public string Amount { get; set; } = string.Empty;
        public string ReceiptStatus { get; set; } = string.Empty;
        public string ReportStatus { get; set; } = string.Empty;
        public string PhoneNumber { get; internal set; }
        public decimal Due { get; internal set; }
        public string GenderAge { get; internal set; }
        public decimal Discount { get; internal set; }
        public int NumberOfTotalTest { get; internal set; }
    }
}