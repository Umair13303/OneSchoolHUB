using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddClassroomAssessmentAndExamSyllabus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseTopicId",
                table: "ExamQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssessmentResultLookups",
                columns: table => new
                {
                    AssessmentResultLookupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LabelEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LabelUr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AssessmentResultLookups", x => x.AssessmentResultLookupId);
                });

            migrationBuilder.CreateTable(
                name: "ClassAssessments",
                columns: table => new
                {
                    ClassAssessmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    CourseTopicId = table.Column<int>(type: "int", nullable: true),
                    CourseTeachingLogId = table.Column<int>(type: "int", nullable: true),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    AssessmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssessmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalMarks = table.Column<int>(type: "int", nullable: true),
                    TotalQuestions = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_ClassAssessments", x => x.ClassAssessmentId);
                    table.ForeignKey(
                        name: "FK_ClassAssessments_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "AcademicYearId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassAssessments_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassAssessments_CourseTeachingLogs_CourseTeachingLogId",
                        column: x => x.CourseTeachingLogId,
                        principalTable: "CourseTeachingLogs",
                        principalColumn: "CourseTeachingLogId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ClassAssessments_CourseTopics_CourseTopicId",
                        column: x => x.CourseTopicId,
                        principalTable: "CourseTopics",
                        principalColumn: "CourseTopicId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassAssessments_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassAssessments_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamSyllabusItems",
                columns: table => new
                {
                    ExamSyllabusItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamPaperId = table.Column<int>(type: "int", nullable: false),
                    CourseChapterId = table.Column<int>(type: "int", nullable: true),
                    CourseTopicId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_ExamSyllabusItems", x => x.ExamSyllabusItemId);
                    table.ForeignKey(
                        name: "FK_ExamSyllabusItems_CourseChapters_CourseChapterId",
                        column: x => x.CourseChapterId,
                        principalTable: "CourseChapters",
                        principalColumn: "CourseChapterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSyllabusItems_CourseTopics_CourseTopicId",
                        column: x => x.CourseTopicId,
                        principalTable: "CourseTopics",
                        principalColumn: "CourseTopicId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSyllabusItems_ExamPapers_ExamPaperId",
                        column: x => x.ExamPaperId,
                        principalTable: "ExamPapers",
                        principalColumn: "ExamPaperId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankItems",
                columns: table => new
                {
                    QuestionBankItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseTopicId = table.Column<int>(type: "int", nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Marks = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTrue = table.Column<bool>(type: "bit", nullable: true),
                    QuestionNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_QuestionBankItems", x => x.QuestionBankItemId);
                    table.ForeignKey(
                        name: "FK_QuestionBankItems_CourseTopics_CourseTopicId",
                        column: x => x.CourseTopicId,
                        principalTable: "CourseTopics",
                        principalColumn: "CourseTopicId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentTopicPerformances",
                columns: table => new
                {
                    StudentTopicPerformanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseTopicId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    PerformanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PerformanceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResultStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_StudentTopicPerformances", x => x.StudentTopicPerformanceId);
                    table.ForeignKey(
                        name: "FK_StudentTopicPerformances_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "AcademicYearId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentTopicPerformances_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentTopicPerformances_CourseTopics_CourseTopicId",
                        column: x => x.CourseTopicId,
                        principalTable: "CourseTopics",
                        principalColumn: "CourseTopicId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentTopicPerformances_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentTopicPerformances_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentTopicPerformances_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClassAssessmentResults",
                columns: table => new
                {
                    ClassAssessmentResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassAssessmentId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ObtainedMarks = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_ClassAssessmentResults", x => x.ClassAssessmentResultId);
                    table.ForeignKey(
                        name: "FK_ClassAssessmentResults_ClassAssessments_ClassAssessmentId",
                        column: x => x.ClassAssessmentId,
                        principalTable: "ClassAssessments",
                        principalColumn: "ClassAssessmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassAssessmentResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBankOptions",
                columns: table => new
                {
                    QuestionBankOptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionBankItemId = table.Column<int>(type: "int", nullable: false),
                    OptionLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_QuestionBankOptions", x => x.QuestionBankOptionId);
                    table.ForeignKey(
                        name: "FK_QuestionBankOptions_QuestionBankItems_QuestionBankItemId",
                        column: x => x.QuestionBankItemId,
                        principalTable: "QuestionBankItems",
                        principalColumn: "QuestionBankItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AssessmentResultLookups",
                columns: new[] { "AssessmentResultLookupId", "CampusId", "Code", "CreatedAt", "CreatedBy", "InstituteId", "IsActive", "IsDeleted", "LabelEn", "LabelUr", "SortOrder", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, null, "Remembered", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Remembered", "یاد ہے", 1, null, null },
                    { 2, null, "PartiallyRemembered", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Partially Remembered", "جزوی یاد ہے", 2, null, null },
                    { 3, null, "NotRemembered", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Not Remembered", "یاد نہیں", 3, null, null },
                    { 4, null, "NeedsPractice", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, false, "Needs Practice", "مزید مشق", 4, null, null }
                });

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 637, DateTimeKind.Utc).AddTicks(4446));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 637, DateTimeKind.Utc).AddTicks(5273));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 637, DateTimeKind.Utc).AddTicks(5274));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 637, DateTimeKind.Utc).AddTicks(5275));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 637, DateTimeKind.Utc).AddTicks(5276));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 637, DateTimeKind.Utc).AddTicks(5278));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 637, DateTimeKind.Utc).AddTicks(5279));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 811, DateTimeKind.Utc).AddTicks(4764));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 811, DateTimeKind.Utc).AddTicks(5839));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 811, DateTimeKind.Utc).AddTicks(5842));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 811, DateTimeKind.Utc).AddTicks(5843));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 811, DateTimeKind.Utc).AddTicks(5866));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 811, DateTimeKind.Utc).AddTicks(5867));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 811, DateTimeKind.Utc).AddTicks(5868));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 647, DateTimeKind.Utc).AddTicks(6841));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 647, DateTimeKind.Utc).AddTicks(7482));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 647, DateTimeKind.Utc).AddTicks(7486));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 647, DateTimeKind.Utc).AddTicks(7487));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 647, DateTimeKind.Utc).AddTicks(7488));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 18, 44, 57, 647, DateTimeKind.Utc).AddTicks(7488));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Yo9o3q2TdpeM45JHY4qxw.HYbQAR2a4YBuWeJyBCzJmxLdO59regS");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_CourseTopicId",
                table: "ExamQuestions",
                column: "CourseTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentResultLookups_InstituteId_Code",
                table: "AssessmentResultLookups",
                columns: new[] { "InstituteId", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssessmentResults_ClassAssessmentId_StudentId",
                table: "ClassAssessmentResults",
                columns: new[] { "ClassAssessmentId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssessmentResults_StudentId",
                table: "ClassAssessmentResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssessments_AcademicYearId",
                table: "ClassAssessments",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssessments_ClassId_AssessmentDate",
                table: "ClassAssessments",
                columns: new[] { "ClassId", "AssessmentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssessments_CourseTeachingLogId",
                table: "ClassAssessments",
                column: "CourseTeachingLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssessments_CourseTopicId",
                table: "ClassAssessments",
                column: "CourseTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssessments_SubjectId",
                table: "ClassAssessments",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssessments_TeacherId",
                table: "ClassAssessments",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSyllabusItems_CourseChapterId",
                table: "ExamSyllabusItems",
                column: "CourseChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSyllabusItems_CourseTopicId",
                table: "ExamSyllabusItems",
                column: "CourseTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSyllabusItems_ExamPaperId",
                table: "ExamSyllabusItems",
                column: "ExamPaperId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankItems_CourseTopicId",
                table: "QuestionBankItems",
                column: "CourseTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBankOptions_QuestionBankItemId",
                table: "QuestionBankOptions",
                column: "QuestionBankItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTopicPerformances_AcademicYearId",
                table: "StudentTopicPerformances",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTopicPerformances_ClassId",
                table: "StudentTopicPerformances",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTopicPerformances_CourseTopicId_PerformanceDate",
                table: "StudentTopicPerformances",
                columns: new[] { "CourseTopicId", "PerformanceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentTopicPerformances_StudentId_CourseTopicId_PerformanceDate",
                table: "StudentTopicPerformances",
                columns: new[] { "StudentId", "CourseTopicId", "PerformanceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentTopicPerformances_SubjectId",
                table: "StudentTopicPerformances",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTopicPerformances_TeacherId",
                table: "StudentTopicPerformances",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamQuestions_CourseTopics_CourseTopicId",
                table: "ExamQuestions",
                column: "CourseTopicId",
                principalTable: "CourseTopics",
                principalColumn: "CourseTopicId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamQuestions_CourseTopics_CourseTopicId",
                table: "ExamQuestions");

            migrationBuilder.DropTable(
                name: "AssessmentResultLookups");

            migrationBuilder.DropTable(
                name: "ClassAssessmentResults");

            migrationBuilder.DropTable(
                name: "ExamSyllabusItems");

            migrationBuilder.DropTable(
                name: "QuestionBankOptions");

            migrationBuilder.DropTable(
                name: "StudentTopicPerformances");

            migrationBuilder.DropTable(
                name: "ClassAssessments");

            migrationBuilder.DropTable(
                name: "QuestionBankItems");

            migrationBuilder.DropIndex(
                name: "IX_ExamQuestions_CourseTopicId",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "CourseTopicId",
                table: "ExamQuestions");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 660, DateTimeKind.Utc).AddTicks(7367));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 660, DateTimeKind.Utc).AddTicks(8540));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 660, DateTimeKind.Utc).AddTicks(8543));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 660, DateTimeKind.Utc).AddTicks(8544));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 660, DateTimeKind.Utc).AddTicks(8545));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 660, DateTimeKind.Utc).AddTicks(8547));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 660, DateTimeKind.Utc).AddTicks(8548));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 948, DateTimeKind.Utc).AddTicks(1079));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 948, DateTimeKind.Utc).AddTicks(3940));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 948, DateTimeKind.Utc).AddTicks(3948));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 948, DateTimeKind.Utc).AddTicks(3950));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 948, DateTimeKind.Utc).AddTicks(3952));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 948, DateTimeKind.Utc).AddTicks(3953));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 948, DateTimeKind.Utc).AddTicks(3955));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 684, DateTimeKind.Utc).AddTicks(4014));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 684, DateTimeKind.Utc).AddTicks(4941));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 684, DateTimeKind.Utc).AddTicks(4944));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 684, DateTimeKind.Utc).AddTicks(4945));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 684, DateTimeKind.Utc).AddTicks(4946));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 10, 2, 17, 16, 11, 684, DateTimeKind.Utc).AddTicks(4947));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$styi5/ZGt46jN5an8Xy8JOKoGQy9Uij5sqA8ENnIh39Zv.9gHxmqS");
        }
    }
}
