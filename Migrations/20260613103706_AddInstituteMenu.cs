using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddInstituteMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 965, DateTimeKind.Utc).AddTicks(9617));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 966, DateTimeKind.Utc).AddTicks(985));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 966, DateTimeKind.Utc).AddTicks(989));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 966, DateTimeKind.Utc).AddTicks(990));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 966, DateTimeKind.Utc).AddTicks(992));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 966, DateTimeKind.Utc).AddTicks(993));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 966, DateTimeKind.Utc).AddTicks(995));

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "MenuItemId", "CampusId", "CreatedAt", "CreatedBy", "Icon", "InstituteId", "IsActive", "IsDeleted", "ParentId", "RouteUrl", "SortOrder", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 41, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "domain", null, true, false, null, "/superadmin/institutes", 5, "Institutes", null, null });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 4, 294, DateTimeKind.Utc).AddTicks(8016));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 4, 294, DateTimeKind.Utc).AddTicks(9658));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 4, 294, DateTimeKind.Utc).AddTicks(9662));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 4, 294, DateTimeKind.Utc).AddTicks(9664));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 4, 294, DateTimeKind.Utc).AddTicks(9665));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 4, 294, DateTimeKind.Utc).AddTicks(9667));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 4, 294, DateTimeKind.Utc).AddTicks(9668));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 988, DateTimeKind.Utc).AddTicks(1943));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 988, DateTimeKind.Utc).AddTicks(2941));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 988, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 988, DateTimeKind.Utc).AddTicks(2947));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 988, DateTimeKind.Utc).AddTicks(2948));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 37, 3, 988, DateTimeKind.Utc).AddTicks(2951));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$7v6hXZuYJ7wvhFxk6BuFCu4M2I5HYaXnkydPYDSSEfPR3djCAkC1.");

            migrationBuilder.InsertData(
                table: "MenuRolePermissions",
                columns: new[] { "Id", "MenuItemId", "RoleId" },
                values: new object[] { 138, 41, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 41);

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(6705));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(9223));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(9228));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(9232));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(9235));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(9238));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(9242));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(3169));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(6302));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(6308));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(6310));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(6311));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(6313));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(6315));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 435, DateTimeKind.Utc).AddTicks(8413));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 436, DateTimeKind.Utc).AddTicks(145));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 436, DateTimeKind.Utc).AddTicks(149));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 436, DateTimeKind.Utc).AddTicks(151));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 436, DateTimeKind.Utc).AddTicks(153));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 436, DateTimeKind.Utc).AddTicks(155));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$7.JgRSSChZY9HTLj8/28a.ykjV4Ul4RjskdSA3bMCtJuXDayJom9S");
        }
    }
}
