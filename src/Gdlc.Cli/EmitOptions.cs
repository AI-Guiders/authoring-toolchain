namespace Gdlc.Cli;

internal sealed class EmitOptions : GdlCommandOptions
{
    public string Namespace { get; init; } = "Generated";

    public string ClassName { get; init; } = "Generated";

    public string? OutputPath { get; init; }

    public static bool TryParse(string[] args, out EmitOptions? options, out string? error)
    {
        options = null;
        error = null;

        var lang = "cs";
        string? surface = null;
        string? path = null;
        string? projectPath = null;
        string? workspaceRoot = null;
        var namespaceName = "Generated";
        var className = "Generated";
        string? outputPath = null;

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
                case "--project" when i + 1 < args.Length:
                    projectPath = args[++i];
                    break;
                case "--workspace" or "-w" when i + 1 < args.Length:
                    workspaceRoot = args[++i];
                    break;
                case "--namespace" when i + 1 < args.Length:
                    namespaceName = args[++i];
                    break;
                case "--class" when i + 1 < args.Length:
                    className = args[++i];
                    break;
                case "--out" or "--output" when i + 1 < args.Length:
                    outputPath = args[++i];
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

        options = new EmitOptions
        {
            Path = path is null ? null : System.IO.Path.GetFullPath(path),
            ProjectPath = projectPath is null ? null : System.IO.Path.GetFullPath(projectPath),
            Lang = lang,
            Surface = surface,
            WorkspaceRoot = workspaceRoot,
            Namespace = namespaceName,
            ClassName = className,
            OutputPath = outputPath,
        };

        return true;
    }
}
