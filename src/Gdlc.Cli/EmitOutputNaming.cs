namespace Gdlc.Cli;

internal static class EmitOutputNaming
{
    public static string DefaultClassName(string documentPath)
    {
        var fileName = Path.GetFileName(documentPath);
        if (fileName.EndsWith(".catalog.gdl", StringComparison.OrdinalIgnoreCase))
        {
            var stem = fileName[..^".catalog.gdl".Length];
            return $"{ToPascalCase(stem)}Catalog";
        }

        if (fileName.EndsWith(".deck.gdl", StringComparison.OrdinalIgnoreCase))
        {
            var stem = fileName[..^".deck.gdl".Length];
            return $"{ToPascalCase(stem)}DeckIds";
        }

        return "Generated";
    }

    public static string ResolveProjectOutputPath(string workspaceRoot, string? outputDir, string documentPath)
    {
        var directory = string.IsNullOrWhiteSpace(outputDir)
            ? Path.Combine(workspaceRoot, "Generated")
            : Path.IsPathRooted(outputDir)
                ? outputDir
                : Path.Combine(workspaceRoot, outputDir);

        return Path.Combine(directory, $"{DefaultClassName(documentPath)}.g.cs");
    }

    private static string ToPascalCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Generated";
        }

        var parts = value.Split(['-', '_', '.'], StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Select(static part =>
            part.Length == 0
                ? string.Empty
                : char.ToUpperInvariant(part[0]) + part[1..]));
    }
}
