using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FindTheBug.Desktop.Reception.Commands;
using FindTheBug.Desktop.Reception.DataAccess;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace FindTheBug.Desktop.Reception.ViewModels
{
    public partial class ReceiptsViewModel : ObservableObject
    {
        #region Observable Properties

        [ObservableProperty]
        private string _searchText = string.Empty;
        [ObservableProperty]
        private string _receiptNumberFilter = string.Empty;
        [ObservableProperty]
        private string _patientNameFilter = string.Empty;
        [ObservableProperty]
        private DateTime? _startDateFilter;
        [ObservableProperty]
        private DateTime? _endDateFilter;
        [ObservableProperty]
        private LabReceiptStatus? _statusFilter = null;
        [ObservableProperty]
        private int _currentPage = 1;
        [ObservableProperty]
        private int _pageSize = 10;
        [ObservableProperty]
        private int _totalItems = 0;
        [ObservableProperty]
        private int _totalPages = 0; 
        public ObservableCollection<ReceiptItem> Receipts { get; } = new ObservableCollection<ReceiptItem>();

        #endregion

        public Task InitializationTask { get; private set; }

        #region Change Events

        public async Task OnSearchTextChanged() => await ApplyFiltersAsync();
        public async Task OnReceiptNumberFilterChanged() => await ApplyFiltersAsync();
        public async Task OnPatientNameFilterChanged() => await ApplyFiltersAsync();
        public async Task OnStartDateFilterChanged() => await ApplyFiltersAsync();
        public async Task OnEndDateFilterChanged() => await ApplyFiltersAsync();
        public async Task OnStatusFilterChanged() => await ApplyFiltersAsync();
        public async Task OnCurrentPageChanged() => await ApplyFiltersAsync();
        public async Task OnPageSizeChanged() => await ApplyFiltersAsync(); 
        
        #endregion


        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string>
        {
            "All", "Paid", "Pending", "Cancelled"
        };

        public ObservableCollection<int> PageSizeOptions { get; } = new ObservableCollection<int>
        {
            5, 10, 20, 50, 100
        };

        public bool CanGoToPreviousPage => CurrentPage > 1;
        public bool CanGoToNextPage => CurrentPage < TotalPages;
        public string PageInfo => $"Page {CurrentPage} of {TotalPages} ({TotalItems} items)";

        public ReceiptsViewModel()
        {
            InitializationTask = ApplyFiltersAsync();
        }

        private async Task ApplyFiltersAsync()
        {
            var query = DbAccess.GetAllReceiptsQuery();


            // Apply specific filters
            if (!string.IsNullOrWhiteSpace(ReceiptNumberFilter))
            {
                query = query.Where(r =>
                    r.InvoiceNumber.ToLower().Contains(ReceiptNumberFilter.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(PatientNameFilter))
            {
                query = query.Where(r =>
                    r.FullName.ToLower().Contains(PatientNameFilter.ToLower()));
            }

            if (StartDateFilter.HasValue)
            {
                query = query.Where(r => r.CreatedAt >= StartDateFilter.Value);
            }

            if (EndDateFilter.HasValue)
            {
                query = query.Where(r => r.CreatedAt <= EndDateFilter.Value);
            }

            if (StatusFilter.HasValue)
            {
                query = query.Where(r => r.LabReceiptStatus == StatusFilter);
            }

            // Update totals
            TotalItems = query.Count();
            TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);

            // Ensure current page is valid
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            // Apply pagination
            var pagedData = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(x => new ReceiptItem
                {
                    Amount = x.Total.ToString(),
                    BilledOn = x.CreatedAt,
                    FullName = x.FullName,
                    InvoiceNumber = x.InvoiceNumber,
                    ReceiptStatus = x.LabReceiptStatus.ToString(),
                    ReportStatus = x.ReportDeliveryStatus == ReportDeliveryStatus.Delivered ? "Yes" : "No",
                    PhoneNumber = x.PhoneNumber,
                    Due = x.Balace,
                    GenderAge = x.Gender + "/" + (x.Age.HasValue ? x.Age.Value.ToString() + (x.IsAgeYear ? "Y" : "M") : ""),
                    NumberOfTotalTest = x.TestEntries.Count,
                    Discount = x.Discount
                })
                .ToListAsync();

            Receipts.Clear();
            pagedData.ForEach(Receipts.Add);

            // Notify page navigation properties
            OnPropertyChanged(nameof(CanGoToPreviousPage));
            OnPropertyChanged(nameof(CanGoToNextPage));
            OnPropertyChanged(nameof(PageInfo));
        }

        #region Relay Commands

        [RelayCommand]
        public void FirstPage()
        {
            CurrentPage = 1;
        }

        [RelayCommand]
        public void PreviousPage()
        {
            if (CanGoToPreviousPage)
            {
                CurrentPage--;
            }
        }

        [RelayCommand]
        public void NextPage()
        {
            if (CanGoToNextPage)
            {
                CurrentPage++;
            }
        }

        [RelayCommand]
        public void LastPage()
        {
            if (TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }
        }

        [RelayCommand]
        public void ClearFilters()
        {
            SearchText = string.Empty;
            ReceiptNumberFilter = string.Empty;
            PatientNameFilter = string.Empty;
            StartDateFilter = null;
            EndDateFilter = null;
            StatusFilter = null;
            CurrentPage = 1;
        }

        #endregion
    }
}