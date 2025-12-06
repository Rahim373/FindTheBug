using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FindTheBug.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRBACTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Route = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsSystemRole = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleModulePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CanView = table.Column<bool>(type: "boolean", nullable: false),
                    CanCreate = table.Column<bool>(type: "boolean", nullable: false),
                    CanEdit = table.Column<bool>(type: "boolean", nullable: false),
                    CanDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleModulePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleModulePermissions_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleModulePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Modules_Name",
                table: "Modules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleModulePermissions_ModuleId",
                table: "RoleModulePermissions",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleModulePermissions_RoleId_ModuleId",
                table: "RoleModulePermissions",
                columns: new[] { "RoleId", "ModuleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_RoleId",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleModulePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.AddColumn<string>(
                name: "Roles",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
