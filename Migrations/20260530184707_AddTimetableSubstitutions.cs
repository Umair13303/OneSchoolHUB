using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTimetableSubstitutions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimetableSubstitutions",
                columns: table => new
                {
                    SubstitutionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimetableId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    SubstituteTeacherId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableSubstitutions", x => x.SubstitutionId);
                    table.ForeignKey(
                        name: "FK_TimetableSubstitutions_SchoolTimetables_TimetableId",
                        column: x => x.TimetableId,
                        principalTable: "SchoolTimetables",
                        principalColumn: "TimetableId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableSubstitutions_Users_SubstituteTeacherId",
                        column: x => x.SubstituteTeacherId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 210, DateTimeKind.Utc).AddTicks(5529));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 210, DateTimeKind.Utc).AddTicks(6992));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 210, DateTimeKind.Utc).AddTicks(6995));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 210, DateTimeKind.Utc).AddTicks(6996));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 210, DateTimeKind.Utc).AddTicks(6998));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 210, DateTimeKind.Utc).AddTicks(6999));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 210, DateTimeKind.Utc).AddTicks(7000));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 489, DateTimeKind.Utc).AddTicks(2131));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 489, DateTimeKind.Utc).AddTicks(3690));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 489, DateTimeKind.Utc).AddTicks(3694));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 489, DateTimeKind.Utc).AddTicks(3696));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 489, DateTimeKind.Utc).AddTicks(3698));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 489, DateTimeKind.Utc).AddTicks(3699));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 489, DateTimeKind.Utc).AddTicks(3701));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 234, DateTimeKind.Utc).AddTicks(2929));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 234, DateTimeKind.Utc).AddTicks(3948));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 234, DateTimeKind.Utc).AddTicks(3951));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 234, DateTimeKind.Utc).AddTicks(3953));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 234, DateTimeKind.Utc).AddTicks(3954));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 18, 47, 5, 234, DateTimeKind.Utc).AddTicks(3955));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$CQ6P9G4sggXlPI61WpxwEeT0Nwr663a5LoXei3PpXTOeiFZGBv/zO");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableSubstitutions_SubstituteTeacherId",
                table: "TimetableSubstitutions",
                column: "SubstituteTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableSubstitutions_TimetableId_Date",
                table: "TimetableSubstitutions",
                columns: new[] { "TimetableId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimetableSubstitutions");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(1781));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(3162));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(3165));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(3167));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(3168));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(3170));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 30, 10, 13, 59, 881, DateTimeKind.Utc).AddTicks(3171));

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
        }
    }
}
