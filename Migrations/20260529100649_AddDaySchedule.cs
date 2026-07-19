using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDaySchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DaySchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    IsWorkingDay = table.Column<bool>(type: "bit", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    NumberOfPeriods = table.Column<int>(type: "int", nullable: false),
                    HasBreak = table.Column<bool>(type: "bit", nullable: false),
                    BreakAfterPeriod = table.Column<int>(type: "int", nullable: false),
                    BreakDuration = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaySchedules", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 863, DateTimeKind.Utc).AddTicks(4601));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 863, DateTimeKind.Utc).AddTicks(6829));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 863, DateTimeKind.Utc).AddTicks(6833));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 863, DateTimeKind.Utc).AddTicks(6835));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 863, DateTimeKind.Utc).AddTicks(6837));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 863, DateTimeKind.Utc).AddTicks(6838));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 863, DateTimeKind.Utc).AddTicks(6840));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 546, DateTimeKind.Utc).AddTicks(5628));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 546, DateTimeKind.Utc).AddTicks(7731));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 546, DateTimeKind.Utc).AddTicks(7741));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 546, DateTimeKind.Utc).AddTicks(7743));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 546, DateTimeKind.Utc).AddTicks(7745));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 6, 46, 546, DateTimeKind.Utc).AddTicks(7747));

            migrationBuilder.UpdateData(
                table: "SchoolSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FridayBreakDuration", "RegularBreakDuration" },
                values: new object[] { 10, 10 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$.4z19imECcsRkW89z5JjsOBnMVckNAjCDm313d278xRqVZ8tcebSi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DaySchedules");

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 44, 407, DateTimeKind.Utc).AddTicks(2580));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 44, 407, DateTimeKind.Utc).AddTicks(5650));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 44, 407, DateTimeKind.Utc).AddTicks(5655));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 44, 407, DateTimeKind.Utc).AddTicks(5658));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 44, 407, DateTimeKind.Utc).AddTicks(5660));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 44, 407, DateTimeKind.Utc).AddTicks(5662));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 44, 407, DateTimeKind.Utc).AddTicks(5665));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 43, 850, DateTimeKind.Utc).AddTicks(7114));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 43, 850, DateTimeKind.Utc).AddTicks(8996));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 43, 850, DateTimeKind.Utc).AddTicks(9003));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 43, 850, DateTimeKind.Utc).AddTicks(9072));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 43, 850, DateTimeKind.Utc).AddTicks(9073));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 8, 55, 43, 850, DateTimeKind.Utc).AddTicks(9075));

            migrationBuilder.UpdateData(
                table: "SchoolSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FridayBreakDuration", "RegularBreakDuration" },
                values: new object[] { 20, 20 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$ckS/w25dkAemICuO1YnyLeRD8UkbrhEBMwo11hlqol7KpJlnMFwwG");
        }
    }
}
