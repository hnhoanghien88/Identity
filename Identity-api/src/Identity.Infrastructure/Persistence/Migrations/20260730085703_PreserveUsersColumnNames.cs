using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PreserveUsersColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "users",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_date",
                table: "users",
                newName: "CreatedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "users",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "users",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "users",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "users",
                newName: "created_date");
        }
    }
}
