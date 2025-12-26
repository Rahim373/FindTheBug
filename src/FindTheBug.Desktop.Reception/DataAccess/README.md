# DataAccess Documentation

This document provides usage examples for the `DbAccess` class, which provides static methods for accessing and manipulating database data in the FindTheBug.Desktop.Reception application.

## Overview

The `DbAccess` class provides a simple, static API for database operations. It automatically manages DbContext lifecycle using the application's dependency injection container.

## Doctors

### Get All Doctors

Retrieves all active doctors with their specialties, ordered by name.

```csharp
var doctors = await DbAccess.GetAllDoctorsAsync();
```

**Returns**: `List<Doctor>` - List of all active doctors with specialties loaded

**Example**:
```csharp
private async Task LoadDoctorsAsync()
{
    var doctors = await DbAccess.GetAllDoctorsAsync();
    
    foreach (var doctor in doctors)
    {
        Console.WriteLine($"Doctor: {doctor.Name}");
        Console.WriteLine($"  Phone: {doctor.PhoneNumber}");
        Console.WriteLine($"  Specialties: {string.Join(", ", doctor.DoctorSpecialities.Select(ds => ds.DoctorSpeciality?.Name))}");
    }
}
```

### Get Doctor by ID

Retrieves a single doctor by their ID with specialties loaded.

```csharp
var doctor = await DbAccess.GetDoctorByIdAsync(doctorId);
```

**Parameters**:
- `doctorId` (Guid) - The ID of the doctor to retrieve

**Returns**: `Doctor?` - The doctor with specified ID, or null if not found

**Example**:
```csharp
private async Task LoadDoctorAsync(Guid id)
{
    var doctor = await DbAccess.GetDoctorByIdAsync(id);
    
    if (doctor != null)
    {
        // Access doctor properties
        var name = doctor.Name;
        var phone = doctor.PhoneNumber;
    }
}
```

### Search Doctors

Searches for doctors by name, phone number, or degree.

```csharp
var doctors = await DbAccess.SearchDoctorsAsync(searchTerm);
```

**Parameters**:
- `searchTerm` (string) - The search term to match against doctor name, phone, or degree

**Returns**: `List<Doctor>` - List of matching doctors

**Example**:
```csharp
private async Task SearchDoctorsAsync()
{
    // Search by name
    var byName = await DbAccess.SearchDoctorsAsync("Dr. Smith");
    
    // Search by phone
    var byPhone = await DbAccess.SearchDoctorsAsync("123-456");
    
    // Search by degree
    var byDegree = await DbAccess.SearchDoctorsAsync("Cardiology");
}
```

### Get Doctors by Specialty

Retrieves all doctors that have a specific specialty.

```csharp
var doctors = await DbAccess.GetDoctorsBySpecialtyAsync(specialtyId);
```

**Parameters**:
- `specialtyId` (Guid) - The ID of the specialty

**Returns**: `List<Doctor>` - List of doctors with the specified specialty

**Example**:
```csharp
private async Task LoadDoctorsBySpecialtyAsync(Guid specialtyId)
{
    var cardiologists = await DbAccess.GetDoctorsBySpecialtyAsync(specialtyId);
    Console.WriteLine($"Found {cardiologists.Count} cardiologists");
}
```

### Add Doctor

Adds a new doctor to the database.

```csharp
var doctorId = await DbAccess.AddDoctorAsync(doctor);
```

**Parameters**:
- `doctor` (Doctor) - The doctor object to add

**Returns**: `Guid` - The ID of the newly created doctor

**Example**:
```csharp
private async Task AddNewDoctorAsync()
{
    var newDoctor = new Doctor
    {
        Name = "Dr. John Smith",
        PhoneNumber = "+1-555-1234",
        Degree = "MBBS, MD",
        Office = "City Hospital",
        IsActive = true
    };
    
    var doctorId = await DbAccess.AddDoctorAsync(newDoctor);
    Console.WriteLine($"Added doctor with ID: {doctorId}");
}
```

### Update Doctor

Updates an existing doctor in the database.

```csharp
var success = await DbAccess.UpdateDoctorAsync(doctor);
```

**Parameters**:
- `doctor` (Doctor) - The doctor object with updated values

**Returns**: `bool` - True if update succeeded, false if doctor not found

**Example**:
```csharp
private async Task UpdateDoctorAsync(Doctor doctor)
{
    doctor.PhoneNumber = "+1-555-9999";
    doctor.Office = "New Medical Center";
    
    var success = await DbAccess.UpdateDoctorAsync(doctor);
    
    if (success)
    {
        Console.WriteLine("Doctor updated successfully");
    }
    else
    {
        Console.WriteLine("Doctor not found");
    }
}
```

### Delete Doctor

