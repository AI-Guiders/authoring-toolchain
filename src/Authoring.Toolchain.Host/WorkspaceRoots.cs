namespace Authoring.Toolchain.Host;

/// <summary>
/// Resolves workspace roots from an LSP initialize request shape (folder list or legacy root).
/// </summary>
public static class WorkspaceRoots
{
    public static IReadOnlyList<string> FromPaths(
        IEnumerable<string>? workspaceFolderPaths,
        string? rootPath)
    {
        if (workspaceFolderPaths is not null)
        {
            var folders = workspaceFolderPaths
                .Where(static p => !string.IsNullOrWhiteSpace(p))
                .Select(Path.GetFullPath)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (folders.Length > 0)
            {
                return folders;
            }
        }

        if (!string.IsNullOrWhiteSpace(rootPath))
        {
            return [Path.GetFullPath(rootPath)];
        }

        return [];
    }
}
