using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddExamModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── ExamPapers ────────────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "ExamPapers",
                columns: table => new
                {
                    ExamPaperId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId  = table.Column<int>(type: "int", nullable: false),
                    ClassId         = table.Column<int>(type: "int", nullable: false),
                    SubjectId       = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    ExamType        = table.Column<int>(type: "int", nullable: false),
                    ClassGroup      = table.Column<int>(type: "int", nullable: false),
                    Title           = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TotalMarks      = table.Column<int>(type: "int", nullable: false),
                    PassMarks       = table.Column<int>(type: "int", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true),
                    Instructions    = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SyllabusNote    = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDraft         = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked        = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted       = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy       = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy       = table.Column<int>(type: "int", nullable: true),
                    CreatedAt       = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt       = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamPapers", x => x.ExamPaperId);
                    table.ForeignKey(
                        name: "FK_ExamPapers_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "AcademicYearId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamPapers_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamPapers_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamPapers_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamPapers_AcademicYearId",
                table: "ExamPapers",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPapers_ClassId",
                table: "ExamPapers",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPapers_SubjectId",
                table: "ExamPapers",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPapers_CreatedByUserId",
                table: "ExamPapers",
                column: "CreatedByUserId");

            // ── ExamPaperSections ─────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "ExamPaperSections",
                columns: table => new
                {
                    ExamPaperSectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamPaperId      = table.Column<int>(type: "int", nullable: false),
                    SectionName      = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SectionType      = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AllocatedMarks   = table.Column<int>(type: "int", nullable: false),
                    TotalQuestions   = table.Column<int>(type: "int", nullable: true),
                    AttemptQuestions = table.Column<int>(type: "int", nullable: true),
                    MarksPerQuestion = table.Column<int>(type: "int", nullable: true),
                    SectionNote      = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SortOrder        = table.Column<int>(type: "int", nullable: false),
                    IsDeleted        = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy        = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy        = table.Column<int>(type: "int", nullable: true),
                    CreatedAt        = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt        = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamPaperSections", x => x.ExamPaperSectionId);
                    table.ForeignKey(
                        name: "FK_ExamPaperSections_ExamPapers_ExamPaperId",
                        column: x => x.ExamPaperId,
                        principalTable: "ExamPapers",
                        principalColumn: "ExamPaperId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamPaperSections_ExamPaperId",
                table: "ExamPaperSections",
                column: "ExamPaperId");

            // ── ExamSchedules ─────────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "ExamSchedules",
                columns: table => new
                {
                    ExamScheduleId    = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamPaperId       = table.Column<int>(type: "int", nullable: false),
                    ExamDate          = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime         = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime           = table.Column<TimeOnly>(type: "time", nullable: true),
                    RoomOrHall        = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InvigilatorUserId = table.Column<int>(type: "int", nullable: true),
                    Status            = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Remarks           = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted         = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy         = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy         = table.Column<int>(type: "int", nullable: true),
                    CreatedAt         = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt         = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSchedules", x => x.ExamScheduleId);
                    table.ForeignKey(
                        name: "FK_ExamSchedules_ExamPapers_ExamPaperId",
                        column: x => x.ExamPaperId,
                        principalTable: "ExamPapers",
                        principalColumn: "ExamPaperId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamSchedules_Users_InvigilatorUserId",
                        column: x => x.InvigilatorUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamSchedules_ExamPaperId",
                table: "ExamSchedules",
                column: "ExamPaperId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSchedules_InvigilatorUserId",
                table: "ExamSchedules",
                column: "InvigilatorUserId");

            // ── ExamResults ───────────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "ExamResults",
                columns: table => new
                {
                    ExamResultId     = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamPaperId      = table.Column<int>(type: "int", nullable: false),
                    StudentId        = table.Column<int>(type: "int", nullable: false),
                    ObtainedMarks    = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    SectionMarksJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAbsent         = table.Column<bool>(type: "bit", nullable: false),
                    IsPass           = table.Column<bool>(type: "bit", nullable: false),
                    Grade            = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Percentage       = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    ClassRank        = table.Column<int>(type: "int", nullable: true),
                    Remarks          = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EnteredByUserId  = table.Column<int>(type: "int", nullable: true),
                    EnteredAt        = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted        = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy        = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy        = table.Column<int>(type: "int", nullable: true),
                    CreatedAt        = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt        = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamResults", x => x.ExamResultId);
                    table.ForeignKey(
                        name: "FK_ExamResults_ExamPapers_ExamPaperId",
                        column: x => x.ExamPaperId,
                        principalTable: "ExamPapers",
                        principalColumn: "ExamPaperId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamResults_Users_EnteredByUserId",
                        column: x => x.EnteredByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_ExamPaperId_StudentId",
                table: "ExamResults",
                columns: new[] { "ExamPaperId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_StudentId",
                table: "ExamResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_EnteredByUserId",
                table: "ExamResults",
                column: "EnteredByUserId");

            // ── Exam menu items ───────────────────────────────────────────────────
            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT [MenuItems] ON;
                INSERT INTO [MenuItems] ([MenuItemId],[ParentId],[Title],[Icon],[RouteUrl],[SortOrder],[IsActive],[IsDeleted],[CreatedAt],[CreatedBy],[UpdatedBy],[UpdatedAt])
                VALUES
                (34, NULL, 'Exams',        'quiz',           '/exams',               65, 1, 0, '2025-01-01', NULL, NULL, NULL),
                (35, 34,   'Paper Setup',  'edit_document',  '/exams/papers',        66, 1, 0, '2025-01-01', NULL, NULL, NULL),
                (36, 34,   'Exam Schedule','calendar_month', '/exams/schedule',      67, 1, 0, '2025-01-01', NULL, NULL, NULL),
                (37, 34,   'Enter Results','grading',        '/exams/results/enter', 68, 1, 0, '2025-01-01', NULL, NULL, NULL),
                (38, 34,   'Result Cards', 'emoji_events',   '/exams/results/cards', 69, 1, 0, '2025-01-01', NULL, NULL, NULL);
                SET IDENTITY_INSERT [MenuItems] OFF;
            ");

            // ── Exam menu role permissions ─────────────────────────────────────────
            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT [MenuRolePermissions] ON;
                INSERT INTO [MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES
                (104,34,1),(105,34,2),(106,34,3),(107,34,4),(108,34,5),
                (109,35,1),(110,35,2),(111,35,3),(112,35,4),
                (113,36,1),(114,36,2),(115,36,3),(116,36,4),(117,36,5),
                (118,37,1),(119,37,2),(120,37,3),(121,37,4),
                (122,38,1),(123,38,2),(124,38,3),(125,38,4),(126,38,5);
                SET IDENTITY_INSERT [MenuRolePermissions] OFF;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove menu permissions
            migrationBuilder.DeleteData("MenuRolePermissions", "Id", new object[] { 104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126 });
            // Remove menu items (children first)
            migrationBuilder.DeleteData("MenuItems", "MenuItemId", new object[] { 35,36,37,38 });
            migrationBuilder.DeleteData("MenuItems", "MenuItemId", new object[] { 34 });

            migrationBuilder.DropTable(name: "ExamResults");
            migrationBuilder.DropTable(name: "ExamSchedules");
            migrationBuilder.DropTable(name: "ExamPaperSections");
            migrationBuilder.DropTable(name: "ExamPapers");
        }
    }
}
