using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseMaterialContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "CourseMaterials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(1653));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(4028));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(4036));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(4038));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(4040));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(4042));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(4044));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(3447));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(6254));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(6261));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(6263));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(6265));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(6267));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(6268));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 978, DateTimeKind.Utc).AddTicks(4533));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 978, DateTimeKind.Utc).AddTicks(5536));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 978, DateTimeKind.Utc).AddTicks(5540));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 978, DateTimeKind.Utc).AddTicks(5542));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 978, DateTimeKind.Utc).AddTicks(5544));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 978, DateTimeKind.Utc).AddTicks(5545));

            // Skip PasswordHash seed updates.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "CourseMaterials");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 79, DateTimeKind.Utc).AddTicks(9133));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 80, DateTimeKind.Utc).AddTicks(205));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 80, DateTimeKind.Utc).AddTicks(207));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 80, DateTimeKind.Utc).AddTicks(208));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 80, DateTimeKind.Utc).AddTicks(210));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 80, DateTimeKind.Utc).AddTicks(211));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 80, DateTimeKind.Utc).AddTicks(212));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(7891));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(9343));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(9348));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(9349));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(9351));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(9376));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(9378));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 89, DateTimeKind.Utc).AddTicks(7620));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 89, DateTimeKind.Utc).AddTicks(8407));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 89, DateTimeKind.Utc).AddTicks(8410));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 89, DateTimeKind.Utc).AddTicks(8411));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 89, DateTimeKind.Utc).AddTicks(8412));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 89, DateTimeKind.Utc).AddTicks(8413));

            // Skip PasswordHash seed rollback.
        }
    }
}
