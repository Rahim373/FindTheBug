using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Desktop.Reception.Data;

public class ReceptionDbContext : DbContext
{
    public ReceptionDbContext(DbContextOptions<ReceptionDbContext> options) : base(options)
    {
    }

    // Patient Management
    public DbSet<LabReceipt> Patients => Set<LabReceipt>();

    // Doctors Management
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<DoctorSpeciality> DoctorSpecialities => Set<DoctorSpeciality>();
    public DbSet<DoctorSpecialityMap> DoctorSpecialityMappings => Set<DoctorSpecialityMap>();

    // Invoice/Receipt Management
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    // Laboratory Management
    public DbSet<DiagnosticTest> DiagnosticTests => Set<DiagnosticTest>();


    // RBAC (Role-Based Access Control) - Synced from Cloud
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<RoleModulePermission> RoleModulePermissions => Set<RoleModulePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}