using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentFeeMonth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FeeMonth",
                table: "StudentFees",
                type: "int",
                nullable: true);

            // Backfill: existing challans were billed for the month of their due date.
            migrationBuilder.Sql("UPDATE StudentFees SET FeeMonth = MONTH(DueDate) WHERE FeeMonth IS NULL;");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 996, DateTimeKind.Utc).AddTicks(9353));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 997, DateTimeKind.Utc).AddTicks(571));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 997, DateTimeKind.Utc).AddTicks(574));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 997, DateTimeKind.Utc).AddTicks(576));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 997, DateTimeKind.Utc).AddTicks(613));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 997, DateTimeKind.Utc).AddTicks(615));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 997, DateTimeKind.Utc).AddTicks(618));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(7704));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(9501));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(9506));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(9508));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(9510));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(9512));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(9513));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 15, DateTimeKind.Utc).AddTicks(3503));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 15, DateTimeKind.Utc).AddTicks(4526));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 15, DateTimeKind.Utc).AddTicks(4531));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 15, DateTimeKind.Utc).AddTicks(4533));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 15, DateTimeKind.Utc).AddTicks(4534));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 15, DateTimeKind.Utc).AddTicks(4559));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$3jM9Qr.SrI0bwMZMEmcL3.TSIv8INFG6CSBd3lLTftvx16q/t9oP.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeeMonth",
                table: "StudentFees");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(2211));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(3477));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(3481));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(3483));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(3484));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(3486));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(3487));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(3592));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(6209));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(6220));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(6222));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(6224));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(6227));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(6229));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 436, DateTimeKind.Utc).AddTicks(8233));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 436, DateTimeKind.Utc).AddTicks(9239));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 436, DateTimeKind.Utc).AddTicks(9243));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 436, DateTimeKind.Utc).AddTicks(9244));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 436, DateTimeKind.Utc).AddTicks(9245));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 436, DateTimeKind.Utc).AddTicks(9246));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$tcPOOt6E2R0xG2mtzaWjue6WlvHjt0s1dy806vKqwTUc9bRWPxue.");
        }
    }
}
