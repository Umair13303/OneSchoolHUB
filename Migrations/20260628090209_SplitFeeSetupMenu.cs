using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class SplitFeeSetupMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing "Fee Setup" item to become "Fee Types"
            migrationBuilder.Sql(@"
                UPDATE [dbo].[MenuItems]
                SET [Title] = 'Fee Types', [RouteUrl] = '/fees/types', [Icon] = 'sell', [SortOrder] = 76
                WHERE [MenuItemId] = 21;
            ");

            // Add Fee Structures and Discount Policies as new menu items under Fees parent (20)
            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT [dbo].[MenuItems] ON;
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [MenuItemId] = 50)
                    INSERT INTO [dbo].[MenuItems] ([MenuItemId],[CreatedAt],[CreatedBy],[Icon],[IsActive],[IsDeleted],[ParentId],[RouteUrl],[SortOrder],[Title],[UpdatedAt],[UpdatedBy])
                    VALUES (50,'2026-06-28 00:00:00',NULL,'receipt_long',1,0,20,'/fees/structures',77,'Fee Structures',NULL,NULL);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [MenuItemId] = 51)
                    INSERT INTO [dbo].[MenuItems] ([MenuItemId],[CreatedAt],[CreatedBy],[Icon],[IsActive],[IsDeleted],[ParentId],[RouteUrl],[SortOrder],[Title],[UpdatedAt],[UpdatedBy])
                    VALUES (51,'2026-06-28 00:00:00',NULL,'local_offer',1,0,20,'/fees/discounts',78,'Discount Policies',NULL,NULL);
                SET IDENTITY_INSERT [dbo].[MenuItems] OFF;
            ");

            // Role permissions: admin (RoleId=2) and superadmin (RoleId=1) only
            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT [dbo].[MenuRolePermissions] ON;
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 130)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (130,50,1);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 131)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (131,50,2);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 132)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (132,51,1);
                IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuRolePermissions] WHERE [Id] = 133)
                    INSERT INTO [dbo].[MenuRolePermissions] ([Id],[MenuItemId],[RoleId]) VALUES (133,51,2);
                SET IDENTITY_INSERT [dbo].[MenuRolePermissions] OFF;
            ");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 965, DateTimeKind.Utc).AddTicks(2528));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 965, DateTimeKind.Utc).AddTicks(5044));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 965, DateTimeKind.Utc).AddTicks(5047));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 965, DateTimeKind.Utc).AddTicks(5049));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 965, DateTimeKind.Utc).AddTicks(5052));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 965, DateTimeKind.Utc).AddTicks(5053));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 965, DateTimeKind.Utc).AddTicks(5054));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 6, 204, DateTimeKind.Utc).AddTicks(1892));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 6, 204, DateTimeKind.Utc).AddTicks(4343));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 6, 204, DateTimeKind.Utc).AddTicks(4353));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 6, 204, DateTimeKind.Utc).AddTicks(4355));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 6, 204, DateTimeKind.Utc).AddTicks(4392));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 6, 204, DateTimeKind.Utc).AddTicks(4394));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 6, 204, DateTimeKind.Utc).AddTicks(4395));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 979, DateTimeKind.Utc).AddTicks(799));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 979, DateTimeKind.Utc).AddTicks(1562));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 979, DateTimeKind.Utc).AddTicks(1566));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 979, DateTimeKind.Utc).AddTicks(1567));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 979, DateTimeKind.Utc).AddTicks(1568));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 28, 9, 2, 5, 979, DateTimeKind.Utc).AddTicks(1569));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$zapjUdGrRhVA82xijXomPOI1hH.gq5Ufi6MmV5h3fwvwet3PvcYIW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM [dbo].[MenuRolePermissions] WHERE [Id] IN (130,131,132,133);
                DELETE FROM [dbo].[MenuItems] WHERE [MenuItemId] IN (50,51);
                UPDATE [dbo].[MenuItems]
                SET [Title] = 'Fee Setup', [RouteUrl] = '/fees/setup', [Icon] = 'settings', [SortOrder] = 76
                WHERE [MenuItemId] = 21;
            ");

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(3796));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(5806));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(5810));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(5812));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(5814));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(5816));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 173, DateTimeKind.Utc).AddTicks(5817));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(2198));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(5092));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(5099));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(5103));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(5108));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(5110));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 498, DateTimeKind.Utc).AddTicks(5112));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 203, DateTimeKind.Utc).AddTicks(2596));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 203, DateTimeKind.Utc).AddTicks(3862));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 203, DateTimeKind.Utc).AddTicks(3866));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 203, DateTimeKind.Utc).AddTicks(3867));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 203, DateTimeKind.Utc).AddTicks(3894));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 17, 16, 48, 15, 203, DateTimeKind.Utc).AddTicks(3895));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$hAsajSlrxAOU22zHDdCXSuyx/4z/UMt7.M3LFM8UxuClFXEl8FXuy");
        }
    }
}
