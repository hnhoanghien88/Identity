using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Identity.Infrastructure.Persistence.Migrations;

public sealed class EmbeddedSqlMigrationsGenerator(
    MigrationsCodeGeneratorDependencies dependencies,
    CSharpMigrationsGeneratorDependencies csharpDependencies)
    : CSharpMigrationsGenerator(dependencies, csharpDependencies)
{
    public override string GenerateMigration(
        string? migrationNamespace,
        string migrationName,
        IReadOnlyList<MigrationOperation> upOperations,
        IReadOnlyList<MigrationOperation> downOperations)
    {
        var code = base.GenerateMigration(
            migrationNamespace,
            migrationName,
            upOperations,
            downOperations);
        var sqlFolder = $"Persistence/Sql/Migrations/{migrationName}";

        code = InsertExecuteFolder(code, "Up", $"{sqlFolder}/Up");
        return InsertExecuteFolder(code, "Down", $"{sqlFolder}/Down");
    }

    private static string InsertExecuteFolder(
        string code,
        string methodName,
        string relativeFolder)
    {
        var pattern = $@"(protected override void {methodName}\(MigrationBuilder migrationBuilder\)\s*\{{)";
        var matches = Regex.Matches(code, pattern, RegexOptions.CultureInvariant);
        if (matches.Count != 1)
        {
            throw new InvalidOperationException(
                $"Expected one generated {methodName} method, but found {matches.Count}.");
        }

        return Regex.Replace(
            code,
            pattern,
            match => $"{match.Value}{Environment.NewLine}" +
                     $"            EmbeddedSql.ExecuteFolder({Environment.NewLine}" +
                     $"                migrationBuilder,{Environment.NewLine}" +
                     $"                {Convert.ToChar(34)}{relativeFolder}{Convert.ToChar(34)});",
            RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(1));
    }
}
