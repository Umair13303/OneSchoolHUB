using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "TimetableSubstitutions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "TimetableSubstitutions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "Subjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "Subjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "StudentFees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "StudentFees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "StudentDiscounts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "StudentDiscounts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "StudentClassEnrollments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "StudentClassEnrollments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "Staff",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "Staff",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "SchoolTimetables",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "SchoolTimetables",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "SchoolSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "SchoolSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "ScheduleProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "ScheduleProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "Periods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "Periods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "MenuItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "MenuItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "Homeworks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "Homeworks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "FeeTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "FeeTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "FeeStructures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "FeeStructures",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "FeePayments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "FeePayments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "ExamSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "ExamSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "ExamResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "ExamResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "ExamQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "ExamQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "ExamQuestionOptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "ExamQuestionOptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "ExamPaperSections",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "ExamPaperSections",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "ExamPapers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "ExamPapers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "DiscountPolicies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "DiscountPolicies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "ClassSubjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "ClassSubjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "Classes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "Classes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "CalendarEventTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "CalendarEventTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "Attendances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "Attendances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "AcademicYears",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "AcademicYears",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "AcademicCalendarEvents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituteId",
                table: "AcademicCalendarEvents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Institutes",
                columns: table => new
                {
                    InstituteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModuleAttendance = table.Column<bool>(type: "bit", nullable: false),
                    ModuleFees = table.Column<bool>(type: "bit", nullable: false),
                    ModuleHomework = table.Column<bool>(type: "bit", nullable: false),
                    ModuleExams = table.Column<bool>(type: "bit", nullable: false),
                    ModuleTimetable = table.Column<bool>(type: "bit", nullable: false),
                    ModuleHR = table.Column<bool>(type: "bit", nullable: false),
                    ModuleReports = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CampusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institutes", x => x.InstituteId);
                });

            migrationBuilder.CreateTable(
                name: "Campuses",
                columns: table => new
                {
                    CampusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstituteId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campuses", x => x.CampusId);
                    table.ForeignKey(
                        name: "FK_Campuses_Institutes_InstituteId",
                        column: x => x.InstituteId,
                        principalTable: "Institutes",
                        principalColumn: "InstituteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(6705), null });

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(9223), null });

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(9228), null });

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(9232), null });

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(9235), null });

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(9238), null });

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 396, DateTimeKind.Utc).AddTicks(9242), null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 1,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 2,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 3,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 4,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 5,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 6,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 7,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 8,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 9,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 10,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 11,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 12,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 13,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 14,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 15,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 16,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 17,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 18,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 19,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 20,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 21,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 22,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 23,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 24,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 25,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 26,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 27,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 31,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 32,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 33,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 34,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 35,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 36,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 37,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 38,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 39,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(3169), null });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(6302), null });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(6308), null });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(6310), null });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(6311), null });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(6313), null });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                columns: new[] { "CampusId", "CreatedAt", "InstituteId" },
                values: new object[] { null, new DateTime(2026, 6, 13, 10, 20, 10, 982, DateTimeKind.Utc).AddTicks(6315), null });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 435, DateTimeKind.Utc).AddTicks(8413));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 436, DateTimeKind.Utc).AddTicks(145));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 436, DateTimeKind.Utc).AddTicks(149));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 436, DateTimeKind.Utc).AddTicks(151));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 436, DateTimeKind.Utc).AddTicks(153));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 13, 10, 20, 10, 436, DateTimeKind.Utc).AddTicks(155));

            migrationBuilder.UpdateData(
                table: "SchoolSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CampusId", "InstituteId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CampusId", "InstituteId", "PasswordHash" },
                values: new object[] { null, null, "$2a$11$7.JgRSSChZY9HTLj8/28a.ykjV4Ul4RjskdSA3bMCtJuXDayJom9S" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CampusId",
                table: "Users",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_InstituteId",
                table: "Users",
                column: "InstituteId");

            migrationBuilder.CreateIndex(
                name: "IX_Campuses_InstituteId",
                table: "Campuses",
                column: "InstituteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Campuses_CampusId",
                table: "Users",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "CampusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Institutes_InstituteId",
                table: "Users",
                column: "InstituteId",
                principalTable: "Institutes",
                principalColumn: "InstituteId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Campuses_CampusId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Institutes_InstituteId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Campuses");

            migrationBuilder.DropTable(
                name: "Institutes");

            migrationBuilder.DropIndex(
                name: "IX_Users_CampusId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_InstituteId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "TimetableSubstitutions");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "TimetableSubstitutions");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "StudentFees");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "StudentFees");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "StudentDiscounts");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "StudentDiscounts");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "StudentClassEnrollments");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "StudentClassEnrollments");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "SchoolTimetables");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "SchoolTimetables");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "SchoolSettings");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "ScheduleProfiles");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "ScheduleProfiles");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Periods");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "Periods");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Homeworks");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "Homeworks");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "FeeTypes");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "FeeTypes");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "FeeStructures");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "FeeStructures");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "FeePayments");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "FeePayments");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "ExamSchedules");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "ExamSchedules");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "ExamQuestionOptions");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "ExamQuestionOptions");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "ExamPaperSections");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "ExamPaperSections");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "ExamPapers");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "ExamPapers");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "DiscountPolicies");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "DiscountPolicies");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "ClassSubjects");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "CalendarEventTypes");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "CalendarEventTypes");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "AcademicCalendarEvents");

            migrationBuilder.DropColumn(
                name: "InstituteId",
                table: "AcademicCalendarEvents");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 756, DateTimeKind.Utc).AddTicks(4666));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 756, DateTimeKind.Utc).AddTicks(7606));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 756, DateTimeKind.Utc).AddTicks(7614));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 756, DateTimeKind.Utc).AddTicks(7618));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 756, DateTimeKind.Utc).AddTicks(7620));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 756, DateTimeKind.Utc).AddTicks(7624));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 756, DateTimeKind.Utc).AddTicks(7627));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 5, 110, DateTimeKind.Utc).AddTicks(4379));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 5, 110, DateTimeKind.Utc).AddTicks(6970));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 5, 110, DateTimeKind.Utc).AddTicks(6974));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 5, 110, DateTimeKind.Utc).AddTicks(6976));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 5, 110, DateTimeKind.Utc).AddTicks(6977));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 5, 110, DateTimeKind.Utc).AddTicks(6979));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 5, 110, DateTimeKind.Utc).AddTicks(6980));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 787, DateTimeKind.Utc).AddTicks(9144));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 788, DateTimeKind.Utc).AddTicks(277));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 788, DateTimeKind.Utc).AddTicks(280));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 788, DateTimeKind.Utc).AddTicks(282));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 788, DateTimeKind.Utc).AddTicks(284));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 11, 17, 54, 4, 788, DateTimeKind.Utc).AddTicks(285));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$FSi5lL21PFV3aWH26fH5Fe9TnHUJbABa2u1L8jjPuUojy/sL0Iq2u");
        }
    }
}
