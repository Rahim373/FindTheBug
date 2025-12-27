using FindTheBug.Desktop.Reception.DataAccess;
using FindTheBug.Desktop.Reception.Dtos;
using System.Collections.ObjectModel;

namespace FindTheBug.Desktop.Reception.ViewModels;

public class DoctorsViewModel : BaseViewModel
{
    private string _searchText = string.Empty;

    public DoctorsViewModel()
    {
        using var _ = LoadSampleDoctors();
    }

    public ObservableCollection<DoctorItem> Doctors { get; } = new ObservableCollection<DoctorItem>();

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                // Filter doctors based on search
            }
        }
    }


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
}