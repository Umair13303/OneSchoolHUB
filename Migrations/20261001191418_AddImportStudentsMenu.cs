using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddImportStudentsMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure Add Student exists on DBs that missed prior seed rows.
            migrationBuilder.Sql(@"
SET IDENTITY_INSERT MenuItems ON;
IF NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemId = 52)
INSERT INTO MenuItems (MenuItemId, ParentId, Title, Icon, RouteUrl, SortOrder, IsActive, IsDeleted, CreatedAt)
VALUES (52, 6, N'Add Student', N'person_add_alt', N'/students/add', 42, 1, 0, '2025-01-01');
SET IDENTITY_INSERT MenuItems OFF;

SET IDENTITY_INSERT MenuRolePermissions ON;
IF NOT EXISTS (SELECT 1 FROM MenuRolePermissions WHERE MenuItemId = 52 AND RoleId = 2)
INSERT INTO MenuRolePermissions (Id, MenuItemId, RoleId) VALUES (141, 52, 2);
IF NOT EXISTS (SELECT 1 FROM MenuRolePermissions WHERE MenuItemId = 52 AND RoleId = 3)
INSERT INTO MenuRolePermissions (Id, MenuItemId, RoleId) VALUES (142, 52, 3);
SET IDENTITY_INSERT MenuRolePermissions OFF;
");

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
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 8,
                column: "SortOrder",
                value: 44);

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "MenuItemId", "CampusId", "CreatedAt", "CreatedBy", "Icon", "InstituteId", "IsActive", "IsDeleted", "ParentId", "RouteUrl", "SortOrder", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 53, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "upload_file", null, true, false, 6, "/students/import", 43, "Import Students", null, null });

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

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$PKyYbeLFjFrgCoJ74kDwe.MH9KWdsLnoVRJyL37QhyIqaHYFHuYRO");

            migrationBuilder.InsertData(
                table: "MenuRolePermissions",
                columns: new[] { "Id", "MenuItemId", "RoleId" },
                values: new object[,]
                {
                    { 201, 53, 2 },
                    { 202, 53, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 53);

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 996, DateTimeKind.Utc).AddTicks(9353));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 997, DateTimeKind.Utc).AddTicks(571));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 997, DateTimeKind.Utc).AddTicks(574));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 997, DateTimeKind.Utc).AddTicks(576));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 997, DateTimeKind.Utc).AddTicks(613));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 997, DateTimeKind.Utc).AddTicks(615));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 34, 997, DateTimeKind.Utc).AddTicks(618));

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 8,
                column: "SortOrder",
                value: 43);

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(7704));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(9501));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(9506));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(9508));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(9510));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(9512));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 295, DateTimeKind.Utc).AddTicks(9513));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 15, DateTimeKind.Utc).AddTicks(3503));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 15, DateTimeKind.Utc).AddTicks(4526));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 15, DateTimeKind.Utc).AddTicks(4531));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 15, DateTimeKind.Utc).AddTicks(4533));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 15, DateTimeKind.Utc).AddTicks(4534));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 10, 17, 26, 35, 15, DateTimeKind.Utc).AddTicks(4559));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$3jM9Qr.SrI0bwMZMEmcL3.TSIv8INFG6CSBd3lLTftvx16q/t9oP.");
        }
    }
}
