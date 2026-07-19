using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 712, DateTimeKind.Utc).AddTicks(7323));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 712, DateTimeKind.Utc).AddTicks(9669));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 712, DateTimeKind.Utc).AddTicks(9674));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 712, DateTimeKind.Utc).AddTicks(9677));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 712, DateTimeKind.Utc).AddTicks(9679));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 712, DateTimeKind.Utc).AddTicks(9681));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 712, DateTimeKind.Utc).AddTicks(9683));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 433, DateTimeKind.Utc).AddTicks(2046));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 433, DateTimeKind.Utc).AddTicks(4484));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 433, DateTimeKind.Utc).AddTicks(4495));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 433, DateTimeKind.Utc).AddTicks(4496));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 433, DateTimeKind.Utc).AddTicks(4498));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 19, 15, 10, 38, 433, DateTimeKind.Utc).AddTicks(4499));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Password", "PasswordHash" },
                values: new object[] { "Admin@123", "$2a$11$c8vMKXpVK.XIuWs4vlxvc.DswSQsJyfX7mGp7y7qGcggefQ7/3WbW" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 539, DateTimeKind.Utc).AddTicks(1469));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 539, DateTimeKind.Utc).AddTicks(3069));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 539, DateTimeKind.Utc).AddTicks(3073));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 539, DateTimeKind.Utc).AddTicks(3075));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 539, DateTimeKind.Utc).AddTicks(3077));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 539, DateTimeKind.Utc).AddTicks(3078));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 539, DateTimeKind.Utc).AddTicks(3080));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 224, DateTimeKind.Utc).AddTicks(3862));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 224, DateTimeKind.Utc).AddTicks(4962));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 224, DateTimeKind.Utc).AddTicks(4966));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 224, DateTimeKind.Utc).AddTicks(4968));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 224, DateTimeKind.Utc).AddTicks(4969));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 18, 21, 15, 40, 224, DateTimeKind.Utc).AddTicks(4970));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$m8wwRHK2G.pwSEn1C42Y9.O.fWOyD4IGo9dMluIOr84ZzB7V6qB.m");
        }
    }
}
