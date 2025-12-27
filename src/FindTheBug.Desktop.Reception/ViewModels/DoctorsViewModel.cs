using FindTheBug.Desktop.Reception.Commands;
using FindTheBug.Desktop.Reception.DataAccess;
using FindTheBug.Desktop.Reception.Dtos;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FindTheBug.Desktop.Reception.ViewModels;

public class DoctorsViewModel : BaseViewModel
{
    public DoctorsViewModel()
    {
        FirstPageCommand = new RelayCommand(_ => FirstPage());
        PreviousPageCommand = new RelayCommand(_ => PreviousPage(), _ => CanGoToPreviousPage);
        NextPageCommand = new RelayCommand(_ => NextPage(), _ => CanGoToNextPage);
        LastPageCommand = new RelayCommand(_ => LastPage(), _ => CanGoToNextPage);
        ClearFiltersCommand = new RelayCommand(_ => ClearFilters());

        // Initialize with sample data
        _ = LoadSampleDoctors();
        ApplyFilters();
    }

    public ObservableCollection<DoctorItem> Doctors { get; } = new ObservableCollection<DoctorItem>();

    private async Task LoadSampleDoctors()
    {
        var doctors = await DbAccess.GetAllDoctorsAsync();
        
        foreach (var item in doctors)
        {
            Doctors.Add(new DoctorItem
            {
                Id = item.Id.ToString(),
                Degree = item.Degree,
                Name = item.Name,
                Phone = item.PhoneNumber,
                Speciality = string.Join(", ", item.DoctorSpecialities.Select(x => x.DoctorSpeciality.Name)),
            });
        }
    }

    private string _nameFilter = string.Empty;
    private string _specialityFilter = string.Empty;
    private string _phoneFilter = string.Empty;
    private string _statusFilter = "All";
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalItems = 0;
    private int _totalPages = 0;

    public string NameFilter
    {
        get => _nameFilter;
        set
        {
            if (SetProperty(ref _nameFilter, value))
            {
                ApplyFilters();
            }
        }
    }

    public string SpecialityFilter
    {
        get => _specialityFilter;
        set
        {
            if (SetProperty(ref _specialityFilter, value))
            {
                ApplyFilters();
            }
        }
    }

    public string PhoneFilter
    {
        get => _phoneFilter;
        set
        {
            if (SetProperty(ref _phoneFilter, value))
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

    private void ApplyFilters()
    {
        var filtered = DbAccess.GetAllDoctorsAsync().GetAwaiter().GetResult().AsEnumerable();

        // Apply specific filters
        if (!string.IsNullOrWhiteSpace(NameFilter))
        {
            filtered = filtered.Where(r => r.Name.ToLower().Contains(NameFilter.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(SpecialityFilter))
        {
            filtered = filtered.Where(r =>
                r.DoctorSpecialities.Any(x => x.DoctorSpeciality.Name.ToLower().Contains(SpecialityFilter.ToLower())));
        }

        if (!string.IsNullOrWhiteSpace(PhoneFilter))
        {
            filtered = filtered.Where(r => r.PhoneNumber.ToLower().Contains(PhoneFilter.ToLower()));
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

        Doctors.Clear();
        foreach (var item in pagedData)
        {
            Doctors.Add(new DoctorItem
            {
                Id = item.Id.ToString(),
                Degree = item.Degree,
                Name = item.Name,
                Phone = item.PhoneNumber,
                Speciality = string.Join(", ", item.DoctorSpecialities.Select(x => x.DoctorSpeciality.Name)),
            });
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
        NameFilter = string.Empty;
        SpecialityFilter = string.Empty;
        PhoneFilter = string.Empty;
        CurrentPage = 1;
    }
}