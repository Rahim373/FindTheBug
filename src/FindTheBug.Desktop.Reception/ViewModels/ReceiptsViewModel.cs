using System.Collections.ObjectModel;

namespace FindTheBug.Desktop.Reception.ViewModels
{
    public class ReceiptsViewModel : BaseViewModel
    {
        private string _searchText = string.Empty;

        public ReceiptsViewModel()
        {
            // Initialize with sample data
            LoadSampleReceipts();
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    // Filter receipts based on search
                }
            }
        }

        public ObservableCollection<ReceiptItem> Receipts { get; } = new ObservableCollection<ReceiptItem>();

        private void LoadSampleReceipts()
        {
            Receipts.Add(new ReceiptItem 
            { 
                ReceiptNumber = "RCT-20251226-1001", 
                PatientName = "John Doe", 
                Date = DateTime.Today, 
                Amount = "$500.00", 
                Status = "Paid" 
            });
            
            Receipts.Add(new ReceiptItem 
            { 
                ReceiptNumber = "RCT-20251226-1002", 
                PatientName = "Jane Smith", 
                Date = DateTime.Today.AddDays(-1), 
                Amount = "$750.00", 
                Status = "Paid" 
            });
            
            Receipts.Add(new ReceiptItem 
            { 
                ReceiptNumber = "RCT-20251226-1003", 
                PatientName = "Bob Johnson", 
                Date = DateTime.Today.AddDays(-2), 
                Amount = "$300.00", 
                Status = "Pending" 
            });
        }
    }

    public class ReceiptItem
    {
        public string ReceiptNumber { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Amount { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}