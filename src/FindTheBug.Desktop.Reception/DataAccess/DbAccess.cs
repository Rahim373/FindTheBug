using FindTheBug.Desktop.Reception.CusomEntity;
using FindTheBug.Desktop.Reception.Data;
using FindTheBug.Desktop.Reception.Dtos;
using FindTheBug.Desktop.Reception.Models;
using FindTheBug.Desktop.Reception.Utils;
using FindTheBug.Domain.Contracts;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FindTheBug.Desktop.Reception.DataAccess;

/// <summary>
/// Provides static methods for accessing and manipulating database data.
/// </summary>
public static class DbAccess
{
    /// <summary>
    /// Gets the DbContext from the application's service provider.
    /// </summary>
    private static ReceptionDbContext GetDbContext()
    {
        var serviceProvider = App.ServiceProvider;
        if (serviceProvider == null)
        {
            throw new InvalidOperationException("Application service provider is not initialized.");
        }

        return serviceProvider.GetRequiredService<ReceptionDbContext>();
    }

    #region Doctors Methods

    /// <summary>
    /// Gets all doctors from the database asynchronously.
    /// </summary>
    public static async Task<List<Doctor>> GetAllDoctorsAsync()
    {
        var dbContext = GetDbContext();
        return await dbContext.Doctors
            .Include(d => d.DoctorSpecialities)
            .ThenInclude(dsm => dsm.DoctorSpeciality)
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a doctor by ID from the database asynchronously.
    /// </summary>
    public static async Task<Doctor?> GetDoctorByIdAsync(Guid doctorId)
    {
        using var dbContext = GetDbContext();
        return await dbContext.Doctors
            .Include(d => d.DoctorSpecialities)
            .ThenInclude(dsm => dsm.DoctorSpeciality)
            .FirstOrDefaultAsync(d => d.Id == doctorId);
    }

    /// <summary>
    /// Searches for doctors by name or phone number asynchronously.
    /// </summary>
    public static async Task<List<Doctor>> SearchDoctorsAsync(string searchTerm)
    {
        using var dbContext = GetDbContext();
        
        var query = dbContext.Doctors
            .Include(d => d.DoctorSpecialities)
            .Where(d => d.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(d => 
                d.Name.Contains(searchTerm) || 
                d.PhoneNumber.Contains(searchTerm) ||
                (d.Degree != null && d.Degree.Contains(searchTerm)));
        }

        return await query
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Gets doctors by specialty asynchronously.
    /// </summary>
    public static async Task<List<Doctor>> GetDoctorsBySpecialtyAsync(Guid specialtyId)
    {
        using var dbContext = GetDbContext();
        
        return await dbContext.DoctorSpecialityMappings
            .Where(dsm => dsm.DoctorSpecialityId == specialtyId)
            .Select(dsm => dsm.Doctor)
            .Include(d => d.DoctorSpecialities)
            .ThenInclude(dsm => dsm.DoctorSpeciality)
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new doctor to the database asynchronously.
    /// </summary>
    public static async Task<Guid> AddDoctorAsync(Doctor doctor)
    {
        using var dbContext = GetDbContext();
        
        dbContext.Doctors.Add(doctor);
        await dbContext.SaveChangesAsync();
        
        return doctor.Id;
    }

    /// <summary>
    /// Updates an existing doctor in the database asynchronously.
    /// </summary>
    public static async Task<bool> UpdateDoctorAsync(Doctor doctor)
    {
        using var dbContext = GetDbContext();
        
        var existingDoctor = await dbContext.Doctors.FindAsync(doctor.Id);
        if (existingDoctor == null)
        {
            return false;
        }

        dbContext.Entry(existingDoctor).CurrentValues.SetValues(doctor);
        
        await dbContext.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Deletes a doctor from the database asynchronously (soft delete by setting IsActive to false).
    /// </summary>
    public static async Task<bool> DeleteDoctorAsync(Guid doctorId)
    {
        using var dbContext = GetDbContext();
        
        var doctor = await dbContext.Doctors.FindAsync(doctorId);
        if (doctor == null)
        {
            return false;
        }

        doctor.IsActive = false;
        await dbContext.SaveChangesAsync();
        
        return true;
    }

    /// <summary>
    /// Adds a specialty to a doctor asynchronously.
    /// </summary>
    public static async Task AddSpecialtyToDoctorAsync(Guid doctorId, Guid specialtyId)
    {
        using var dbContext = GetDbContext();
        
        var existingMapping = await dbContext.DoctorSpecialityMappings
            .FirstOrDefaultAsync(dsm => dsm.DoctorId == doctorId && dsm.DoctorSpecialityId == specialtyId);
        
        if (existingMapping != null)
        {
            return; // Mapping already exists
        }

        var mapping = new DoctorSpecialityMap
        {
            DoctorId = doctorId,
            DoctorSpecialityId = specialtyId
        };

        dbContext.DoctorSpecialityMappings.Add(mapping);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Removes a specialty from a doctor asynchronously.
    /// </summary>
    public static async Task RemoveSpecialtyFromDoctorAsync(Guid doctorId, Guid specialtyId)
    {
        using var dbContext = GetDbContext();
        
        var mapping = await dbContext.DoctorSpecialityMappings
            .FirstOrDefaultAsync(dsm => dsm.DoctorId == doctorId && dsm.DoctorSpecialityId == specialtyId);
        
        if (mapping != null)
        {
            dbContext.DoctorSpecialityMappings.Remove(mapping);
            await dbContext.SaveChangesAsync();
        }
    }

    #endregion

    #region Doctor Specialties Methods

    /// <summary>
    /// Gets all doctor specialties from the database asynchronously.
    /// </summary>
    public static async Task<List<DoctorSpeciality>> GetAllDoctorSpecialitiesAsync()
    {
        using var dbContext = GetDbContext();
        return await dbContext.DoctorSpecialities
            .Where(ds => ds.IsActive)
            .OrderBy(ds => ds.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a doctor specialty by ID asynchronously.
    /// </summary>
    public static async Task<DoctorSpeciality?> GetDoctorSpecialityByIdAsync(Guid specialtyId)
    {
        using var dbContext = GetDbContext();
        return await dbContext.DoctorSpecialities
            .FirstOrDefaultAsync(ds => ds.Id == specialtyId);
    }

    /// <summary>
    /// Adds a new doctor specialty to the database asynchronously.
    /// </summary>
    public static async Task<Guid> AddDoctorSpecialityAsync(DoctorSpeciality specialty)
    {
        using var dbContext = GetDbContext();
        
        dbContext.DoctorSpecialities.Add(specialty);
        await dbContext.SaveChangesAsync();
        
        return specialty.Id;
    }

    /// <summary>
    /// Updates an existing doctor specialty in the database asynchronously.
    /// </summary>
    public static async Task<bool> UpdateDoctorSpecialityAsync(DoctorSpeciality specialty)
    {
        using var dbContext = GetDbContext();
        
        var existingSpecialty = await dbContext.DoctorSpecialities.FindAsync(specialty.Id);
        if (existingSpecialty == null)
        {
            return false;
        }

        dbContext.Entry(existingSpecialty).CurrentValues.SetValues(specialty);
        await dbContext.SaveChangesAsync();
        
        return true;
    }

    /// <summary>
    /// Deletes a doctor specialty from the database asynchronously (soft delete).
    /// </summary>
    public static async Task<bool> DeleteDoctorSpecialityAsync(Guid specialtyId)
    {
        using var dbContext = GetDbContext();
        
        var specialty = await dbContext.DoctorSpecialities.FindAsync(specialtyId);
        if (specialty == null)
        {
            return false;
        }

        specialty.IsActive = false;
        await dbContext.SaveChangesAsync();
        
        return true;
    }

    internal static async Task<User?> CheckLoginAsync(string phoneNumber, string password)
    {
        var dbContext = GetDbContext();

        var dbUser = await dbContext.Users.Where(x => x.Phone == phoneNumber && x.IsActive
                && x.UserRoles.Any(y => y.Role.RoleModulePermissions
                    .Any(z => z.Module.Name == ModuleConstants.Reception && z.CanView)))
            .FirstOrDefaultAsync();

        if (dbUser is not null && PasswordHasher.VerifyPassword(password, dbUser.PasswordHash))
        {
            return dbUser;
        }

        return null;
    }

    internal static async Task<Guid> SaveReceiptAsync(ReceiptInformation receiptInfo, List<LabTestDto> selectedTests)
    {
        var dbContext = GetDbContext();
        Guid? referredByDoctorId = null;

        // Parse doctor ID if provided
        if (!string.IsNullOrEmpty(receiptInfo.ReferredBy.Value) && 
            Guid.TryParse(receiptInfo.ReferredBy.Value, out var parsedDoctorId))
        {
            referredByDoctorId = parsedDoctorId;
        }

        // Create new lab receipt
        var labReceipt = new DesktopLabReceipt
        {
            Address = receiptInfo.Address.Value,
            Age = receiptInfo.Age.Value,
            PhoneNumber = receiptInfo.PhoneNumber.Value,
            Balace = receiptInfo.Balance.Value,
            Discount = receiptInfo.Discount.Value,
            Due = receiptInfo.Due.Value,
            IsAgeYear = receiptInfo.IsAgeYear.Value,
            CreatedAt = DateTime.UtcNow,
            Gender = receiptInfo.Gender.Value,
            FullName = receiptInfo.PatientName.Value,
            SubTotal = receiptInfo.SubTotal.Value,
            Total = receiptInfo.Total.Value,
            ReferredByDoctorId = referredByDoctorId,
            LabReceiptStatus = LabReceiptStatus.Paid,
            InvoiceNumber = GenerateInvoiceNumber()
        };

        // Set the user who created this receipt
        if (App.CurrentUser != null)
        {
            labReceipt.CreatedBy = $"{App.CurrentUser.FirstName} {App.CurrentUser.LastName} ({App.CurrentUser.Phone})".Trim();
        }

        await dbContext.LabReceipts.AddAsync(labReceipt);
        await dbContext.SaveChangesAsync();

        // Add test entries for the receipt
        foreach (var testDto in selectedTests)
        {
            var testEntry = new ReceiptTest
            {
                LabReceiptId = labReceipt.Id,
                DiagnosticTestId = testDto.Id,
                Amount = testDto.Amount,
                DiscountPercentage = testDto.Discount,
                Total = testDto.Total,
                Status = ReceiptTestStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };

            await dbContext.ReceiptTests.AddAsync(testEntry);
        }

        await dbContext.SaveChangesAsync();

        // Mark as dirty for sync
        ((IPushableEntity)labReceipt).IsDirty = true;
        await dbContext.SaveChangesAsync();

        return labReceipt.Id;
    }

    /// <summary>
    /// Generates a unique invoice number for the receipt
    /// </summary>
    private static string GenerateInvoiceNumber()
    {
        return $"{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
    }

    /// <summary>
    /// Gets all doctor specialties from the database asynchronously.
    /// </summary>
    public static async Task<List<DropdownOption>> GetAllDiagnosticsTestForDropdownAsync()
    {
        var dbContext = GetDbContext();
        return await dbContext.DiagnosticTests
            .Where(ds => ds.IsActive)
            .OrderBy(ds => ds.TestName)
            .Select(dt => new DropdownOption(dt.Id, dt.TestName, dt.Category))
            .ToListAsync();
    }

    /// <summary>
    /// Gets all doctors from the database asynchronously.
    /// </summary>
    public static async Task<List<DropdownOption>> GetAllDoctorsForDropdownAsync()
    {
        var dbContext = GetDbContext();
        return await dbContext.Doctors
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .Select(d => new DropdownOption(d.Id, $"{d.Name} ({d.Degree})"))
            .ToListAsync();
    }

    internal static async Task<DiagnosticTest?> GetDiagnosticsTestByIdAsync(Guid value)
    {
        var dbContext = GetDbContext();
        return await dbContext.DiagnosticTests
            .FindAsync(value);
    }

    #endregion
}