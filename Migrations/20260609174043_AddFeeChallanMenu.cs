using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFeeChallanMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 237, DateTimeKind.Utc).AddTicks(9481));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 238, DateTimeKind.Utc).AddTicks(1434));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 238, DateTimeKind.Utc).AddTicks(1438));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 238, DateTimeKind.Utc).AddTicks(1441));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 238, DateTimeKind.Utc).AddTicks(1443));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 238, DateTimeKind.Utc).AddTicks(1445));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 238, DateTimeKind.Utc).AddTicks(1447));

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT [dbo].[MenuItems] ON;
                -- Insert Fees parent (20) first, then children (21,22,23,39)
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [MenuItemId] = 20)
                    INSERT INTO [dbo].[MenuItems] ([MenuItemId],[CreatedAt],[CreatedBy],[Icon],[IsActive],[IsDeleted],[ParentId],[RouteUrl],[SortOrder],[Title],[UpdatedAt],[UpdatedBy])
                    VALUES (20,'2025-01-01 00:00:00',NULL,'payments',1,0,NULL,NULL,75,'Fees',NULL,NULL);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [MenuItemId] = 21)
                    INSERT INTO [dbo].[MenuItems] ([MenuItemId],[CreatedAt],[CreatedBy],[Icon],[IsActive],[IsDeleted],[ParentId],[RouteUrl],[SortOrder],[Title],[UpdatedAt],[UpdatedBy])
                    VALUES (21,'2025-01-01 00:00:00',NULL,'settings',1,0,20,'/fees/setup',76,'Fee Setup',NULL,NULL);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [MenuItemId] = 22)
                    INSERT INTO [dbo].[MenuItems] ([MenuItemId],[CreatedAt],[CreatedBy],[Icon],[IsActive],[IsDeleted],[ParentId],[RouteUrl],[SortOrder],[Title],[UpdatedAt],[UpdatedBy])
                    VALUES (22,'2025-01-01 00:00:00',NULL,'receipt',1,0,20,'/fees/student-fees',77,'Student Fees',NULL,NULL);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [MenuItemId] = 23)
                    INSERT INTO [dbo].[MenuItems] ([MenuItemId],[CreatedAt],[CreatedBy],[Icon],[IsActive],[IsDeleted],[ParentId],[RouteUrl],[SortOrder],[Title],[UpdatedAt],[UpdatedBy])
                    VALUES (23,'2025-01-01 00:00:00',NULL,'bar_chart',1,0,20,'/fees/report',78,'Fee Report',NULL,NULL);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [MenuItemId] = 39)
                    INSERT INTO [dbo].[MenuItems] ([MenuItemId],[CreatedAt],[CreatedBy],[Icon],[IsActive],[IsDeleted],[ParentId],[RouteUrl],[SortOrder],[Title],[UpdatedAt],[UpdatedBy])
                    VALUES (39,'2025-01-01 00:00:00',NULL,'receipt_long',1,0,20,'/fees/challan',79,'Fee Challan',NULL,NULL);
                SET IDENTITY_INSERT [dbo].[MenuItems] OFF;
            ");

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT [dbo].[MenuRolePermissions] ON;
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 66)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (66,20,1);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 67)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (67,20,2);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 68)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (68,20,3);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 69)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (69,21,1);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 70)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (70,21,2);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 71)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (71,22,1);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 72)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (72,22,2);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 73)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (73,22,3);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 74)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (74,23,1);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 75)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (75,23,2);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 76)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (76,23,3);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 127)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (127,39,1);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 128)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (128,39,2);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 129)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (129,39,3);
                SET IDENTITY_INSERT [dbo].[MenuRolePermissions] OFF;
            ");

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 777, DateTimeKind.Utc).AddTicks(7720));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 778, DateTimeKind.Utc).AddTicks(1047));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 778, DateTimeKind.Utc).AddTicks(1054));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 778, DateTimeKind.Utc).AddTicks(1058));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 778, DateTimeKind.Utc).AddTicks(1061));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 778, DateTimeKind.Utc).AddTicks(1065));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 778, DateTimeKind.Utc).AddTicks(1068));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 280, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 280, DateTimeKind.Utc).AddTicks(1374));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 280, DateTimeKind.Utc).AddTicks(1382));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 280, DateTimeKind.Utc).AddTicks(1384));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 280, DateTimeKind.Utc).AddTicks(1388));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 40, 40, 280, DateTimeKind.Utc).AddTicks(1447));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$cCC5QpX3SXGVadES.YS5AebG95W1IKHn5JCzb4ncVYCiMSco1/N6C");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "MenuItemId",
                keyValue: 39);

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 179, DateTimeKind.Utc).AddTicks(8892));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 180, DateTimeKind.Utc).AddTicks(338));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 180, DateTimeKind.Utc).AddTicks(342));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 180, DateTimeKind.Utc).AddTicks(344));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 180, DateTimeKind.Utc).AddTicks(345));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 180, DateTimeKind.Utc).AddTicks(347));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 180, DateTimeKind.Utc).AddTicks(348));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 626, DateTimeKind.Utc).AddTicks(2964));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 626, DateTimeKind.Utc).AddTicks(4679));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 626, DateTimeKind.Utc).AddTicks(4683));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 626, DateTimeKind.Utc).AddTicks(4685));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 626, DateTimeKind.Utc).AddTicks(4688));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 626, DateTimeKind.Utc).AddTicks(4689));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 626, DateTimeKind.Utc).AddTicks(4691));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 202, DateTimeKind.Utc).AddTicks(365));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 202, DateTimeKind.Utc).AddTicks(1380));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 202, DateTimeKind.Utc).AddTicks(1384));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 202, DateTimeKind.Utc).AddTicks(1385));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 202, DateTimeKind.Utc).AddTicks(1386));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 9, 17, 15, 0, 202, DateTimeKind.Utc).AddTicks(1388));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$GoTVZe2j3IygbHjqrt1PruSlWe6gOE9mPseku7i8h3K6tOYisiOYO");
        }
    }
}
