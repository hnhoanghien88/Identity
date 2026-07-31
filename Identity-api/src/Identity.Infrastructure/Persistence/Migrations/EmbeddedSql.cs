using System.Reflection;

namespace Identity.Infrastructure.Persistence.Migrations;

internal static class EmbeddedSql
{
    public static string Read(string relativePath)
    {
        var assembly = typeof(EmbeddedSql).Assembly;
        var resourceSuffix = relativePath
            .Replace('/', '.')
            .Replace('\\', '.');
        var resourceNames = assembly
            .GetManifestResourceNames()
            .Where(name => name.EndsWith(resourceSuffix, StringComparison.Ordinal))
            .ToArray();

        if (resourceNames.Length != 1)
        {
            throw new InvalidOperationException(
                $"Expected one embedded SQL resource ending with '{resourceSuffix}', but found {resourceNames.Length}.");
        }

        using var stream = assembly.GetManifestResourceStream(resourceNames[0])
            ?? throw new InvalidOperationException($"Embedded SQL resource '{resourceNames[0]}' could not be opened.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
