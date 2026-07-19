using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingsMenuItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "MenuItemId", "CreatedAt", "CreatedBy", "Icon", "IsActive", "IsDeleted", "ParentId", "RouteUrl", "SortOrder", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 26, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "tune", true, false, null, "/admin/settings", 95, "School Settings", null, null });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 1, 118, DateTimeKind.Utc).AddTicks(3332));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 1, 118, DateTimeKind.Utc).AddTicks(4907));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 1, 118, DateTimeKind.Utc).AddTicks(4912));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 1, 118, DateTimeKind.Utc).AddTicks(4914));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 1, 118, DateTimeKind.Utc).AddTicks(4916));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 1, 118, DateTimeKind.Utc).AddTicks(4917));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 1, 118, DateTimeKind.Utc).AddTicks(4919));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 0, 804, DateTimeKind.Utc).AddTicks(529));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 0, 804, DateTimeKind.Utc).AddTicks(1949));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 0, 804, DateTimeKind.Utc).AddTicks(1956));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 0, 804, DateTimeKind.Utc).AddTicks(1958));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 0, 804, DateTimeKind.Utc).AddTicks(1959));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 42, 0, 804, DateTimeKind.Utc).AddTicks(1961));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$X8cTpipKjXnyZ3ZfLGi7YuCYbry0GSW6CP/iQQtSt0VZGRv/56Fbu");

            migrationBuilder.InsertData(
                table: "MenuRolePermissions",
                columns: new[] { "Id", "MenuItemId", "RoleId" },
                values: new object[,]
                {
                    { 82, 26, 1 },
                    { 83, 26, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 26);

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 689, DateTimeKind.Utc).AddTicks(2605));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 689, DateTimeKind.Utc).AddTicks(5396));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 689, DateTimeKind.Utc).AddTicks(5403));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 689, DateTimeKind.Utc).AddTicks(5405));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 689, DateTimeKind.Utc).AddTicks(5407));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 689, DateTimeKind.Utc).AddTicks(5409));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 689, DateTimeKind.Utc).AddTicks(5412));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 325, DateTimeKind.Utc).AddTicks(636));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 325, DateTimeKind.Utc).AddTicks(2193));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 325, DateTimeKind.Utc).AddTicks(2199));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 325, DateTimeKind.Utc).AddTicks(2202));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 325, DateTimeKind.Utc).AddTicks(2203));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 4, 28, 27, 325, DateTimeKind.Utc).AddTicks(2205));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$SpL7Ds0sjOHvvlqNRW7Qf.JqKPPyCeVqpdasKy9Mt8bA.5rzfXgaC");
        }
    }
}
