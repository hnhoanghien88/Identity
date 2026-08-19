using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "user_roles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "role_permissions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "role_permissions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "refresh_tokens",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "permissions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "permissions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "permission_actions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "permission_actions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
            global::Identity.Infrastructure.Persistence.Migrations.EmbeddedSql.ExecuteFolder(
                migrationBuilder,
                "Persistence/Sql/Migrations/AddCascadeSoftDelete/Up");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            global::Identity.Infrastructure.Persistence.Migrations.EmbeddedSql.ExecuteFolder(
                migrationBuilder,
                "Persistence/Sql/Migrations/AddCascadeSoftDelete/Down");
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "role_permissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "role_permissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "permissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "permissions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "permission_actions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "permission_actions");
        }
    }
}
