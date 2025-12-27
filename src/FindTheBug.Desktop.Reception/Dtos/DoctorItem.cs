namespace FindTheBug.Desktop.Reception.Dtos;

public class DoctorItem
{
    public string Degree { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Speciality { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Id { get; internal set; }
}