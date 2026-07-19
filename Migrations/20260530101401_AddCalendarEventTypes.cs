using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCalendarEventTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarEventTypes",
                columns: table => new
                {
                    CalendarEventTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEventTypes", x => x.CalendarEventTypeId);
                });

            migrationBuilder.CreateTable(
                name: "AcademicCalendarEvents",
                columns: table => new
                {
                    AcademicCalendarEventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    CalendarEventTypeId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicCalendarEvents", x => x.AcademicCalendarEventId);
                    table.ForeignKey(
                        name: "FK_AcademicCalendarEvents_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "AcademicYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcademicCalendarEvents_CalendarEventTypes_CalendarEventTypeId",
                        column: x => x.CalendarEventTypeId,
                        principalTable: "CalendarEventTypes",
                        principalColumn: "CalendarEventTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CalendarEventTypes",
                columns: new[] { "CalendarEventTypeId", "Color", "CreatedAt", "CreatedBy", "Icon", "IsDeleted", "Name", "SortOrder", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "#7c3aed", new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(1781), null, "beach_access", false, "Annual Holiday", 1, null, null },
                    { 2, "#dc2626", new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(3162), null, "flag", false, "Gazetted Holiday", 2, null, null },
                    { 3, "#d97706", new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(3165), null, "sports_soccer", false, "Sports Day", 3, null, null },
                    { 4, "#0891b2", new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(3167), null, "quiz", false, "Short Term Exam", 4, null, null },
                    { 5, "#059669", new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(3168), null, "school", false, "Final Exam", 5, null, null },
                    { 6, "#ea580c", new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(3170), null, "emoji_events", false, "Result Day", 6, null, null },
                    { 7, "#6b7280", new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(3171), null, "event", false, "Other", 7, null, null }
                });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 14, 0, 227, DateTimeKind.Utc).AddTicks(4294));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 14, 0, 227, DateTimeKind.Utc).AddTicks(5916));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 14, 0, 227, DateTimeKind.Utc).AddTicks(5921));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 14, 0, 227, DateTimeKind.Utc).AddTicks(5923));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 14, 0, 227, DateTimeKind.Utc).AddTicks(5925));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 14, 0, 227, DateTimeKind.Utc).AddTicks(5926));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 14, 0, 227, DateTimeKind.Utc).AddTicks(5928));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 900, DateTimeKind.Utc).AddTicks(4371));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 900, DateTimeKind.Utc).AddTicks(5395));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 900, DateTimeKind.Utc).AddTicks(5398));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 900, DateTimeKind.Utc).AddTicks(5400));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 900, DateTimeKind.Utc).AddTicks(5401));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 900, DateTimeKind.Utc).AddTicks(5402));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$JnrW.WnCd1TWKb/sXW5jU.XuvD/qrBk3JN5QCL0qgoghrwTxgXrqu");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendarEvents_AcademicYearId",
                table: "AcademicCalendarEvents",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicCalendarEvents_CalendarEventTypeId",
                table: "AcademicCalendarEvents",
                column: "CalendarEventTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicCalendarEvents");

            migrationBuilder.DropTable(
                name: "CalendarEventTypes");

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 27, 352, DateTimeKind.Utc).AddTicks(1564));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 27, 352, DateTimeKind.Utc).AddTicks(9115));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 27, 352, DateTimeKind.Utc).AddTicks(9133));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 27, 352, DateTimeKind.Utc).AddTicks(9136));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 27, 352, DateTimeKind.Utc).AddTicks(9138));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 27, 352, DateTimeKind.Utc).AddTicks(9140));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 27, 352, DateTimeKind.Utc).AddTicks(9142));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 26, 612, DateTimeKind.Utc).AddTicks(7226));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 26, 612, DateTimeKind.Utc).AddTicks(9209));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 26, 612, DateTimeKind.Utc).AddTicks(9215));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 26, 612, DateTimeKind.Utc).AddTicks(9217));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 26, 612, DateTimeKind.Utc).AddTicks(9219));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 29, 11, 23, 26, 612, DateTimeKind.Utc).AddTicks(9220));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$KT1P5C3gsDdIWfxyoWcDSOZllO.CY3.5tLWOVwsePuoMNpEFTEKzO");
        }
    }
}
