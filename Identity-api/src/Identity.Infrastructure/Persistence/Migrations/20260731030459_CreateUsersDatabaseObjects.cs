using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateUsersDatabaseObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(EmbeddedSql.Read(
                "Persistence/Sql/StoredProcedures/sp_get_all_users.Drop.sql"));
            migrationBuilder.Sql(EmbeddedSql.Read(
                "Persistence/Sql/StoredProcedures/sp_get_all_users.Create.sql"));
            migrationBuilder.Sql(EmbeddedSql.Read(
                "Persistence/Sql/Views/v_get_all_users.Create.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(EmbeddedSql.Read(
                "Persistence/Sql/StoredProcedures/sp_get_all_users.Drop.sql"));
            migrationBuilder.Sql(EmbeddedSql.Read(
                "Persistence/Sql/Views/v_get_all_users.Drop.sql"));
        }
    }
}
