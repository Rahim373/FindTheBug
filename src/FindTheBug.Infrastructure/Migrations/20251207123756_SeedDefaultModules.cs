using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FindTheBug.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "DisplayName", "IsActive", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 12, 7, 12, 37, 55, 331, DateTimeKind.Utc).AddTicks(9981), null, "Main dashboard view", "Dashboard", true, "Dashboard", new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(315), null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(853), null, "Manage system users", "User Management", true, "Users", new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(853), null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(860), null, "Manage system roles and permissions", "Role Management", true, "Roles", new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(860), null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(870), null, "Manage patient records", "Patient Management", true, "Patients", new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(870), null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(873), null, "Manage diagnostic tests", "Diagnostic Tests", true, "DiagnosticTests", new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(873), null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(907), null, "Manage test entries", "Test Entries", true, "TestEntries", new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(907), null },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(910), null, "Manage invoices and billing", "Invoice Management", true, "Invoices", new DateTime(2025, 12, 7, 12, 37, 55, 332, DateTimeKind.Utc).AddTicks(910), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));
        }
    }
}
