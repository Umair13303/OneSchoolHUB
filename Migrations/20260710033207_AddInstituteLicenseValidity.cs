using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddInstituteLicenseValidity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseValidUntil",
                table: "Institutes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(726));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(2067));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(2069));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(2071));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(2085));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(2087));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(2089));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 419, DateTimeKind.Utc).AddTicks(7493));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 420, DateTimeKind.Utc).AddTicks(410));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 420, DateTimeKind.Utc).AddTicks(418));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 420, DateTimeKind.Utc).AddTicks(420));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 420, DateTimeKind.Utc).AddTicks(422));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 420, DateTimeKind.Utc).AddTicks(424));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 420, DateTimeKind.Utc).AddTicks(426));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 110, DateTimeKind.Utc).AddTicks(5329));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 110, DateTimeKind.Utc).AddTicks(7122));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 110, DateTimeKind.Utc).AddTicks(7127));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 110, DateTimeKind.Utc).AddTicks(7128));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 110, DateTimeKind.Utc).AddTicks(7130));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 110, DateTimeKind.Utc).AddTicks(7131));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$qBvuXp12/2YNE22.oN38WOh/1cThcrjtejGhDnejbp90dPmDe/oXC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseValidUntil",
                table: "Institutes");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 94, DateTimeKind.Utc).AddTicks(7405));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 94, DateTimeKind.Utc).AddTicks(8626));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 94, DateTimeKind.Utc).AddTicks(8629));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 94, DateTimeKind.Utc).AddTicks(8631));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 94, DateTimeKind.Utc).AddTicks(8632));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 94, DateTimeKind.Utc).AddTicks(8634));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 94, DateTimeKind.Utc).AddTicks(8635));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 368, DateTimeKind.Utc).AddTicks(1847));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 368, DateTimeKind.Utc).AddTicks(3675));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 368, DateTimeKind.Utc).AddTicks(3678));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 368, DateTimeKind.Utc).AddTicks(3680));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 368, DateTimeKind.Utc).AddTicks(3682));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 368, DateTimeKind.Utc).AddTicks(3684));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 368, DateTimeKind.Utc).AddTicks(3712));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 113, DateTimeKind.Utc).AddTicks(6816));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 113, DateTimeKind.Utc).AddTicks(7898));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 113, DateTimeKind.Utc).AddTicks(7902));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 113, DateTimeKind.Utc).AddTicks(7903));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 113, DateTimeKind.Utc).AddTicks(7905));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 28, 59, 113, DateTimeKind.Utc).AddTicks(7906));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$n9E.DD.KUHnXPFNoJsuop.AK8ehivuyhe5jUduOc51iAgdx.9TcZa");
        }
    }
}
