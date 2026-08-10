using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations;

[DbContext(typeof(IdentityDbContext))]
[Migration("20260810090000_EnforceUserCodeIdentity")]
public sealed class EnforceUserCodeIdentity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            INSERT INTO user_code_mappings (UserId, Code)
            SELECT 1, 'admin'
            FROM users
            WHERE Id = 1
              AND NOT EXISTS (
                  SELECT 1
                  FROM user_code_mappings
                  WHERE UserId = 1
              );
            """);

        migrationBuilder.Sql(
            """
            INSERT INTO user_code_mappings (UserId, Code)
            SELECT u.Id, CONCAT('user-', u.Id)
            FROM users AS u
            LEFT JOIN user_code_mappings AS m ON m.UserId = u.Id
            WHERE m.UserId IS NULL;
            """);

        migrationBuilder.Sql(
            """
            UPDATE users AS u
            INNER JOIN user_code_mappings AS m ON m.UserId = u.Id
            SET u.Code = m.Code,
                u.NormalizedCode = UPPER(m.Code)
            WHERE m.Code REGEXP '^[A-Za-z0-9._-]{1,50}$';
            """);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "users",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "NormalizedCode",
            table: "users",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "users",
            type: "varchar(254)",
            maxLength: 254,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "NormalizedEmail",
            table: "users",
            type: "varchar(254)",
            maxLength: 254,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(255)",
            oldMaxLength: 255);

        migrationBuilder.CreateIndex(
            name: "UQUsersNormalizedCode",
            table: "users",
            column: "NormalizedCode",
            unique: true);

        migrationBuilder.DropTable(
            name: "user_code_mappings");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "UQUsersNormalizedCode",
            table: "users");

        migrationBuilder.CreateTable(
            name: "user_code_mappings",
            columns: table => new
            {
                UserId = table.Column<ulong>(
                    type: "bigint unsigned",
                    nullable: false),
                Code = table.Column<string>(
                    type: "varchar(50)",
                    maxLength: 50,
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_user_code_mappings",
                    x => x.UserId);
            });

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "users",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50);

        migrationBuilder.AlterColumn<string>(
            name: "NormalizedCode",
            table: "users",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50);

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "users",
            type: "varchar(255)",
            maxLength: 255,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(254)",
            oldMaxLength: 254);

        migrationBuilder.AlterColumn<string>(
            name: "NormalizedEmail",
            table: "users",
            type: "varchar(255)",
            maxLength: 255,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(254)",
            oldMaxLength: 254);
    }
}
