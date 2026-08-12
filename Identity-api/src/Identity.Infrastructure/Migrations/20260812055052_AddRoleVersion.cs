using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "Version",
                table: "roles",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 1ul);
            global::Identity.Infrastructure.Persistence.Migrations.EmbeddedSql.ExecuteFolder(
                migrationBuilder,
                "Persistence/Sql/Migrations/AddRoleVersion/Up");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            global::Identity.Infrastructure.Persistence.Migrations.EmbeddedSql.ExecuteFolder(
                migrationBuilder,
                "Persistence/Sql/Migrations/AddRoleVersion/Down");
            migrationBuilder.DropColumn(
                name: "Version",
                table: "roles");
        }
    }
}
