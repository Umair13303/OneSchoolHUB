using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkingDaysAndBreakAfterPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FridayBreakAfterPeriod",
                table: "SchoolSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RegularBreakAfterPeriod",
                table: "SchoolSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WorkingDaysJson",
                table: "SchoolSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 24, 86, DateTimeKind.Utc).AddTicks(1846));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 24, 86, DateTimeKind.Utc).AddTicks(3481));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 24, 86, DateTimeKind.Utc).AddTicks(3484));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 24, 86, DateTimeKind.Utc).AddTicks(3486));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 24, 86, DateTimeKind.Utc).AddTicks(3487));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 24, 86, DateTimeKind.Utc).AddTicks(3489));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 24, 86, DateTimeKind.Utc).AddTicks(3491));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 23, 687, DateTimeKind.Utc).AddTicks(1396));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 23, 687, DateTimeKind.Utc).AddTicks(2539));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 23, 687, DateTimeKind.Utc).AddTicks(2543));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 23, 687, DateTimeKind.Utc).AddTicks(2544));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 23, 687, DateTimeKind.Utc).AddTicks(2545));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 7, 56, 23, 687, DateTimeKind.Utc).AddTicks(2547));

            migrationBuilder.UpdateData(
                table: "SchoolSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FridayBreakAfterPeriod", "RegularBreakAfterPeriod", "WorkingDaysJson" },
                values: new object[] { 2, 3, "[1,2,3,4,6]" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$iZftEtWSm76/694Nff4b1OkvK8Kys3zLfWlS2NYbBWwe6vhB0A/mW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FridayBreakAfterPeriod",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "RegularBreakAfterPeriod",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "WorkingDaysJson",
                table: "SchoolSettings");

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 52, 90, DateTimeKind.Utc).AddTicks(8068));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 52, 91, DateTimeKind.Utc).AddTicks(1011));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 52, 91, DateTimeKind.Utc).AddTicks(1020));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 52, 91, DateTimeKind.Utc).AddTicks(1024));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 52, 91, DateTimeKind.Utc).AddTicks(1026));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 52, 91, DateTimeKind.Utc).AddTicks(1028));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 52, 91, DateTimeKind.Utc).AddTicks(1032));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 51, 742, DateTimeKind.Utc).AddTicks(9851));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 51, 743, DateTimeKind.Utc).AddTicks(1704));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 51, 743, DateTimeKind.Utc).AddTicks(1710));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 51, 743, DateTimeKind.Utc).AddTicks(1712));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 51, 743, DateTimeKind.Utc).AddTicks(1716));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 28, 18, 55, 51, 743, DateTimeKind.Utc).AddTicks(1719));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Z.axpTf6NS4JdKfnywW0X.EhpZwUm/kELo5kU5cnCtYFhjoT.hFcy");
        }
    }
}
