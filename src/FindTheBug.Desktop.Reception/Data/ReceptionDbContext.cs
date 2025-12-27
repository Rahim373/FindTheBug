using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Desktop.Reception.Data;

public class ReceptionDbContext : DbContext
{
    public ReceptionDbContext(DbContextOptions<ReceptionDbContext> options) : base(options)
    {
    }

    // Patient Management
    public DbSet<Patient> Patients => Set<Patient>();

    // Doctors Management
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<DoctorSpeciality> DoctorSpecialities => Set<DoctorSpeciality>();
    public DbSet<DoctorSpecialityMap> DoctorSpecialityMappings => Set<DoctorSpecialityMap>();

    // Invoice/Receipt Management
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();


    // RBAC (Role-Based Access Control) - Synced from Cloud
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<RoleModulePermission> RoleModulePermissions => Set<RoleModulePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Patient entity
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PatientCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.MobileNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.EmergencyContact).HasMaxLength(200);
            entity.Property(e => e.EmergencyContactNumber).HasMaxLength(20);
        });

        // Configure Doctor entity
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Degree).HasMaxLength(500);
            entity.Property(e => e.Office).HasMaxLength(200);
        });

        // Configure DoctorSpeciality entity
        modelBuilder.Entity<DoctorSpeciality>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        });

        // Configure DoctorSpecialityMap (many-to-many relationship)
        modelBuilder.Entity<DoctorSpecialityMap>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(d => d.Doctor)
                .WithMany(d => d.DoctorSpecialities)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.DoctorSpeciality)
                .WithMany(d => d.DoctorMappings)
                .HasForeignKey(d => d.DoctorSpecialityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Invoice entity
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasOne(i => i.Patient)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure InvoiceItem entity
        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.DiscountPercentage).HasPrecision(5, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.HasOne(i => i.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey(i => i.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}