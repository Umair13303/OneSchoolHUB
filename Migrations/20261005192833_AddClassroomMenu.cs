using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddClassroomMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Additive Classroom menu seed only. IF NOT EXISTS so live DBs that
            // already received MenuItem 59 via Program.cs ensure or manual SQL
            // are not duplicated or altered (no UPDATE of existing rows).
            migrationBuilder.Sql(@"
SET IDENTITY_INSERT MenuItems ON;
IF NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemId = 59)
INSERT INTO MenuItems (MenuItemId, ParentId, Title, Icon, RouteUrl, SortOrder, IsActive, IsDeleted, CreatedAt, CampusId, InstituteId, CreatedBy, UpdatedAt, UpdatedBy)
VALUES (59, NULL, N'Classroom', N'play_lesson', N'/classroom/today', 12, 1, 0, '2025-01-01', NULL, NULL, NULL, NULL, NULL);
SET IDENTITY_INSERT MenuItems OFF;

SET IDENTITY_INSERT MenuRolePermissions ON;
IF NOT EXISTS (SELECT 1 FROM MenuRolePermissions WHERE Id = 214)
INSERT INTO MenuRolePermissions (Id, MenuItemId, RoleId) VALUES (214, 59, 4);
SET IDENTITY_INSERT MenuRolePermissions OFF;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM MenuRolePermissions WHERE Id = 214;
DELETE FROM MenuItems WHERE MenuItemId = 59;
");
        }
    }
}
