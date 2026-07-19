using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clear orphaned DaySchedule rows before adding FK
            migrationBuilder.Sql("DELETE FROM DaySchedules");

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                table: "DaySchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ScheduleProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleProfiles", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 988, DateTimeKind.Utc).AddTicks(7504));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 989, DateTimeKind.Utc).AddTicks(349));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 989, DateTimeKind.Utc).AddTicks(355));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 989, DateTimeKind.Utc).AddTicks(358));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 989, DateTimeKind.Utc).AddTicks(360));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 989, DateTimeKind.Utc).AddTicks(362));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 989, DateTimeKind.Utc).AddTicks(364));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 506, DateTimeKind.Utc).AddTicks(8779));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 506, DateTimeKind.Utc).AddTicks(9940));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 506, DateTimeKind.Utc).AddTicks(9945));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 506, DateTimeKind.Utc).AddTicks(9946));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 506, DateTimeKind.Utc).AddTicks(9947));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 10, 19, 15, 506, DateTimeKind.Utc).AddTicks(9975));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$QkteDNeW9wl/n6EUosaQ1ebo3Xsax3LaWNSFANS5QngTbfG8QoqPW");

            migrationBuilder.CreateIndex(
                name: "IX_DaySchedules_ProfileId",
                table: "DaySchedules",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_DaySchedules_ScheduleProfiles_ProfileId",
                table: "DaySchedules",
                column: "ProfileId",
                principalTable: "ScheduleProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DaySchedules_ScheduleProfiles_ProfileId",
                table: "DaySchedules");

            migrationBuilder.DropTable(
                name: "ScheduleProfiles");

            migrationBuilder.DropIndex(
                name: "IX_DaySchedules_ProfileId",
                table: "DaySchedules");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "DaySchedules");

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
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$.4z19imECcsRkW89z5JjsOBnMVckNAjCDm313d278xRqVZ8tcebSi");
        }
    }
}