Soft deletes a doctor by setting IsActive to false (doesn't permanently remove from database).

```csharp
var success = await DbAccess.DeleteDoctorAsync(doctorId);
```

**Parameters**:
- `doctorId` (Guid) - The ID of the doctor to delete

**Returns**: `bool` - True if deletion succeeded, false if doctor not found

**Example**:
```csharp
private async Task DeleteDoctorAsync(Guid doctorId)
{
    var result = await MessageBox.Show(
        "Are you sure you want to delete this doctor?",
        "Confirm Delete",
        MessageBoxButton.YesNo,
        MessageBoxImage.Warning);
    
    if (result == MessageBoxResult.Yes)
    {
        var success = await DbAccess.DeleteDoctorAsync(doctorId);
        
        if (success)
        {
            MessageBox.Show("Doctor deleted successfully");
        }
        else
        {
            MessageBox.Show("Doctor not found");
        }
    }
}
```

### Add Specialty to Doctor

Adds a specialty to a doctor's list of specialties.

```csharp
await DbAccess.AddSpecialtyToDoctorAsync(doctorId, specialtyId);
```

**Parameters**:
- `doctorId` (Guid) - The ID of the doctor
- `specialtyId` (Guid) - The ID of the specialty to add

**Example**:
```csharp
private async Task AddSpecialtyAsync(Guid doctorId, Guid specialtyId)
{
    await DbAccess.AddSpecialtyToDoctorAsync(doctorId, specialtyId);
    Console.WriteLine("Specialty added to doctor");
}
```

### Remove Specialty from Doctor

Removes a specialty from a doctor's list of specialties.

```csharp
await DbAccess.RemoveSpecialtyFromDoctorAsync(doctorId, specialtyId);
```

**Parameters**:
- `doctorId` (Guid) - The ID of the doctor
- `specialtyId` (Guid) - The ID of the specialty to remove

**Example**:
```csharp
private async Task RemoveSpecialtyAsync(Guid doctorId, Guid specialtyId)
{
    await DbAccess.RemoveSpecialtyFromDoctorAsync(doctorId, specialtyId);
    Console.WriteLine("Specialty removed from doctor");
}
```

## Doctor Specialties

### Get All Doctor Specialties

Retrieves all active doctor specialties, ordered by name.

```csharp
var specialties = await DbAccess.GetAllDoctorSpecialitiesAsync();
```

**Returns**: `List<DoctorSpeciality>` - List of all active specialties

**Example**:
```csharp
private async Task LoadSpecialtiesAsync()
{
    var specialties = await DbAccess.GetAllDoctorSpecialitiesAsync();
    
    // Bind to ComboBox
    SpecialtyComboBox.ItemsSource = specialties;
}
```

### Get Doctor Specialty by ID

Retrieves a single specialty by ID.

```csharp
var specialty = await DbAccess.GetDoctorSpecialityByIdAsync(specialtyId);
```

**Parameters**:
- `specialtyId` (Guid) - The ID of the specialty

**Returns**: `DoctorSpeciality?` - The specialty, or null if not found

**Example**:
```csharp
private async Task LoadSpecialtyAsync(Guid id)
{
    var specialty = await DbAccess.GetDoctorSpecialityByIdAsync(id);
    
    if (specialty != null)
    {
        Console.WriteLine($"Specialty: {specialty.Name}");
    }
}
```

### Add Doctor Specialty

Adds a new doctor specialty to the database.

```csharp
var specialtyId = await DbAccess.AddDoctorSpecialityAsync(specialty);
```

**Parameters**:
- `specialty` (DoctorSpeciality) - The specialty object to add

**Returns**: `Guid` - The ID of the newly created specialty

**Example**:
```csharp
private async Task AddNewSpecialtyAsync()
{
    var newSpecialty = new DoctorSpeciality
    {
        Name = "Cardiology",
        Description = "Heart and cardiovascular system specialist",
        IsActive = true
    };
    
    var specialtyId = await DbAccess.AddDoctorSpecialityAsync(newSpecialty);
    Console.WriteLine($"Added specialty with ID: {specialtyId}");
}
```

### Update Doctor Specialty

Updates an existing doctor specialty.

```csharp
var success = await DbAccess.UpdateDoctorSpecialityAsync(specialty);
```

**Parameters**:
- `specialty` (DoctorSpeciality) - The specialty object with updated values

**Returns**: `bool` - True if update succeeded, false if specialty not found

**Example**:
```csharp
private async Task UpdateSpecialtyAsync(DoctorSpeciality specialty)
{
    specialty.Description = "Updated description";
    
    var success = await DbAccess.UpdateDoctorSpecialityAsync(specialty);
}
```

### Delete Doctor Specialty

Soft deletes a doctor specialty by setting IsActive to false.

```csharp
var success = await DbAccess.DeleteDoctorSpecialityAsync(specialtyId);
```

**Parameters**:
- `specialtyId` (Guid) - The ID of the specialty to delete

**Returns**: `bool` - True if deletion succeeded, false if specialty not found

## Best Practices

1. **Always use async/await**: All methods are asynchronous to prevent UI freezing
2. **Handle exceptions**: Wrap calls in try-catch blocks for proper error handling
3. **Check return values**: Methods that return `bool` indicate success/failure
4. **Dispose automatically**: DbContext is automatically disposed - no manual cleanup needed
5. **Use soft deletes**: Deletion methods set IsActive to false, preserving data

## Error Handling

All methods can throw exceptions. Wrap in try-catch blocks:

```csharp
try
{
    var doctors = await DbAccess.GetAllDoctorsAsync();
    // Process doctors
}
catch (Exception ex)
{
    MessageBox.Show($"Error loading doctors: {ex.Message}", "Error", 
        MessageBoxButton.OK, MessageBoxImage.Error);
}
```

## Integration with ViewModels

Example of using DbAccess in a ViewModel:

```csharp
public class DoctorsViewModel : BaseViewModel
{
    private ObservableCollection<Doctor> _doctors;
    
    public ObservableCollection<Doctor> Doctors
    {
        get => _doctors;
        set => SetProperty(ref _doctors, value);
    }
    
    public async Task LoadDoctorsAsync()
    {
        try
        {
            var doctors = await DbAccess.GetAllDoctorsAsync();
            Doctors = new ObservableCollection<Doctor>(doctors);
        }
        catch (Exception ex)
        {
            // Handle error
        }
    }
    
    public async Task AddDoctorAsync(Doctor doctor)
    {
        try
        {
            var doctorId = await DbAccess.AddDoctorAsync(doctor);
            await LoadDoctorsAsync(); // Refresh list
        }
        catch (Exception ex)
        {
            // Handle error
        }
    }
}