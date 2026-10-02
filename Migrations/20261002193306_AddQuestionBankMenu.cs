using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionBankMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Additive menu seed only. IF NOT EXISTS so live DBs that already received
            // MenuItem 58 via manual IDENTITY_INSERT are not duplicated or altered.
            migrationBuilder.Sql(@"
SET IDENTITY_INSERT MenuItems ON;
IF NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemId = 58)
INSERT INTO MenuItems (MenuItemId, ParentId, Title, Icon, RouteUrl, SortOrder, IsActive, IsDeleted, CreatedAt, CampusId, InstituteId, CreatedBy, UpdatedAt, UpdatedBy)
VALUES (58, 34, N'Question Bank', N'library_books', N'/exams/question-bank', 67, 1, 0, '2025-01-01', NULL, NULL, NULL, NULL, NULL);
SET IDENTITY_INSERT MenuItems OFF;

SET IDENTITY_INSERT MenuRolePermissions ON;
IF NOT EXISTS (SELECT 1 FROM MenuRolePermissions WHERE Id = 211)
INSERT INTO MenuRolePermissions (Id, MenuItemId, RoleId) VALUES (211, 58, 2);
IF NOT EXISTS (SELECT 1 FROM MenuRolePermissions WHERE Id = 212)
INSERT INTO MenuRolePermissions (Id, MenuItemId, RoleId) VALUES (212, 58, 3);
IF NOT EXISTS (SELECT 1 FROM MenuRolePermissions WHERE Id = 213)
INSERT INTO MenuRolePermissions (Id, MenuItemId, RoleId) VALUES (213, 58, 4);
SET IDENTITY_INSERT MenuRolePermissions OFF;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM MenuRolePermissions WHERE Id IN (211, 212, 213);
DELETE FROM MenuItems WHERE MenuItemId = 58;
");
        }
    }
}
