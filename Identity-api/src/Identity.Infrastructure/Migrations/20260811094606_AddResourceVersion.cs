using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TEMPORARY TABLE resource_code_validation (
                    ApplicationId BIGINT UNSIGNED NOT NULL,
                    NormalizedCode VARCHAR(120) NOT NULL,
                    UNIQUE KEY UQResourceCodeValidation (ApplicationId, NormalizedCode)
                ) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;

                INSERT INTO resource_code_validation (ApplicationId, NormalizedCode)
                SELECT ApplicationId, UPPER(TRIM(Code))
                FROM resources;

                DROP TEMPORARY TABLE resource_code_validation;
                """);
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "resources",
                type: "varchar(120)",
                maxLength: 120,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AddColumn<ulong>(
                name: "Version",
                table: "resources",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 1ul);
            global::Identity.Infrastructure.Persistence.Migrations.EmbeddedSql.ExecuteFolder(
                migrationBuilder,
                "Persistence/Sql/Migrations/AddResourceVersion/Up");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            global::Identity.Infrastructure.Persistence.Migrations.EmbeddedSql.ExecuteFolder(
                migrationBuilder,
                "Persistence/Sql/Migrations/AddResourceVersion/Down");
            migrationBuilder.DropColumn(
                name: "Version",
                table: "resources");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "resources",
                type: "varchar(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(120)",
                oldMaxLength: 120,
                oldCollation: "utf8mb4_0900_ai_ci");
        }
    }
}
