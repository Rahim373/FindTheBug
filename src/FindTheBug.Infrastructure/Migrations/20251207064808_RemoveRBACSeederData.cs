using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FindTheBug.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRBACSeederData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RoleModulePermissions",
                keyColumn: "Id",
                keyValue: new Guid("1104b19a-4bb2-48e7-a2d4-78aac20403cc"));

            migrationBuilder.DeleteData(
                table: "RoleModulePermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d2dea58-a5f4-4b12-b059-af7369086e06"));

            migrationBuilder.DeleteData(
                table: "RoleModulePermissions",
                keyColumn: "Id",
                keyValue: new Guid("33019cfa-fdf9-4db8-a405-4c58a2389b7f"));

            migrationBuilder.DeleteData(
                table: "RoleModulePermissions",
                keyColumn: "Id",
                keyValue: new Guid("398cbe4f-55e1-49b2-85c1-da914bc7e24c"));

            migrationBuilder.DeleteData(
                table: "RoleModulePermissions",
                keyColumn: "Id",
                keyValue: new Guid("5b66d394-fa2f-4cfc-acbf-cb358f33c2f7"));

            migrationBuilder.DeleteData(
                table: "RoleModulePermissions",
                keyColumn: "Id",
                keyValue: new Guid("5bffe60a-776d-48d0-890b-14f92d4fd644"));

            migrationBuilder.DeleteData(
                table: "RoleModulePermissions",
                keyColumn: "Id",
                keyValue: new Guid("6652643e-ffbd-45db-b7c4-bb0aa81737ef"));

            migrationBuilder.DeleteData(
                table: "RoleModulePermissions",
                keyColumn: "Id",
                keyValue: new Guid("6ca512c3-bcd6-4a92-89c1-d76bca9f0775"));

            migrationBuilder.DeleteData(
                table: "RoleModulePermissions",
                keyColumn: "Id",
                keyValue: new Guid("8ece6aa1-3386-42f1-b467-e45d277b6f9c"));

            migrationBuilder.DeleteData(
                table: "RoleModulePermissions",
                keyColumn: "Id",
                keyValue: new Guid("bc4a4bd7-a654-4d5d-ad40-d8bc255aff85"));

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
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "DisplayName", "Icon", "IsActive", "Name", "Route", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 12, 5, 18, 40, 15, 713, DateTimeKind.Utc).AddTicks(216), "System", "Main dashboard and overview", "Dashboard", "dashboard", true, "Dashboard", "/admin/dashboard", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 12, 5, 18, 40, 15, 713, DateTimeKind.Utc).AddTicks(653), "System", "Manage system users and their access", "User Management", "user", true, "Users", "/admin/users", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 12, 5, 18, 40, 15, 713, DateTimeKind.Utc).AddTicks(657), "System", "Manage user roles and permissions", "Role Management", "safety", true, "Roles", "/admin/roles", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2025, 12, 5, 18, 40, 15, 713, DateTimeKind.Utc).AddTicks(659), "System", "Manage system modules", "Module Management", "appstore", true, "Modules", "/admin/modules", null, null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsSystemRole", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2025, 12, 5, 18, 40, 15, 713, DateTimeKind.Utc).AddTicks(7336), "System", "Administrator with full access to all modules", true, true, "Admin", null, null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2025, 12, 5, 18, 40, 15, 713, DateTimeKind.Utc).AddTicks(7341), "System", "Standard user with limited access", true, true, "User", null, null },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2025, 12, 5, 18, 40, 15, 713, DateTimeKind.Utc).AddTicks(7344), "System", "Super administrator with unrestricted access", true, true, "SuperUser", null, null }
                });

            migrationBuilder.InsertData(
                table: "RoleModulePermissions",
                columns: new[] { "Id", "CanCreate", "CanDelete", "CanEdit", "CanView", "CreatedAt", "ModuleId", "RoleId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1104b19a-4bb2-48e7-a2d4-78aac20403cc"), true, true, true, true, new DateTime(2025, 12, 5, 18, 40, 15, 714, DateTimeKind.Utc).AddTicks(2528), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), null },
                    { new Guid("2d2dea58-a5f4-4b12-b059-af7369086e06"), true, true, true, true, new DateTime(2025, 12, 5, 18, 40, 15, 714, DateTimeKind.Utc).AddTicks(1242), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null },
                    { new Guid("33019cfa-fdf9-4db8-a405-4c58a2389b7f"), true, true, true, true, new DateTime(2025, 12, 5, 18, 40, 15, 714, DateTimeKind.Utc).AddTicks(2511), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), null },
                    { new Guid("398cbe4f-55e1-49b2-85c1-da914bc7e24c"), true, true, true, true, new DateTime(2025, 12, 5, 18, 40, 15, 714, DateTimeKind.Utc).AddTicks(1526), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null },
                    { new Guid("5b66d394-fa2f-4cfc-acbf-cb358f33c2f7"), false, false, false, true, new DateTime(2025, 12, 5, 18, 40, 15, 714, DateTimeKind.Utc).AddTicks(2604), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), null },
                    { new Guid("5bffe60a-776d-48d0-890b-14f92d4fd644"), false, false, false, true, new DateTime(2025, 12, 5, 18, 40, 15, 714, DateTimeKind.Utc).AddTicks(2607), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), null },
                    { new Guid("6652643e-ffbd-45db-b7c4-bb0aa81737ef"), true, true, true, true, new DateTime(2025, 12, 5, 18, 40, 15, 714, DateTimeKind.Utc).AddTicks(1524), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null },
                    { new Guid("6ca512c3-bcd6-4a92-89c1-d76bca9f0775"), true, true, true, true, new DateTime(2025, 12, 5, 18, 40, 15, 714, DateTimeKind.Utc).AddTicks(1527), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null },
                    { new Guid("8ece6aa1-3386-42f1-b467-e45d277b6f9c"), true, true, true, true, new DateTime(2025, 12, 5, 18, 40, 15, 714, DateTimeKind.Utc).AddTicks(2513), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), null },
                    { new Guid("bc4a4bd7-a654-4d5d-ad40-d8bc255aff85"), true, true, true, true, new DateTime(2025, 12, 5, 18, 40, 15, 714, DateTimeKind.Utc).AddTicks(2542), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), null }
                });
        }
    }
}
