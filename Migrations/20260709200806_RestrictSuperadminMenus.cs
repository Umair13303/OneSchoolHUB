using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class RestrictSuperadminMenus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "MenuRolePermissions",
                keyColumn: "Id",
                keyValue: 140);

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(2699));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(4537));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(4569));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(4571));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(4573));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(4574));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 778, DateTimeKind.Utc).AddTicks(4576));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(507));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(2320));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(2350));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(2352));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(2353));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(2355));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 3, 127, DateTimeKind.Utc).AddTicks(2357));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 799, DateTimeKind.Utc).AddTicks(8613));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 800, DateTimeKind.Utc).AddTicks(209));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 800, DateTimeKind.Utc).AddTicks(214));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 800, DateTimeKind.Utc).AddTicks(215));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 800, DateTimeKind.Utc).AddTicks(250));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 9, 20, 8, 2, 800, DateTimeKind.Utc).AddTicks(252));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$sGhtzhmmhJQJJsHU.0x2e.S5sg7WBJryOJlGN7Ojwb853Q6iAkWzm");

            // The DeleteData calls above only cover menu grants that came from the
            // HasData seed. Databases may also contain superadmin grants added at
            // runtime through Menu Management (e.g. Inventory/POS screens). Superadmin
            // is a provisioning role, so strip every grant except its whitelist:
            // /dashboard, /users, /admin/menu, /admin/settings and /superadmin/*.
            // NULL-route rows are group headers (Academics, Fees, ...) — superadmin
            // keeps none of those.
            migrationBuilder.Sql(@"
DELETE mrp
FROM   MenuRolePermissions mrp
JOIN   Roles r      ON r.RoleId = mrp.RoleId
JOIN   MenuItems mi ON mi.MenuItemId = mrp.MenuItemId
WHERE  r.RoleName = 'superadmin'
  AND (mi.RouteUrl IS NULL
       OR (mi.RouteUrl NOT LIKE '/superadmin/%'
           AND mi.RouteUrl NOT IN ('/dashboard', '/users', '/admin/menu', '/admin/settings')));
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 744, DateTimeKind.Utc).AddTicks(1770));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 744, DateTimeKind.Utc).AddTicks(3806));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 744, DateTimeKind.Utc).AddTicks(3810));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 744, DateTimeKind.Utc).AddTicks(3812));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 744, DateTimeKind.Utc).AddTicks(3832));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 744, DateTimeKind.Utc).AddTicks(3835));

            migrationBuilder.UpdateData(
                table: "CalendarEventTypes",
                keyColumn: "CalendarEventTypeId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 744, DateTimeKind.Utc).AddTicks(3837));

            migrationBuilder.InsertData(
                table: "MenuRolePermissions",
                columns: new[] { "Id", "MenuItemId", "RoleId" },
                values: new object[,]
                {
                    { 9, 3, 1 },
                    { 11, 4, 1 },
                    { 13, 5, 1 },
                    { 15, 6, 1 },
                    { 18, 7, 1 },
                    { 21, 8, 1 },
                    { 24, 9, 1 },
                    { 29, 10, 1 },
                    { 32, 11, 1 },
                    { 37, 12, 1 },
                    { 43, 14, 1 },
                    { 48, 15, 1 },
                    { 54, 17, 1 },
                    { 59, 18, 1 },
                    { 66, 20, 1 },
                    { 69, 21, 1 },
                    { 71, 22, 1 },
                    { 74, 23, 1 },
                    { 77, 24, 1 },
                    { 79, 25, 1 },
                    { 84, 27, 1 },
                    { 95, 31, 1 },
                    { 98, 32, 1 },
                    { 101, 33, 1 },
                    { 104, 34, 1 },
                    { 109, 35, 1 },
                    { 113, 36, 1 },
                    { 118, 37, 1 },
                    { 122, 38, 1 },
                    { 127, 39, 1 },
                    { 140, 52, 1 }
                });

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 987, DateTimeKind.Utc).AddTicks(7803));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 987, DateTimeKind.Utc).AddTicks(9396));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 987, DateTimeKind.Utc).AddTicks(9401));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 987, DateTimeKind.Utc).AddTicks(9403));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 987, DateTimeKind.Utc).AddTicks(9405));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 987, DateTimeKind.Utc).AddTicks(9407));

            migrationBuilder.UpdateData(
                table: "Periods",
                keyColumn: "PeriodId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 987, DateTimeKind.Utc).AddTicks(9408));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 765, DateTimeKind.Utc).AddTicks(3895));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 765, DateTimeKind.Utc).AddTicks(5029));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 765, DateTimeKind.Utc).AddTicks(5033));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 765, DateTimeKind.Utc).AddTicks(5034));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 765, DateTimeKind.Utc).AddTicks(5035));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 5, 8, 23, 3, 765, DateTimeKind.Utc).AddTicks(5049));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$lHFbOjeHJO3vegnPw6Dn0OE9eyj43cFKzUNb.7YBrDN6tWrn7Wb0m");
        }
    }
}
