using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseTopicActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseTopicActivities",
                columns: table => new
                {
                    CourseTopicActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseTopicId = table.Column<int>(type: "int", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructionText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceImageFileId = table.Column<int>(type: "int", nullable: true),
                    ExampleImageFileId = table.Column<int>(type: "int", nullable: true),
                    WorksheetFileId = table.Column<int>(type: "int", nullable: true),
                    ConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CanvasEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstituteId = table.Column<int>(type: "int", nullable: true),
                    CampusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTopicActivities", x => x.CourseTopicActivityId);
                    table.ForeignKey(
                        name: "FK_CourseTopicActivities_CourseTopics_CourseTopicId",
                        column: x => x.CourseTopicId,
                        principalTable: "CourseTopics",
                        principalColumn: "CourseTopicId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseTopicActivities_FileStores_ExampleImageFileId",
                        column: x => x.ExampleImageFileId,
                        principalTable: "FileStores",
                        principalColumn: "FileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseTopicActivities_FileStores_ReferenceImageFileId",
                        column: x => x.ReferenceImageFileId,
                        principalTable: "FileStores",
                        principalColumn: "FileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseTopicActivities_FileStores_WorksheetFileId",
                        column: x => x.WorksheetFileId,
                        principalTable: "FileStores",
                        principalColumn: "FileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 542, DateTimeKind.Utc).AddTicks(7377));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 543, DateTimeKind.Utc).AddTicks(423));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 543, DateTimeKind.Utc).AddTicks(428));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 543, DateTimeKind.Utc).AddTicks(430));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 543, DateTimeKind.Utc).AddTicks(432));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 543, DateTimeKind.Utc).AddTicks(434));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 543, DateTimeKind.Utc).AddTicks(435));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 903, DateTimeKind.Utc).AddTicks(8678));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 904, DateTimeKind.Utc).AddTicks(1499));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 904, DateTimeKind.Utc).AddTicks(1508));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 904, DateTimeKind.Utc).AddTicks(1510));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 904, DateTimeKind.Utc).AddTicks(1513));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 904, DateTimeKind.Utc).AddTicks(1515));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 904, DateTimeKind.Utc).AddTicks(1517));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 565, DateTimeKind.Utc).AddTicks(807));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 565, DateTimeKind.Utc).AddTicks(2488));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 565, DateTimeKind.Utc).AddTicks(2494));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 565, DateTimeKind.Utc).AddTicks(2496));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 565, DateTimeKind.Utc).AddTicks(2498));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 41, 33, 565, DateTimeKind.Utc).AddTicks(2500));

            migrationBuilder.CreateIndex(
                name: "IX_CourseTopicActivities_CourseTopicId_SortOrder",
                table: "CourseTopicActivities",
                columns: new[] { "CourseTopicId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseTopicActivities_ExampleImageFileId",
                table: "CourseTopicActivities",
                column: "ExampleImageFileId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTopicActivities_ReferenceImageFileId",
                table: "CourseTopicActivities",
                column: "ReferenceImageFileId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTopicActivities_WorksheetFileId",
                table: "CourseTopicActivities",
                column: "WorksheetFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseTopicActivities");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(1653));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(4028));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(4036));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(4038));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(4040));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(4042));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 957, DateTimeKind.Utc).AddTicks(4044));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(3447));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(6254));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(6261));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(6263));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(6265));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(6267));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 10, 291, DateTimeKind.Utc).AddTicks(6268));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 978, DateTimeKind.Utc).AddTicks(4533));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 978, DateTimeKind.Utc).AddTicks(5536));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 978, DateTimeKind.Utc).AddTicks(5540));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 978, DateTimeKind.Utc).AddTicks(5542));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 978, DateTimeKind.Utc).AddTicks(5544));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 31, 9, 978, DateTimeKind.Utc).AddTicks(5545));

        }
    }
}
