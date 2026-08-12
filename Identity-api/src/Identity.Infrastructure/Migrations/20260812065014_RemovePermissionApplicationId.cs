using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePermissionApplicationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_permissions_applications_ApplicationId",
                table: "permissions");

            migrationBuilder.DropIndex(
                name: "IX_permissions_ApplicationId",
                table: "permissions");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "permissions");
            global::Identity.Infrastructure.Persistence.Migrations.EmbeddedSql.ExecuteFolder(
                migrationBuilder,
                "Persistence/Sql/Migrations/RemovePermissionApplicationId/Up");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            global::Identity.Infrastructure.Persistence.Migrations.EmbeddedSql.ExecuteFolder(
                migrationBuilder,
                "Persistence/Sql/Migrations/RemovePermissionApplicationId/Down");
            migrationBuilder.AddColumn<ulong>(
                name: "ApplicationId",
                table: "permissions",
                type: "bigint unsigned",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE permissions p
                INNER JOIN resources r ON r.Id = p.ResourceId
                SET p.ApplicationId = r.ApplicationId;
                """);

            migrationBuilder.AlterColumn<ulong>(
                name: "ApplicationId",
                table: "permissions",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_permissions_ApplicationId",
                table: "permissions",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_permissions_applications_ApplicationId",
                table: "permissions",
                column: "ApplicationId",
                principalTable: "applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
