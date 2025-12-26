using FindTheBug.Desktop.Reception.DataAccess;
using System.Collections.ObjectModel;

namespace FindTheBug.Desktop.Reception.ViewModels
{
    public class DoctorsViewModel : BaseViewModel
    {
        private string _searchText = string.Empty;

        public DoctorsViewModel()
        {
            // Initialize with sample data
            LoadSampleDoctors();
        }

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

        public ObservableCollection<DoctorItem> Doctors { get; } = new ObservableCollection<DoctorItem>();

        private async Task LoadSampleDoctors()
        {
            var doctors = await DbAccess.GetAllDoctorsAsync();
            
            foreach (var item in doctors)
            {
                Doctors.Add(new DoctorItem
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    Phone = item.PhoneNumber,
                    Speciality = string.Join(", ", item.DoctorSpecialities.Select(x => x.DoctorSpeciality.Name)),
                });
            }
        }
    }

    public class DoctorItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Speciality { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}