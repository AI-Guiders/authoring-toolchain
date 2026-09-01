namespace Authoring.Cli;

internal static class CliCatalogWorkspace
{
    public static bool TryParseCatalogArgs(
        string[] args,
        out string catalogPath,
        out string? workspaceRoot,
        out string[] passthrough)
    {
        catalogPath = "";
        workspaceRoot = null;
        var list = new List<string>();

        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] is "--workspace" or "-w" && i + 1 < args.Length)
            {
                workspaceRoot = args[++i];
                continue;
            }

            list.Add(args[i]);
        }

        passthrough = list.ToArray();
        if (passthrough.Length == 0)
        {
            return false;
        }

        catalogPath = Path.GetFullPath(passthrough[0]);
        return true;
    }

    public static string ResolveWorkspaceRoot(string catalogPath, string? explicitRoot)
    {
        if (!string.IsNullOrWhiteSpace(explicitRoot))
        {
            return Path.GetFullPath(explicitRoot);
        }

        var dir = new DirectoryInfo(Path.GetDirectoryName(Path.GetFullPath(catalogPath))!);
        while (dir is not null)
        {
            if (dir.GetFiles("*.slnx").Length > 0
                || dir.GetFiles("*.sln").Length > 0
                || Directory.Exists(Path.Combine(dir.FullName, ".git")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        return Path.GetDirectoryName(Path.GetFullPath(catalogPath))!;
    }
}
