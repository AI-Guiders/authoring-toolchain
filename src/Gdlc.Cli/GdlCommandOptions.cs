namespace Gdlc.Cli;

internal class GdlCommandOptions
{
    public string? Path { get; init; }

    public string? ProjectPath { get; init; }

    public string Lang { get; init; } = "cs";

    public string? Surface { get; init; }

    public string? WorkspaceRoot { get; init; }

    public static bool TryParse(
        string[] args,
        out GdlCommandOptions? options,
        out string? error,
        bool allowProject = true)
    {
        options = null;
        error = null;

        var lang = "cs";
        string? surface = null;
        string? path = null;
        string? projectPath = null;
        string? workspaceRoot = null;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg.StartsWith("--lang=", StringComparison.Ordinal))
            {
                lang = arg["--lang=".Length..];
                continue;
            }

            if (arg.StartsWith("--surface=", StringComparison.Ordinal))
            {
                surface = arg["--surface=".Length..];
                continue;
            }

            switch (arg)
            {
                case "--lang" when i + 1 < args.Length:
                    lang = args[++i];
                    break;
                case "--surface" when i + 1 < args.Length:
                    surface = args[++i];
                    break;
                case "--project" when allowProject && i + 1 < args.Length:
                    projectPath = args[++i];
                    break;
                case "--workspace" or "-w" when i + 1 < args.Length:
                    workspaceRoot = args[++i];
                    break;
                default:
                    if (arg.StartsWith('-'))
                    {
                        error = $"unknown argument `{arg}`";
                        return false;
                    }

                    if (path is not null)
                    {
                        error = "multiple file paths";
                        return false;
                    }

                    path = arg;
                    break;
            }
        }

        if (allowProject && projectPath is not null && path is not null)
        {
            error = "use either --project or a single file path, not both";
            return false;
        }

        if (string.IsNullOrWhiteSpace(projectPath) && string.IsNullOrWhiteSpace(path))
        {
            error = "missing file path or --project";
            return false;
        }

        if (!string.Equals(lang, "cs", StringComparison.OrdinalIgnoreCase))
        {
            error = $"unsupported --lang `{lang}` (supports cs only)";
            return false;
        }

        options = new GdlCommandOptions
        {
            Path = path is null ? null : System.IO.Path.GetFullPath(path),
            ProjectPath = projectPath is null ? null : System.IO.Path.GetFullPath(projectPath),
            Lang = lang,
            Surface = surface,
            WorkspaceRoot = workspaceRoot,
        };

        return true;
    }
}
