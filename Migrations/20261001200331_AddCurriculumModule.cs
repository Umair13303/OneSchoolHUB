using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCurriculumModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ModuleCurriculum",
                table: "Institutes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseTopicId",
                table: "Homeworks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CoursePlans",
                columns: table => new
                {
                    CoursePlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_CoursePlans", x => x.CoursePlanId);
                    table.ForeignKey(
                        name: "FK_CoursePlans_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "AcademicYearId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoursePlans_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoursePlans_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseChapters",
                columns: table => new
                {
                    CourseChapterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoursePlanId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CourseChapters", x => x.CourseChapterId);
                    table.ForeignKey(
                        name: "FK_CourseChapters_CoursePlans_CoursePlanId",
                        column: x => x.CoursePlanId,
                        principalTable: "CoursePlans",
                        principalColumn: "CoursePlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseTopics",
                columns: table => new
                {
                    CourseTopicId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseChapterId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CourseTopics", x => x.CourseTopicId);
                    table.ForeignKey(
                        name: "FK_CourseTopics_CourseChapters_CourseChapterId",
                        column: x => x.CourseChapterId,
                        principalTable: "CourseChapters",
                        principalColumn: "CourseChapterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseMaterials",
                columns: table => new
                {
                    CourseMaterialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseChapterId = table.Column<int>(type: "int", nullable: true),
                    CourseTopicId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileStoreId = table.Column<int>(type: "int", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CourseMaterials", x => x.CourseMaterialId);
                    table.ForeignKey(
                        name: "FK_CourseMaterials_CourseChapters_CourseChapterId",
                        column: x => x.CourseChapterId,
                        principalTable: "CourseChapters",
                        principalColumn: "CourseChapterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseMaterials_CourseTopics_CourseTopicId",
                        column: x => x.CourseTopicId,
                        principalTable: "CourseTopics",
                        principalColumn: "CourseTopicId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseMaterials_FileStores_FileStoreId",
                        column: x => x.FileStoreId,
                        principalTable: "FileStores",
                        principalColumn: "FileId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CourseTeachingLogs",
                columns: table => new
                {
                    CourseTeachingLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeachingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    CourseTopicId = table.Column<int>(type: "int", nullable: false),
                    TeachingType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtraNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeworkId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_CourseTeachingLogs", x => x.CourseTeachingLogId);
                    table.ForeignKey(
                        name: "FK_CourseTeachingLogs_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "AcademicYearId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseTeachingLogs_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseTeachingLogs_CourseTopics_CourseTopicId",
                        column: x => x.CourseTopicId,
                        principalTable: "CourseTopics",
                        principalColumn: "CourseTopicId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseTeachingLogs_Homeworks_HomeworkId",
                        column: x => x.HomeworkId,
                        principalTable: "Homeworks",
                        principalColumn: "HomeworkId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CourseTeachingLogs_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseTeachingLogs_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseTopicProgresses",
                columns: table => new
                {
                    CourseTopicProgressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseTopicId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedByTeacherId = table.Column<int>(type: "int", nullable: true),
                    LastUpdatedByTeacherId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_CourseTopicProgresses", x => x.CourseTopicProgressId);
                    table.ForeignKey(
                        name: "FK_CourseTopicProgresses_CourseTopics_CourseTopicId",
                        column: x => x.CourseTopicId,
                        principalTable: "CourseTopics",
                        principalColumn: "CourseTopicId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseTopicProgresses_Users_CompletedByTeacherId",
                        column: x => x.CompletedByTeacherId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseTopicProgresses_Users_LastUpdatedByTeacherId",
                        column: x => x.LastUpdatedByTeacherId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 79, DateTimeKind.Utc).AddTicks(9133));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 80, DateTimeKind.Utc).AddTicks(205));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 80, DateTimeKind.Utc).AddTicks(207));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 80, DateTimeKind.Utc).AddTicks(208));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 80, DateTimeKind.Utc).AddTicks(210));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 80, DateTimeKind.Utc).AddTicks(211));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 80, DateTimeKind.Utc).AddTicks(212));

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "MenuItemId", "CampusId", "CreatedAt", "CreatedBy", "Icon", "InstituteId", "IsActive", "IsDeleted", "ParentId", "RouteUrl", "SortOrder", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 54, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "auto_stories", null, true, false, null, null, 55, "Curriculum", null, null });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(7891));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(9343));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(9348));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(9349));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(9351));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(9376));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 316, DateTimeKind.Utc).AddTicks(9378));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 89, DateTimeKind.Utc).AddTicks(7620));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 89, DateTimeKind.Utc).AddTicks(8407));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 89, DateTimeKind.Utc).AddTicks(8410));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 89, DateTimeKind.Utc).AddTicks(8411));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 89, DateTimeKind.Utc).AddTicks(8412));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 20, 3, 29, 89, DateTimeKind.Utc).AddTicks(8413));

            // Intentionally skip PasswordHash seed updates (would reset superadmin password).

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "MenuItemId", "CampusId", "CreatedAt", "CreatedBy", "Icon", "InstituteId", "IsActive", "IsDeleted", "ParentId", "RouteUrl", "SortOrder", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 55, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "library_books", null, true, false, 54, "/curriculum/plans", 56, "Course Plans", null, null },
                    { 56, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "monitoring", null, true, false, 54, "/curriculum/progress", 57, "Curriculum Progress", null, null },
                    { 57, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "menu_book", null, true, false, 54, "/curriculum/my-courses", 58, "My Courses", null, null }
                });

            migrationBuilder.InsertData(
                table: "MenuRolePermissions",
                columns: new[] { "Id", "MenuItemId", "RoleId" },
                values: new object[,]
                {
                    { 203, 54, 2 },
                    { 204, 54, 3 },
                    { 205, 54, 4 },
                    { 206, 55, 2 },
                    { 207, 55, 3 },
                    { 208, 56, 2 },
                    { 209, 56, 3 },
                    { 210, 57, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Homeworks_CourseTopicId",
                table: "Homeworks",
                column: "CourseTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseChapters_CoursePlanId",
                table: "CourseChapters",
                column: "CoursePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMaterials_CourseChapterId",
                table: "CourseMaterials",
                column: "CourseChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMaterials_CourseTopicId",
                table: "CourseMaterials",
                column: "CourseTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMaterials_FileStoreId",
                table: "CourseMaterials",
                column: "FileStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePlans_AcademicYearId",
                table: "CoursePlans",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePlans_ClassId",
                table: "CoursePlans",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePlans_InstituteId_AcademicYearId_ClassId_SubjectId",
                table: "CoursePlans",
                columns: new[] { "InstituteId", "AcademicYearId", "ClassId", "SubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_CoursePlans_SubjectId",
                table: "CoursePlans",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachingLogs_AcademicYearId",
                table: "CourseTeachingLogs",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachingLogs_ClassId_TeachingDate",
                table: "CourseTeachingLogs",
                columns: new[] { "ClassId", "TeachingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachingLogs_CourseTopicId",
                table: "CourseTeachingLogs",
                column: "CourseTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachingLogs_HomeworkId",
                table: "CourseTeachingLogs",
                column: "HomeworkId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachingLogs_SubjectId",
                table: "CourseTeachingLogs",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachingLogs_TeacherId",
                table: "CourseTeachingLogs",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTopicProgresses_CompletedByTeacherId",
                table: "CourseTopicProgresses",
                column: "CompletedByTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTopicProgresses_CourseTopicId",
                table: "CourseTopicProgresses",
                column: "CourseTopicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseTopicProgresses_LastUpdatedByTeacherId",
                table: "CourseTopicProgresses",
                column: "LastUpdatedByTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTopics_CourseChapterId",
                table: "CourseTopics",
                column: "CourseChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Homeworks_CourseTopics_CourseTopicId",
                table: "Homeworks",
                column: "CourseTopicId",
                principalTable: "CourseTopics",
                principalColumn: "CourseTopicId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Homeworks_CourseTopics_CourseTopicId",
                table: "Homeworks");

            migrationBuilder.DropTable(
                name: "CourseMaterials");

            migrationBuilder.DropTable(
                name: "CourseTeachingLogs");

            migrationBuilder.DropTable(
                name: "CourseTopicProgresses");

            migrationBuilder.DropTable(
                name: "CourseTopics");

            migrationBuilder.DropTable(
                name: "CourseChapters");

            migrationBuilder.DropTable(
                name: "CoursePlans");

            migrationBuilder.DropIndex(
                name: "IX_Homeworks_CourseTopicId",
                table: "Homeworks");

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 207);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 208);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 210);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 54);

            migrationBuilder.DropColumn(
                name: "ModuleCurriculum",
                table: "Institutes");

            migrationBuilder.DropColumn(
                name: "CourseTopicId",
                table: "Homeworks");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 6, DateTimeKind.Utc).AddTicks(969));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 6, DateTimeKind.Utc).AddTicks(2229));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 6, DateTimeKind.Utc).AddTicks(2231));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 6, DateTimeKind.Utc).AddTicks(2233));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 6, DateTimeKind.Utc).AddTicks(2234));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 6, DateTimeKind.Utc).AddTicks(2236));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 6, DateTimeKind.Utc).AddTicks(2237));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 333, DateTimeKind.Utc).AddTicks(682));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 333, DateTimeKind.Utc).AddTicks(2672));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 333, DateTimeKind.Utc).AddTicks(2677));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 333, DateTimeKind.Utc).AddTicks(2679));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 333, DateTimeKind.Utc).AddTicks(2681));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 333, DateTimeKind.Utc).AddTicks(2682));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 333, DateTimeKind.Utc).AddTicks(2713));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 18, DateTimeKind.Utc).AddTicks(7446));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 18, DateTimeKind.Utc).AddTicks(8445));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 18, DateTimeKind.Utc).AddTicks(8449));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 18, DateTimeKind.Utc).AddTicks(8465));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 18, DateTimeKind.Utc).AddTicks(8466));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 1, 19, 14, 13, 18, DateTimeKind.Utc).AddTicks(8467));

            // Intentionally skip PasswordHash seed rollback.
        }
    }
}
