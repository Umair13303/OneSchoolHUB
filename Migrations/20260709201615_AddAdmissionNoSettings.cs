using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmissionNoSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdmissionNoIncludeYear",
                table: "SchoolSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AdmissionNoPadding",
                table: "SchoolSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionNoPrefix",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
                table: "SchoolSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AdmissionNoIncludeYear", "AdmissionNoPadding", "AdmissionNoPrefix" },
                values: new object[] { true, 4, "ADM" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$cv.r5RBXjKT2dwEva4m8pOstOPf3/P69s7sPphwzxWdMsc0eoyewS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdmissionNoIncludeYear",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionNoPadding",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "AdmissionNoPrefix",
                table: "SchoolSettings");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(2699));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(4537));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(4569));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(4571));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(4573));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(4574));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(4576));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(507));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(2320));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(2350));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(2352));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(2353));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(2355));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(2357));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 799, DateTimeKind.Utc).AddTicks(8613));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 800, DateTimeKind.Utc).AddTicks(209));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 800, DateTimeKind.Utc).AddTicks(214));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 800, DateTimeKind.Utc).AddTicks(215));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 800, DateTimeKind.Utc).AddTicks(250));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 800, DateTimeKind.Utc).AddTicks(252));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$sGhtzhmmhJQJJsHU.0x2e.S5sg7WBJryOJlGN7Ojwb853Q6iAkWzm");
        }
    }
}
