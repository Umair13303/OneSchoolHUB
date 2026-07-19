using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSchoolSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchoolSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegularStartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    RegularEndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    RegularTotalPeriods = table.Column<int>(type: "int", nullable: false),
                    RegularBreakAllowed = table.Column<bool>(type: "bit", nullable: false),
                    RegularBreakStart = table.Column<TimeOnly>(type: "time", nullable: false),
                    RegularBreakEnd = table.Column<TimeOnly>(type: "time", nullable: false),
                    RegularGapMinutes = table.Column<int>(type: "int", nullable: false),
                    FridayStartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    FridayEndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    FridayTotalPeriods = table.Column<int>(type: "int", nullable: false),
                    FridayBreakAllowed = table.Column<bool>(type: "bit", nullable: false),
                    FridayBreakStart = table.Column<TimeOnly>(type: "time", nullable: false),
                    FridayBreakEnd = table.Column<TimeOnly>(type: "time", nullable: false),
                    FridayGapMinutes = table.Column<int>(type: "int", nullable: false),
                    SundayEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SundayStartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    SundayEndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    HolidaysJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinPeriodDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    MaxPeriodDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    MaxPeriodsPerDay = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSettings", x => x.Id);
                });

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

            migrationBuilder.InsertData(
                table: "SchoolSettings",
                columns: new[] { "Id", "FridayBreakAllowed", "FridayBreakEnd", "FridayBreakStart", "FridayEndTime", "FridayGapMinutes", "FridayStartTime", "FridayTotalPeriods", "HolidaysJson", "MaxPeriodDurationMinutes", "MaxPeriodsPerDay", "MinPeriodDurationMinutes", "RegularBreakAllowed", "RegularBreakEnd", "RegularBreakStart", "RegularEndTime", "RegularGapMinutes", "RegularStartTime", "RegularTotalPeriods", "SundayEnabled", "SundayEndTime", "SundayStartTime", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, false, new TimeOnly(10, 10, 0), new TimeOnly(10, 0, 0), new TimeOnly(11, 30, 0), 0, new TimeOnly(8, 0, 0), 4, "[]", 60, 10, 20, true, new TimeOnly(10, 20, 0), new TimeOnly(10, 0, 0), new TimeOnly(13, 0, 0), 0, new TimeOnly(8, 0, 0), 6, false, new TimeOnly(13, 0, 0), new TimeOnly(8, 0, 0), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$SpL7Ds0sjOHvvlqNRW7Qf.JqKPPyCeVqpdasKy9Mt8bA.5rzfXgaC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchoolSettings");

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 639, DateTimeKind.Utc).AddTicks(7377));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 639, DateTimeKind.Utc).AddTicks(9915));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 639, DateTimeKind.Utc).AddTicks(9921));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 639, DateTimeKind.Utc).AddTicks(9924));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 639, DateTimeKind.Utc).AddTicks(9926));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 639, DateTimeKind.Utc).AddTicks(9929));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 639, DateTimeKind.Utc).AddTicks(9932));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 313, DateTimeKind.Utc).AddTicks(4266));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 313, DateTimeKind.Utc).AddTicks(6252));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 313, DateTimeKind.Utc).AddTicks(6262));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 313, DateTimeKind.Utc).AddTicks(6264));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 313, DateTimeKind.Utc).AddTicks(6265));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 22, 3, 32, 46, 313, DateTimeKind.Utc).AddTicks(6266));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$eE3H1rtf.vNLf1yIl8CU1uX526ktGi1qJjUX/0R3lvHnlMVaD.Ux.");
        }
    }
}
