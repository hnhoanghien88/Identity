using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations;

[DbContext(typeof(IdentityDbContext))]
[Migration("20260812081000_RemovePermissionIsActive")]
public sealed class RemovePermissionIsActive : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsActive",
            table: "permissions");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsActive",
            table: "permissions",
            type: "tinyint(1)",
            nullable: false,
            defaultValue: true);
    }
}
