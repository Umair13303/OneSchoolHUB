using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class TenantIsolationHardening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_AdmissionNo",
                table: "Students");

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

            migrationBuilder.CreateIndex(
                name: "IX_Students_InstituteId_AdmissionNo",
                table: "Students",
                columns: new[] { "InstituteId", "AdmissionNo" },
                unique: true,
                filter: "[InstituteId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_InstituteId_AdmissionNo",
                table: "Students");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 267, DateTimeKind.Utc).AddTicks(7875));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 267, DateTimeKind.Utc).AddTicks(9073));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 267, DateTimeKind.Utc).AddTicks(9076));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 267, DateTimeKind.Utc).AddTicks(9078));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 267, DateTimeKind.Utc).AddTicks(9079));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 267, DateTimeKind.Utc).AddTicks(9081));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 267, DateTimeKind.Utc).AddTicks(9083));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 547, DateTimeKind.Utc).AddTicks(284));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 547, DateTimeKind.Utc).AddTicks(2933));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 547, DateTimeKind.Utc).AddTicks(2973));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 547, DateTimeKind.Utc).AddTicks(2975));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 547, DateTimeKind.Utc).AddTicks(2977));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 547, DateTimeKind.Utc).AddTicks(2979));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 547, DateTimeKind.Utc).AddTicks(2981));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 279, DateTimeKind.Utc).AddTicks(7739));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 279, DateTimeKind.Utc).AddTicks(8609));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 279, DateTimeKind.Utc).AddTicks(8614));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 279, DateTimeKind.Utc).AddTicks(8615));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 279, DateTimeKind.Utc).AddTicks(8616));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 16, 13, 279, DateTimeKind.Utc).AddTicks(8617));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$cv.r5RBXjKT2dwEva4m8pOstOPf3/P69s7sPphwzxWdMsc0eoyewS");

            migrationBuilder.CreateIndex(
                name: "IX_Students_AdmissionNo",
                table: "Students",
                column: "AdmissionNo",
                unique: true);
        }
    }
}
