using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Infrastructure.Mappings;

/// <summary>
/// Entity Framework configuration for Expense entity
/// </summary>
public class ExpenseMapping : IMapping<Expense>
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Date)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.Note)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.PaymentMethod)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.ReferenceNo)
                .HasColumnType("text");

            entity.Property(e => e.Attachment)
                .HasColumnType("text");

            entity.Property(e => e.Department)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.CreatedBy)
                .HasColumnType("text");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.UpdatedBy)
                .HasColumnType("text");

            entity.ToTable("Expenses");
        });
    }
}