using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations;

[DbContext(typeof(IdentityDbContext))]
[Migration("20260812080000_RemovePermissionIsDeleted")]
public sealed class RemovePermissionIsDeleted : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "permissions");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "permissions",
            type: "tinyint(1)",
            nullable: false,
            defaultValue: false);
    }
}
