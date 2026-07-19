using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentLeavingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "LeavingDate",
                table: "Students",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeavingReason",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(3796));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(5806));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(5810));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(5812));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(5814));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(5816));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(5817));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(2198));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(5092));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(5099));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(5103));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(5108));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(5110));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(5112));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 203, DateTimeKind.Utc).AddTicks(2596));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 203, DateTimeKind.Utc).AddTicks(3862));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 203, DateTimeKind.Utc).AddTicks(3866));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 203, DateTimeKind.Utc).AddTicks(3867));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 203, DateTimeKind.Utc).AddTicks(3894));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 203, DateTimeKind.Utc).AddTicks(3895));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$hAsajSlrxAOU22zHDdCXSuyx/4z/UMt7.M3LFM8UxuClFXEl8FXuy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeavingDate",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LeavingReason",
                table: "Students");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 211, DateTimeKind.Utc).AddTicks(7508));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 211, DateTimeKind.Utc).AddTicks(8979));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 211, DateTimeKind.Utc).AddTicks(8983));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 211, DateTimeKind.Utc).AddTicks(8984));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 211, DateTimeKind.Utc).AddTicks(8986));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 211, DateTimeKind.Utc).AddTicks(8987));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 211, DateTimeKind.Utc).AddTicks(8988));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 610, DateTimeKind.Utc).AddTicks(8284));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 610, DateTimeKind.Utc).AddTicks(9934));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 610, DateTimeKind.Utc).AddTicks(9968));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 610, DateTimeKind.Utc).AddTicks(9970));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 610, DateTimeKind.Utc).AddTicks(9972));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 610, DateTimeKind.Utc).AddTicks(9974));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 610, DateTimeKind.Utc).AddTicks(9975));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 232, DateTimeKind.Utc).AddTicks(5575));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 232, DateTimeKind.Utc).AddTicks(6719));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 232, DateTimeKind.Utc).AddTicks(6723));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 232, DateTimeKind.Utc).AddTicks(6725));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 232, DateTimeKind.Utc).AddTicks(6726));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 16, 19, 12, 36, 232, DateTimeKind.Utc).AddTicks(6727));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$LPgHKONut1iVGNaGyuhe2u/.ZhszxqtkaDH.bYXLNhA49EmwxURnW");
        }
    }
}
