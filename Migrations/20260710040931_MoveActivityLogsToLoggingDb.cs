using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class MoveActivityLogsToLoggingDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ActivityLogs moved to the separate logging database (LoggingDbContext).
            // Rename instead of drop so existing rows survive until they are copied:
            //   INSERT INTO SchoolLogsDB.dbo.ActivityLogs (UserId, UserName, UserEmail, UserRole, Action,
            //          EntityName, EntityId, OldValues, NewValues, IpAddress, UserAgent, Timestamp, InstituteId)
            //   SELECT UserId, UserName, UserEmail, UserRole, Action, EntityName, EntityId,
            //          OldValues, NewValues, IpAddress, UserAgent, Timestamp, InstituteId
            //   FROM   SchoolDB.dbo.ActivityLogs_Backup ORDER BY Id;
            // After verifying the copy: DROP TABLE ActivityLogs_Backup;
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'ActivityLogs', N'U') IS NOT NULL AND OBJECT_ID(N'ActivityLogs_Backup', N'U') IS NULL
    EXEC sp_rename 'ActivityLogs', 'ActivityLogs_Backup';");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(2211));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(3477));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(3481));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(3483));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(3484));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(3486));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 420, DateTimeKind.Utc).AddTicks(3487));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(3592));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(6209));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(6220));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(6222));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(6224));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(6227));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 723, DateTimeKind.Utc).AddTicks(6229));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 436, DateTimeKind.Utc).AddTicks(8233));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 436, DateTimeKind.Utc).AddTicks(9239));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 436, DateTimeKind.Utc).AddTicks(9243));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 436, DateTimeKind.Utc).AddTicks(9244));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 436, DateTimeKind.Utc).AddTicks(9245));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 4, 9, 29, 436, DateTimeKind.Utc).AddTicks(9246));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$tcPOOt6E2R0xG2mtzaWjue6WlvHjt0s1dy806vKqwTUc9bRWPxue.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore the renamed table if it still exists; otherwise recreate it empty.
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'ActivityLogs_Backup', N'U') IS NOT NULL
    EXEC sp_rename 'ActivityLogs_Backup', 'ActivityLogs';
ELSE IF OBJECT_ID(N'ActivityLogs', N'U') IS NULL
    CREATE TABLE [ActivityLogs] (
        [Id] bigint NOT NULL IDENTITY(1,1),
        [Action] nvarchar(max) NOT NULL,
        [EntityId] nvarchar(max) NULL,
        [EntityName] nvarchar(max) NOT NULL,
        [InstituteId] int NULL,
        [IpAddress] nvarchar(max) NULL,
        [NewValues] nvarchar(max) NULL,
        [OldValues] nvarchar(max) NULL,
        [Timestamp] datetime2 NOT NULL,
        [UserAgent] nvarchar(max) NULL,
        [UserEmail] nvarchar(max) NOT NULL,
        [UserId] int NULL,
        [UserName] nvarchar(max) NOT NULL,
        [UserRole] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ActivityLogs] PRIMARY KEY ([Id])
    );");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(726));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(2067));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(2069));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(2071));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(2085));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(2087));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 91, DateTimeKind.Utc).AddTicks(2089));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 419, DateTimeKind.Utc).AddTicks(7493));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 420, DateTimeKind.Utc).AddTicks(410));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 420, DateTimeKind.Utc).AddTicks(418));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 420, DateTimeKind.Utc).AddTicks(420));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 420, DateTimeKind.Utc).AddTicks(422));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 420, DateTimeKind.Utc).AddTicks(424));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 420, DateTimeKind.Utc).AddTicks(426));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 110, DateTimeKind.Utc).AddTicks(5329));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 110, DateTimeKind.Utc).AddTicks(7122));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 110, DateTimeKind.Utc).AddTicks(7127));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 110, DateTimeKind.Utc).AddTicks(7128));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 110, DateTimeKind.Utc).AddTicks(7130));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 3, 32, 3, 110, DateTimeKind.Utc).AddTicks(7131));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$qBvuXp12/2YNE22.oN38WOh/1cThcrjtejGhDnejbp90dPmDe/oXC");
        }
    }
}
