using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "applications",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Audience = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "permission_actions",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_actions", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    SecurityStamp = table.Column<Guid>(type: "char(36)", nullable: false),
                    PermissionVersion = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "resources",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ApplicationId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Code = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    ResourceType = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_resources_applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ApplicationId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    IsSystemRole = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_roles_applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ApplicationId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TokenHash = table.Column<string>(type: "char(64)", fixedLength: true, maxLength: 64, nullable: false),
                    JwtId = table.Column<Guid>(type: "char(36)", nullable: false),
                    FamilyId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ExpiresDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RevokedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RevokedBy = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    ReplacedByTokenId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_refresh_tokens_ReplacedByTokenId",
                        column: x => x.ReplacedByTokenId,
                        principalTable: "refresh_tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "menus",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ApplicationId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ParentId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    ResourceId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    Code = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Route = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    Icon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsVisible = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_menus_applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_menus_menus_ParentId",
                        column: x => x.ParentId,
                        principalTable: "menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_menus_resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ApplicationId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ResourceId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ActionId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Code = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_permissions_applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_permissions_permission_actions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "permission_actions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_permissions_resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    RoleId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_roles_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_roles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    PermissionId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "UQApplicationsCode",
                table: "applications",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menus_ParentId",
                table: "menus",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_menus_ResourceId",
                table: "menus",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "UQMenus",
                table: "menus",
                columns: new[] { "ApplicationId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQPermissionActions",
                table: "permission_actions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permissions_ActionId",
                table: "permissions",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_ApplicationId",
                table: "permissions",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "UQPermissions",
                table: "permissions",
                columns: new[] { "ResourceId", "ActionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_ApplicationId",
                table: "refresh_tokens",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_ReplacedByTokenId",
                table: "refresh_tokens",
                column: "ReplacedByTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQRefreshTokenHash",
                table: "refresh_tokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQResources",
                table: "resources",
                columns: new[] { "ApplicationId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_PermissionId",
                table: "role_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "UQRolePermissions",
                table: "role_permissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQRoles",
                table: "roles",
                columns: new[] { "ApplicationId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_RoleId",
                table: "user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQUserRoles",
                table: "user_roles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQUsersNormalizedEmail",
                table: "users",
                column: "NormalizedEmail",
                unique: true);
            global::Identity.Infrastructure.Persistence.Migrations.EmbeddedSql.ExecuteFolder(
                migrationBuilder,
                "Persistence/Sql/Migrations/InitialDB/Up");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            global::Identity.Infrastructure.Persistence.Migrations.EmbeddedSql.ExecuteFolder(
                migrationBuilder,
                "Persistence/Sql/Migrations/InitialDB/Down");
            migrationBuilder.DropTable(
                name: "menus");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "permission_actions");

            migrationBuilder.DropTable(
                name: "resources");

            migrationBuilder.DropTable(
                name: "applications");
        }
    }
}
