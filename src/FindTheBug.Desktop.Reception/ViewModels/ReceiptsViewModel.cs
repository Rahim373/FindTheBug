using FindTheBug.Desktop.Reception.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FindTheBug.Desktop.Reception.ViewModels
{
    public class ReceiptsViewModel : BaseViewModel
    {
        private string _searchText = string.Empty;
        private string _receiptNumberFilter = string.Empty;
        private string _patientNameFilter = string.Empty;
        private DateTime? _startDateFilter;
        private DateTime? _endDateFilter;
        private string _statusFilter = "All";
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalItems = 0;
        private int _totalPages = 0;

        private readonly List<ReceiptItem> _allReceipts = new List<ReceiptItem>();

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string ReceiptNumberFilter
        {
            get => _receiptNumberFilter;
            set
            {
                if (SetProperty(ref _receiptNumberFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string PatientNameFilter
        {
            get => _patientNameFilter;
            set
            {
                if (SetProperty(ref _patientNameFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        public DateTime? StartDateFilter
        {
            get => _startDateFilter;
            set
            {
                if (SetProperty(ref _startDateFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        public DateTime? EndDateFilter
        {
            get => _endDateFilter;
            set
            {
                if (SetProperty(ref _endDateFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string StatusFilter
        {
            get => _statusFilter;
            set
            {
                if (SetProperty(ref _statusFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    ApplyFilters();
                }
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (SetProperty(ref _pageSize, value) && _pageSize > 0)
                {
                    CurrentPage = 1;
                    ApplyFilters();
                }
            }
        }

        public int TotalItems
        {
            get => _totalItems;
            private set => SetProperty(ref _totalItems, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            private set => SetProperty(ref _totalPages, value);
        }

        public ObservableCollection<ReceiptItem> Receipts { get; } = new ObservableCollection<ReceiptItem>();

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

        // Commands
        public ICommand FirstPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand LastPageCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public ReceiptsViewModel()
        {
            // Initialize commands
            FirstPageCommand = new RelayCommand(_ => FirstPage());
            PreviousPageCommand = new RelayCommand(_ => PreviousPage(), _ => CanGoToPreviousPage);
            NextPageCommand = new RelayCommand(_ => NextPage(), _ => CanGoToNextPage);
            LastPageCommand = new RelayCommand(_ => LastPage(), _ => CanGoToNextPage);
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());

            // Initialize with sample data
            LoadSampleReceipts();
            ApplyFilters();
        }

        private void LoadSampleReceipts()
        {
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1001",
                PatientName = "John Doe",
                Date = DateTime.Today,
                Amount = "$500.00",
                Status = "Paid"
            });

            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1002",
                PatientName = "Jane Smith",
                Date = DateTime.Today.AddDays(-1),
                Amount = "$750.00",
                Status = "Paid"
            });

            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1003",
                PatientName = "Bob Johnson",
                Date = DateTime.Today.AddDays(-2),
                Amount = "$300.00",
                Status = "Pending"
            });

            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1004",
                PatientName = "Alice Williams",
                Date = DateTime.Today.AddDays(-3),
                Amount = "$450.00",
                Status = "Paid"
            });

            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1005",
                PatientName = "Charlie Brown",
                Date = DateTime.Today.AddDays(-4),
                Amount = "$600.00",
                Status = "Cancelled"
            });

            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1006",
                PatientName = "Diana Prince",
                Date = DateTime.Today.AddDays(-5),
                Amount = "$800.00",
                Status = "Paid"
            });

            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1007",
                PatientName = "Edward Norton",
                Date = DateTime.Today.AddDays(-6),
                Amount = "$350.00",
                Status = "Pending"
            });

            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1008",
                PatientName = "Fiona Green",
                Date = DateTime.Today.AddDays(-7),
                Amount = "$900.00",
                Status = "Paid"
            });

            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1009",
                PatientName = "George Hill",
                Date = DateTime.Today.AddDays(-8),
                Amount = "$550.00",
                Status = "Pending"
            });

            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1010",
                PatientName = "Helen White",
                Date = DateTime.Today.AddDays(-9),
                Amount = "$400.00",
                Status = "Paid"
            });

            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1011",
                PatientName = "Ian Black",
                Date = DateTime.Today.AddDays(-10),
                Amount = "$700.00",
                Status = "Paid"
            });

            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
            _allReceipts.Add(new ReceiptItem
            {
                ReceiptNumber = "RCT-20251226-1012",
                PatientName = "Julia Red",
                Date = DateTime.Today.AddDays(-11),
                Amount = "$650.00",
                Status = "Cancelled"
            });
        }

        private void ApplyFilters()
        {
            var filtered = _allReceipts.AsEnumerable();

            // Apply general search
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchTerm = SearchText.ToLower();
                filtered = filtered.Where(r =>
                    r.ReceiptNumber.ToLower().Contains(searchTerm) ||
                    r.PatientName.ToLower().Contains(searchTerm) ||
                    r.Amount.ToLower().Contains(searchTerm) ||
                    r.Status.ToLower().Contains(searchTerm));
            }

            // Apply specific filters
            if (!string.IsNullOrWhiteSpace(ReceiptNumberFilter))
            {
                filtered = filtered.Where(r =>
                    r.ReceiptNumber.ToLower().Contains(ReceiptNumberFilter.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(PatientNameFilter))
            {
                filtered = filtered.Where(r =>
                    r.PatientName.ToLower().Contains(PatientNameFilter.ToLower()));
            }

            if (StartDateFilter.HasValue)
            {
                filtered = filtered.Where(r => r.Date >= StartDateFilter.Value);
            }

            if (EndDateFilter.HasValue)
            {
                filtered = filtered.Where(r => r.Date <= EndDateFilter.Value);
            }

            if (StatusFilter != "All")
            {
                filtered = filtered.Where(r => r.Status == StatusFilter);
            }

            // Update totals
            TotalItems = filtered.Count();
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
            var pagedData = filtered
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            Receipts.Clear();
            foreach (var item in pagedData)
            {
                Receipts.Add(item);
            }

            // Notify page navigation properties
            OnPropertyChanged(nameof(CanGoToPreviousPage));
            OnPropertyChanged(nameof(CanGoToNextPage));
            OnPropertyChanged(nameof(PageInfo));
        }

        public void FirstPage()
        {
            CurrentPage = 1;
        }

        public void PreviousPage()
        {
            if (CanGoToPreviousPage)
            {
                CurrentPage--;
            }
        }

        public void NextPage()
        {
            if (CanGoToNextPage)
            {
                CurrentPage++;
            }
        }

        public void LastPage()
        {
            if (TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }
        }

        public void ClearFilters()
        {
            SearchText = string.Empty;
            ReceiptNumberFilter = string.Empty;
            PatientNameFilter = string.Empty;
            StartDateFilter = null;
            EndDateFilter = null;
            StatusFilter = "All";
            CurrentPage = 1;
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