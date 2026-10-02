using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfilePhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhotoFileId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhotoFileId",
                table: "Users",
                column: "PhotoFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_FileStores_PhotoFileId",
                table: "Users",
                column: "PhotoFileId",
                principalTable: "FileStores",
                principalColumn: "FileId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_FileStores_PhotoFileId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PhotoFileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhotoFileId",
                table: "Users");
        }
    }
}
