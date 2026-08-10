using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNormalizedUserIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TEMPORARY TABLE user_identity_preflight (
                    Code VARCHAR(50) COLLATE utf8mb4_unicode_ci NOT NULL,
                    Email VARCHAR(254) COLLATE utf8mb4_unicode_ci NOT NULL,
                    UNIQUE KEY UQPreflightCode (Code),
                    UNIQUE KEY UQPreflightEmail (Email),
                    CONSTRAINT CKPreflightCode CHECK (Code REGEXP '^[A-Za-z0-9._-]{1,50}$'),
                    CONSTRAINT CKPreflightEmail CHECK (CHAR_LENGTH(TRIM(Email)) BETWEEN 3 AND 254 AND Email = TRIM(Email))
                );
                INSERT INTO user_identity_preflight (Code, Email)
                SELECT Code, Email FROM users;
                DROP TEMPORARY TABLE user_identity_preflight;
                """);

            migrationBuilder.DropIndex(
                name: "UQUsersNormalizedCode",
                table: "users");

            migrationBuilder.DropIndex(
                name: "UQUsersNormalizedEmail",
                table: "users");

            migrationBuilder.DropColumn(
                name: "NormalizedCode",
                table: "users");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "users",
                type: "varchar(254)",
                maxLength: 254,
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(254)",
                oldMaxLength: 254);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                collation: "utf8mb4_unicode_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "UQUsersCode",
                table: "users",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQUsersEmail",
                table: "users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQUsersCode",
                table: "users");

            migrationBuilder.DropIndex(
                name: "UQUsersEmail",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "users",
                type: "varchar(254)",
                maxLength: 254,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(254)",
                oldMaxLength: 254,
                oldCollation: "utf8mb4_unicode_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldCollation: "utf8mb4_unicode_ci");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedCode",
                table: "users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "users",
                type: "varchar(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE users
                SET NormalizedCode = UPPER(Code),
                    NormalizedEmail = UPPER(Email);
                """);

            migrationBuilder.CreateIndex(
                name: "UQUsersNormalizedCode",
                table: "users",
                column: "NormalizedCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQUsersNormalizedEmail",
                table: "users",
                column: "NormalizedEmail",
                unique: true);
        }
    }
}
