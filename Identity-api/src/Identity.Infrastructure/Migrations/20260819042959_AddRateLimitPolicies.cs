using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRateLimitPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rate_limit_policies",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ApplicationId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    RoutePattern = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    HttpMethods = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    PartitionBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Algorithm = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    PermitLimit = table.Column<uint>(type: "int unsigned", nullable: false),
                    WindowSeconds = table.Column<uint>(type: "int unsigned", nullable: false),
                    BurstLimit = table.Column<uint>(type: "int unsigned", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<ulong>(type: "bigint unsigned", nullable: false, defaultValue: 1ul),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rate_limit_policies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rate_limit_policies_applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IXRateLimitPoliciesActivePriority",
                table: "rate_limit_policies",
                columns: new[] { "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IXRateLimitPoliciesMatch",
                table: "rate_limit_policies",
                columns: new[] { "ApplicationId", "RoutePattern" });

            migrationBuilder.CreateIndex(
                name: "UQRateLimitPoliciesName",
                table: "rate_limit_policies",
                column: "Name",
                unique: true);
            global::Identity.Infrastructure.Persistence.Migrations.EmbeddedSql.ExecuteFolder(
                migrationBuilder,
                "Persistence/Sql/Migrations/AddRateLimitPolicies/Up");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            global::Identity.Infrastructure.Persistence.Migrations.EmbeddedSql.ExecuteFolder(
                migrationBuilder,
                "Persistence/Sql/Migrations/AddRateLimitPolicies/Down");
            migrationBuilder.DropTable(
                name: "rate_limit_policies");
        }
    }
}
