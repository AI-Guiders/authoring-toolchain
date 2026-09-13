namespace Gdlc.Cli;

internal sealed class SatOptions : GdlCommandOptions
{
    public string? AdrId { get; init; }

    public string? FactsPath { get; init; }

    public bool UsesHoareObservers =>
        !string.IsNullOrWhiteSpace(AdrId) || !string.IsNullOrWhiteSpace(FactsPath);

    public static bool TryParse(string[] args, out SatOptions? options, out string? error)
    {
        options = null;
        error = null;

        var lang = "cs";
        string? surface = null;
        string? path = null;
        string? projectPath = null;
        string? workspaceRoot = null;
        string? adrId = null;
        string? factsPath = null;

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

            if (arg.StartsWith("--adr=", StringComparison.Ordinal))
            {
                adrId = arg["--adr=".Length..];
                continue;
            }

            if (arg.StartsWith("--facts=", StringComparison.Ordinal))
            {
                factsPath = arg["--facts=".Length..];
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
                case "--project" when i + 1 < args.Length:
                    projectPath = args[++i];
                    break;
                case "--workspace" or "-w" when i + 1 < args.Length:
                    workspaceRoot = args[++i];
                    break;
                case "--adr" when i + 1 < args.Length:
                    adrId = args[++i];
                    break;
                case "--facts" when i + 1 < args.Length:
                    factsPath = args[++i];
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

        if (projectPath is not null && path is not null)
        {
            error = "use either --project or a single file path, not both";
            return false;
        }

        var usesHoareObservers = !string.IsNullOrWhiteSpace(adrId) || !string.IsNullOrWhiteSpace(factsPath);
        if (!usesHoareObservers && string.IsNullOrWhiteSpace(projectPath) && string.IsNullOrWhiteSpace(path))
        {
            error = "missing file path, --project, --adr, or --facts";
            return false;
        }

        if (!string.Equals(lang, "cs", StringComparison.OrdinalIgnoreCase))
        {
            error = $"unsupported --lang `{lang}` (supports cs only)";
            return false;
        }

        options = new SatOptions
        {
            Path = path is null ? null : System.IO.Path.GetFullPath(path),
            ProjectPath = projectPath is null ? null : System.IO.Path.GetFullPath(projectPath),
            Lang = lang,
            Surface = surface,
            WorkspaceRoot = workspaceRoot,
            AdrId = adrId,
            FactsPath = factsPath is null ? null : System.IO.Path.GetFullPath(factsPath),
        };

        return true;
    }
}
